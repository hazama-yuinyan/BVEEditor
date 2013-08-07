﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using Caliburn.Micro;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Extensions;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class parses internal ${xyz} tags of BVEEditor.
	/// All environment variables are avaible under the name env.[NAME]
	/// where [NAME] represents the string under which it is avaiable in
	/// the environment.
	/// </summary>
	public static class StringParser
	{
		readonly static ConcurrentDictionary<string, IStringTagProvider> prefixedStringTagProviders
			= InitializePrefixedStringTagProviders();
		
		// not really a stack - we only use Add and GetEnumerator
		readonly static ConcurrentStack<IStringTagProvider> stringTagProviders = new ConcurrentStack<IStringTagProvider>();
		
		static ConcurrentDictionary<string, IStringTagProvider> InitializePrefixedStringTagProviders()
		{
			var dict = new ConcurrentDictionary<string, IStringTagProvider>(StringComparer.OrdinalIgnoreCase);
			
			// entryAssembly == null might happen in unit test mode
			Assembly entry_assembly = Assembly.GetEntryAssembly();
			if(entry_assembly != null){
				string exe_name = entry_assembly.Location;
				dict["exe"] = new PropertyObjectTagProvider(FileVersionInfo.GetVersionInfo(exe_name));
			}
			
			return dict;
		}
		
		/// <summary>
		/// Escapes all occurrences of '${' to '${$}{'.
		/// </summary>
		public static string Escape(string input)
		{
			if(input == null)
				throw new ArgumentNullException("input");
			
            return input.Replace("${", "${$}{");
		}

        /// <summary>
        /// Replaces all occurrences of a dot '.' with an underscore '_' in order to make the identifier valid.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ValidateIdentifier(string name)
        {
            if(name == null)
                throw new ArgumentNullException("name");

            return name.Replace('.', '_');
        }
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public static string Parse(string input)
		{
			return Parse(input, (StringTagPair[])null);
		}
		
		public static void RegisterStringTagProvider(IStringTagProvider tagProvider)
		{
			if(tagProvider == null)
				throw new ArgumentNullException("tagProvider");
			
            stringTagProviders.Push(tagProvider);
		}
		
		public static void RegisterStringTagProvider(string prefix, IStringTagProvider tagProvider)
		{
			if(prefix == null)
				throw new ArgumentNullException("prefix");
			if(tagProvider == null)
				throw new ArgumentNullException("tagProvider");
			
            prefixedStringTagProviders[prefix] = tagProvider;
		}
		
		//readonly static Regex pattern = new Regex(@"\$\{([^\}]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public static string Parse(string input, params StringTagPair[] customTags)
		{
			if(input == null)
				return null;
			
            int pos = 0;
			StringBuilder output = null; // don't use StringBuilder if input is a single property
			
            do{
				int old_pos = pos;
				pos = input.IndexOf("${", pos, StringComparison.Ordinal);
				if(pos < 0){
					if(output == null){
						return input;
					}else{
						if(old_pos < input.Length){
							// normal text after last property
							output.Append(input, old_pos, input.Length - old_pos);
						}
						return output.ToString();
					}
				}

				if(output == null){
					if(pos == 0)
						output = new StringBuilder();
					else
						output = new StringBuilder(input, 0, pos, pos + 16);
				}else{
					if(pos > old_pos){
						// normal text between two properties
						output.Append(input, old_pos, pos - old_pos);
					}
				}

				int end = input.IndexOf('}', pos + 1);
				if(end < 0){
					output.Append("${");
					pos += 2;
				}else{
					string property = input.Substring(pos + 2, end - pos - 2);
					string val = GetValue(property, customTags);
					if(val == null){
						output.Append("${");
						output.Append(property);
						output.Append('}');
					}else{
						output.Append(val);
					}
					pos = end + 1;
				}
			}while(pos < input.Length);
			
            return output.ToString();
		}
		
		/// <summary>
		/// Evaluates a property using the StringParser. Equivalent to StringParser.Parse("${" + propertyName + "}");
		/// </summary>
		public static string GetValue(string propertyName, params StringTagPair[] customTags)
		{
			if(propertyName == null)
				throw new ArgumentNullException("propertyName");
			if(propertyName == "$")
				return "$";
			
			if(customTags != null){
				foreach(StringTagPair pair in customTags){
					if(propertyName.Equals(pair.Tag, StringComparison.OrdinalIgnoreCase))
						return pair.Value;
				}
			}
			
			int k = propertyName.IndexOf(':');
			if(k <= 0){
				// it's property without prefix
				
				if(propertyName.Equals("DATE", StringComparison.OrdinalIgnoreCase))
					return DateTime.Today.ToShortDateString();
				if(propertyName.Equals("TIME", StringComparison.OrdinalIgnoreCase))
					return DateTime.Now.ToShortTimeString();
				if(propertyName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
					return IoC.Get<IMessageService>().ProductName;
				if(propertyName.Equals("GUID", StringComparison.OrdinalIgnoreCase))
					return Guid.NewGuid().ToString().ToUpperInvariant();
				if(propertyName.Equals("USER", StringComparison.OrdinalIgnoreCase))
					return Environment.UserName;
				if(propertyName.Equals("Version", StringComparison.OrdinalIgnoreCase))
					return "1.00";
				if(propertyName.Equals("CONFIGDIRECTORY", StringComparison.OrdinalIgnoreCase))
					return IoC.Get<IPropertyService>().ConfigDirectory;
				
				foreach(IStringTagProvider provider in stringTagProviders){
					string result = provider.ProvideString(propertyName, customTags);
					if(result != null)
						return result;
				}
				
				return null;
			}else{
				// it's a prefixed property
				
				
				// res: properties are quite common, so optimize by testing for them first
				// before allocaing the prefix/propertyName strings
				// All other prefixed properties {prefix:Key} shoulg get handled in the switch below.
				if(propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase)){
                    string str_resource;
                    var loc_extension = new LocExtension(ValidateIdentifier(propertyName.Substring(4)));
                    if(!loc_extension.ResolveLocalizedValue(out str_resource))
                        return null;
                    
                    return Parse(str_resource, customTags);
				}
				
				string prefix = propertyName.Substring(0, k);
				propertyName = propertyName.Substring(k + 1);
				switch(prefix.ToUpperInvariant()){
					case "SDKTOOLPATH":
						return FileUtility.GetSdkPath(propertyName);

					case "ADDINPATH":
						foreach(var addIn in IoC.Get<IAddInTree>().AddIns){
							if(addIn.Manifest.Identities.ContainsKey(propertyName))
								return System.IO.Path.GetDirectoryName(addIn.FileName);
						}
						return null;
					
                    case "DATE":
						try{
							return DateTime.Now.ToString(propertyName, CultureInfo.CurrentCulture);
						}catch(Exception ex){
							return ex.Message;
						}

					case "ENV":
						return Environment.GetEnvironmentVariable(propertyName);

					case "PROPERTY":
						return GetProperty(propertyName);

					default:
						IStringTagProvider provider;
						if(prefixedStringTagProviders.TryGetValue(prefix, out provider))
							return provider.ProvideString(propertyName, customTags);
						else
							return null;
				}
			}
		}
		
		/// <summary>
		/// Applies the StringParser to the formatstring; and then calls <c>string.Format</c> on the result.
		/// 
		/// This method is equivalent to:
		/// <code>return string.Format(StringParser.Parse(formatstring), formatitems);</code>
		/// but additionally includes error handling.
		/// </summary>
		public static string Format(string formatstring, params object[] formatitems)
		{
			try{
				return String.Format(StringParser.Parse(formatstring), formatitems);
			}catch(FormatException ex){
				DebugLogger.Instance.Warn(ex.ToString());
				
				StringBuilder b = new StringBuilder(StringParser.Parse(formatstring));
				foreach(object formatitem in formatitems){
					b.Append("\nItem: ");
					b.Append(formatitem);
				}
				return b.ToString();
			}
		}
		
		/// <summary>
		/// Allow special syntax to retrieve property values:
		/// ${property:PropertyName}
		/// ${property:PropertyName??DefaultValue}
		/// ${property:ContainerName/PropertyName}
		/// ${property:ContainerName/PropertyName??DefaultValue}
		/// A container is a Properties instance stored in the PropertyService. This is
		/// used by many AddIns to group all their properties into one container.
		/// </summary>
		static string GetProperty(string propertyName)
		{
			string default_value = "";
			int pos = propertyName.LastIndexOf("??", StringComparison.Ordinal);
			if(pos >= 0){
				default_value = propertyName.Substring(pos + 2);
				propertyName = propertyName.Substring(0, pos);
			}
			
            Properties properties = IoC.Get<IPropertyService>().MainPropertiesContainer;
			pos = propertyName.IndexOf('/');
			while(pos >= 0){
				properties = properties.NestedProperties(propertyName.Substring(0, pos));
				propertyName = propertyName.Substring(pos + 1);
				pos = propertyName.IndexOf('/');
			}

			return properties.Get(propertyName, default_value);
		}
	}
	
    /// <summary>
    /// Represents a tag-value pair in "${xyz}" notation.
    /// </summary>
	public struct StringTagPair
	{
		readonly string tag;
		readonly string value;
		
		public string Tag {
			get { return tag; }
		}
		
		public string Value {
			get { return value; }
		}
		
		public StringTagPair(string tag, string value)
		{
			if(tag == null)
				throw new ArgumentNullException("tag");
			if(value == null)
				throw new ArgumentNullException("value");
			
            this.tag = tag;
			this.value = value;
		}
	}
}
