﻿using Caliburn.Micro;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// The view model for Edit menu.
    /// </summary>
    public class EditMenuViewModel : MenuViewModelBase
    {
        public EditMenuViewModel() : base(LogManager.GetLog(typeof(EditMenuViewModel)), "Edit")
        {
            ItemsBefore.AddRange(MenuViewModelBase.TransformMenuItems(AddInTree.BuildItems<MenuItemDescriptor>("/BVEEditor/Workbench/MainMenu/EditMenu/Before", null)));
            ItemsAfter.AddRange(MenuViewModelBase.TransformMenuItems(AddInTree.BuildItems<MenuItemDescriptor>("/BVEEditor/Workbench/MainMenu/EditMenu/After", null)));
        }
    }
}
