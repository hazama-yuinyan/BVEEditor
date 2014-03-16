// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design;
using Ninject;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Registers a service in the Ninject kernel.
	/// </summary>
	/// <attribute name="bindingFrom" use="required">
	/// The service interface type.
	/// </attribute>
	/// <attribute name="bindingTo" use="required">
	/// The implementing service class name.
	/// </attribute>
	/// <usage>Only in /BVEEditor/Services</usage>
	/// <returns>
	/// <c>null</c>. The service is registered, but not returned.
	/// </returns>
	public class ServiceDoozer : IDoozer
	{
		public bool HandleConditions{
			get{return false;}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			var kernel = (IKernel)args.Parameter;
			if(kernel == null)
				throw new InvalidOperationException("Expected the parameter to be an IKernel");
			
            Type interface_type = args.AddIn.FindType(args.Codon.Properties["bindingFrom"]);
			if(interface_type != null){
				string class_name = args.Codon.Properties["bindingTo"];
                Type concrete_type = args.AddIn.FindType(class_name);
				kernel.Bind(interface_type)
                      .To(concrete_type)
                      .InSingletonScope();
			}
			return null;
		}
	}
}
