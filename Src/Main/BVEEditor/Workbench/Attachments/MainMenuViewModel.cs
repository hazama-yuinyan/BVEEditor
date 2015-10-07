using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BVEEditor.Views.Main
{
    /// <summary>
    /// The view model for the main menu.
    /// </summary>
    public class MainMenuViewModel : PropertyChangedBase, IMenu
    {
        const string MainMenuPath = "/BVEEditor/Workbench/MainMenu";
        static readonly Guid MainMenuGuid = new Guid("C7249984-1645-48B1-907E-F2946AECA725");
        readonly ILog Logger = LogManager.GetLog(typeof(MainMenuViewModel));
        BindableCollection<IMenu> items_before = new BindableCollection<IMenu>();
        BindableCollection<IMenu> items_after = new BindableCollection<IMenu>();

        #region Binding sources
        public FileMenuViewModel FileMenu{
            get; private set;
        }

        public EditMenuViewModel EditMenu{
            get; private set;
        }

        public ToolsMenuViewModel ToolsMenu{
            get; private set;
        }

        public HelpMenuViewModel HelpMenu{
            get; private set;
        }
        #endregion

        public MainMenuViewModel(FileMenuViewModel fileMenu, EditMenuViewModel editMenu, ToolsMenuViewModel toolsMenu,
            HelpMenuViewModel helpMenu)
        {
            var menu_descriptors = AddInTree.BuildItems<MenuItemDescriptor>(MainMenuPath, null);
            FileMenu = fileMenu;
            EditMenu = editMenu;
            ToolsMenu = toolsMenu;
            HelpMenu = helpMenu;
            /*Items = new BindableCollection<IRootMenu>{
                fileMenu,
                editMenu,
                toolsMenu,
                helpMenu
            };*/
            //foreach(var descriptor in menu_descriptors)
            //    descriptor.Codon
        }

        void SetWorkbenchOnChildMenus(IWorkbench workbench)
        {
            FileMenu.Workbench = workbench;
            EditMenu.Workbench = workbench;
            ToolsMenu.Workbench = workbench;
            HelpMenu.Workbench = workbench;

            foreach(var menu in ItemsBefore)
                menu.Workbench = workbench;

            foreach(var menu in ItemsAfter)
                menu.Workbench = workbench;
        }

        /*public IEnumerable<IResult> ShowQuickSearchPanel()
        {
            //yield return 
        }*/

        #region IMenu メンバー

        public BindableCollection<IMenu> ItemsBefore{
            get{return items_before;}
        }

        public BindableCollection<IMenu> ItemsAfter{
            get{return items_after;}
        }

        public string ReferenceAssemblyName{
            get{return "";}
        }

        public string MenuName{
            get{return "MainMenu";}
        }

        #endregion

        #region ICanReferToWorkbench メンバー

        // This property can't use constructor injection
        // because doing so creates a cyclic dependency.
        IWorkbench workbench;
        public IWorkbench Workbench{
            private get{return workbench;}
            set{
                workbench = value;
                SetWorkbenchOnChildMenus(value);
            }
        }

        #endregion

        #region IParent<IMenu> メンバー

        public IEnumerable<IMenu> GetChildren()
        {
            return ItemsBefore.Concat(ItemsAfter);
        }

        #endregion

        #region IParent メンバー

        System.Collections.IEnumerable IParent.GetChildren()
        {
            return GetChildren();
        }

        #endregion

        #region IUnique メンバー

        public Guid Id{
            get{return MainMenuGuid;}
        }

        #endregion

        #region IChild メンバー

        public object Parent{
            get{return Workbench;}
            set{
                Workbench = (IWorkbench)value;
            }
        }

        #endregion

        #region IActivate メンバー

        /// <summary>
        /// Activates all inactive child items.
        /// </summary>
        /// <remarks>
        /// Note that the Activated event is fired for the first time this menu is activated.
        /// </remarks>
        public void Activate()
        {
            Logger.Info("Activating the main menu.");
            foreach(var menu_item in ItemsBefore.Concat(ItemsAfter)){
                if(!menu_item.IsActive){
                    menu_item.Parent = this;
                    menu_item.Activate();
                }
            }

            if(!IsActive){
                IsActive = true;
                if(Activated != null){
                    Activated(this, new ActivationEventArgs{
                        WasInitialized = false
                    });
                }
            }
        }

        public event EventHandler<ActivationEventArgs> Activated;

        public bool IsActive{
            get; set;
        }

        #endregion

        #region IDeactivate メンバー

        public event EventHandler<DeactivationEventArgs> AttemptingDeactivation;

        public void Deactivate(bool close)
        {
            if(IsActive){
                if(AttemptingDeactivation != null){
                    AttemptingDeactivation(this, new DeactivationEventArgs{
                        WasClosed = close
                    });
                }

                IsActive = false;
                Logger.Info("Deactivating the main menu.");
                foreach(var menu_item in ItemsBefore.Concat(ItemsAfter))
                    menu_item.Deactivate(close);

                if(Deactivated != null){
                    Deactivated(this, new DeactivationEventArgs{
                        WasClosed = close
                    });
                }
            }
        }

        public event EventHandler<DeactivationEventArgs> Deactivated;

        #endregion
    }
}
