using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Logging;
using BVEEditor.Result;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Options
{
    /// <summary>
    /// The view model for the Options dialog.
    /// </summary>
    public class OptionsViewModel : ShellPresentationViewModel, IConductActiveItem
    {
        const string OptionPanelsPath = "/BVEEditor/Dialogs/OptionsDialog";

        List<OptionPanelViewModel> options = new List<OptionPanelViewModel>();

        OptionPanelViewModel active_panel;
        public OptionPanelViewModel ActivePanel{
            get{return active_panel;}
            set{
                if(active_panel != value){
                    active_panel = value;
                    NotifyOfPropertyChange(() => ActivePanel);
                }
            }
        }

        public IList<OptionPanelViewModel> Items{
            get{return options;}
        }

        public OptionsViewModel(IResultFactory resultFactory) : base(resultFactory)
        {
            var option_panels = AddInTree.BuildItems<IOptionPanelDescriptor>(OptionPanelsPath, this, false);
            foreach(var panel in option_panels)
                options.Add(CreateOptionPanel(panel));

            DisplayName = StringParser.Parse("${res:BVEEditor:StringResources:Common.Dialogs.Captions.Options}");
        }

        OptionPanelViewModel CreateOptionPanel(IOptionPanelDescriptor descriptor)
        {
            var panel = descriptor.CreateViewModel();
            panel.PropertyChanged += OptionPanelPropertyChanged;
            return panel;
        }

        public IEnumerable<IResult> PressedOk()
        {
            foreach(var option in options){
                if(!option.SaveOptions()){
                    yield return Result.ShowMessageBox("${res:BVEEditor:StringResources:Common.TextError}",
                        "${res:BVEEditor:StringResources:OptionsDialog.FailedToSaveOptions}", System.Windows.MessageBoxButton.OK);
                    break;
                }
            }
            yield return Result.Close();
        }

        public IEnumerable<IResult> PressedCancel()
        {
            yield return Result.Close();
        }

        void OptionPanelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsSelected"){
                var category = sender as OptionCategoryViewModel;
                if(category != null){
                    ActivePanel = category.Children[0];
                }else{
                    var panel = sender as OptionPanelViewModel;
                    if(panel != null)
                        ActivePanel = panel;
                }
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if(ActiveItem != null)
                DeactivateItem(ActiveItem, close);
        }

        #region IConductor メンバー

        public void ActivateItem(object item)
        {
            if(item != null && item.Equals(ActiveItem)){
                if(IsActive){
                    ScreenExtensions.TryActivate(item);
                    OnActivationProcessed(item, true);
                }
                return;
            }

            ChangeActiveItem(item, false);
        }

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        public void DeactivateItem(object item, bool close)
        {
            if(item == null) return;

            ScreenExtensions.TryDeactivate(item, close);
        }

        #endregion

        object EnsureItem(object item)
        {
            var node = item as IChild;
            if(node != null && node.Parent != this)
                node.Parent = this;

            return node;
        }

        void ChangeActiveItem(object item, bool closePrevious)
        {
            ScreenExtensions.TryDeactivate(ActiveItem, closePrevious);

            item = EnsureItem(item);

            if(IsActive)
                ScreenExtensions.TryActivate(item);

            ActiveItem = item;
            NotifyOfPropertyChange("ActiveItem");
            OnActivationProcessed(item, true);
        }

        void OnActivationProcessed(object item, bool success)
        {
            if(item == null) return;

            if(ActivationProcessed != null){
                ActivationProcessed(this, new ActivationProcessedEventArgs{
                    Item = item,
                    Success = success
                });
            }
        }

        #region IParent メンバー

        public System.Collections.IEnumerable GetChildren()
        {
            return options;
        }

        #endregion

        #region IHaveActiveItem メンバー

        public object ActiveItem{
            get{
                return ActivePanel;
            }
            set{
                active_panel = (OptionPanelViewModel)value;
            }
        }

        #endregion
    }
}
