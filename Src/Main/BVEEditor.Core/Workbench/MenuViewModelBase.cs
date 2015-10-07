using Caliburn.Micro;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BVEEditor.Workbench
{
    /// <summary>
    /// Base class for all menu view models.
    /// </summary>
    public class MenuViewModelBase : PropertyChangedBase, IMenu
    {
        protected readonly ILog Logger;

        BindableCollection<IMenu> items_before = new BindableCollection<IMenu>();
        BindableCollection<IMenu> items_after = new BindableCollection<IMenu>();

        #region IMenu メンバー

        public BindableCollection<IMenu> ItemsBefore{
            get{return items_before;}
        }

        public BindableCollection<IMenu> ItemsAfter{
            get{return items_after;}
        }

        public string ReferenceAssemblyName{
            get; private set;
        }

        public string MenuName{
            get; private set;
        }

        #endregion

        #region IChild メンバー

        public object Parent{
            get; set;
        }

        #endregion

        #region IUnique メンバー

        public Guid Id{
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICanReferToWorkbench メンバー

        public IWorkbench Workbench{
            protected get; set;
        }

        #endregion

        #region IActivate メンバー

        public void Activate()
        {
            if(IsActive)
                return;

            Logger.Info("Activating the menu named '{0}'.", MenuName);
            IsActive = true;
            if(Activated != null){
                Activated(this, new ActivationEventArgs{
                    WasInitialized = false
                });
            }
        }

        public event EventHandler<ActivationEventArgs> Activated;

        public bool IsActive{
            get; private set;
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

                Logger.Info("Deactivating {0}.", MenuName);
                IsActive = false;
                if(Deactivated != null){
                    Deactivated(this, new DeactivationEventArgs{
                        WasClosed = close
                    });
                }
            }
        }

        public event EventHandler<DeactivationEventArgs> Deactivated;

        #endregion

        protected MenuViewModelBase(ILog logger, string menuName, string refereceAssemblyName = null)
        {
            Logger = logger;
            ReferenceAssemblyName = refereceAssemblyName;
            MenuName = menuName;
        }

        #region IParent メンバー

        System.Collections.IEnumerable IParent.GetChildren()
        {
            return GetChildren();
        }

        #endregion

        #region IParent<IMenu> メンバー

        public IEnumerable<IMenu> GetChildren()
        {
            return ItemsBefore.Concat(ItemsAfter);
        }

        #endregion

        public static IEnumerable<IMenu> TransformMenuItems(IEnumerable<MenuItemDescriptor> descriptors)
        {
            foreach(var desc in descriptors){
                yield return null;
            }
        }
    }
}
