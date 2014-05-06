/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 06/30/2013
 * Time: 20:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Core.Presentation;
using ICSharpCode.Core;

namespace BVEEditor.Options
{
	/// <summary>
	/// Simple implementation of IOptionPanel with support for OptionBinding markup extensions.
	/// </summary>
	public class OptionPanelViewModel : Screen, IOptionPanel
	{
        readonly List<OptionPanelViewModel> children = new List<OptionPanelViewModel>();
        protected readonly Properties app_settings;
        bool is_expanded, is_selected;

        public bool IsExpanded{
            get{return is_expanded;}
            set{
                if(is_expanded != value){
                    is_expanded = value;
                    NotifyOfPropertyChange(() => IsExpanded);
                }
            }
        }

        public bool IsSelected{
            get{return is_selected;}
            set{
                if(is_selected != value){
                    is_selected = value;
                    NotifyOfPropertyChange(() => IsSelected);
                }
            }
        }

        public OptionPanelViewModel(IPropertyService propertyService)
        {
            if(propertyService != null)
                app_settings = propertyService.NestedProperties("ApplicationSettings");
        }

        protected override void OnInitialize()
        {
            LoadOptions();
        }

        /// <summary>
        /// Initializes values corresponding to the UI.
        /// </summary>
        public virtual void LoadOptions()
        {
        }

        /// <summary>
        /// Responsible for saving UI values to Model class.
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveOptions()
        {
            return true;
        }

        #region IOptionPanel メンバー
        
        public string Title{
            get; set;
        }

        public IList<OptionPanelViewModel> Children{
            get {return children;}
        }

        #endregion
    }
}
