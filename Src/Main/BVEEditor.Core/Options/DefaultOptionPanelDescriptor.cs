// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;

namespace BVEEditor.Options
{
	public class DefaultOptionPanelDescriptor : IOptionPanelDescriptor
	{
		string id = String.Empty;
		List<IOptionPanelDescriptor> option_panel_descriptors = null;
		
		public string ID{
			get{
				return id;
			}
		}
		
		public string Label{get; set;}
		
		public IEnumerable<IOptionPanelDescriptor> ChildOptionPanelDescriptors {
			get {
				return option_panel_descriptors;
			}
		}
		
        AddIn addin;
		object owner;
		string viewmodel_name;
		
		public bool HasOptionPanel {
			get {
				return viewmodel_name != null;
			}
		}

		public DefaultOptionPanelDescriptor(string id, string label, AddIn addin)
		{
			this.id    = id;
			this.Label = label;
            this.addin = addin;
		}
		
		public DefaultOptionPanelDescriptor(string id, string label, AddIn addin, List<IOptionPanelDescriptor> dialogPanelDescriptors)
            : this(id, label, addin)
		{
			this.option_panel_descriptors = dialogPanelDescriptors;
		}
		
		public DefaultOptionPanelDescriptor(string id, string label, AddIn addin, object owner, string viewModelName) :
            this(id, label, addin)
		{
			this.owner = owner;
			this.viewmodel_name = viewModelName;
		}

        public OptionPanelViewModel CreateViewModel()
        {
            if(!HasOptionPanel){
                addin.LoadRuntimeAssemblies();  // Ensures that the dependency assemblies are loaded

                var viewmodel = new OptionCategoryViewModel();
                viewmodel.Title = StringParser.Parse(Label);
                
                foreach(var item in option_panel_descriptors){
                    var child_viewmodel = item.CreateViewModel();
                    viewmodel.Children.Add(child_viewmodel);
                }
                return viewmodel;
            }else{
                var viewmodel = (OptionPanelViewModel)addin.CreateObject(viewmodel_name);
                viewmodel.Title = StringParser.Parse(Label);
                return viewmodel;
            }
        }
	}
}
