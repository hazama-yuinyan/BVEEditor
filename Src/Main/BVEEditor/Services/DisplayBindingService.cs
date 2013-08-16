using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Workbench;
using ICSharpCode.Core;

namespace BVEEditor.Services
{
    public class DisplayBindingService : IDisplayBindingService
    {
        const string DisplayBindingPath = "BVEEditor/Workbench/DisplayBindings";
        List<DisplayBindingDescriptor> descriptors;
        ICSharpCode.Core.Properties display_binding_properties;
        IMessageLoop main_thread;

        public DisplayBindingService(IPropertyService propertyService, IMessageLoop mainThread)
        {
            main_thread = mainThread;

            descriptors = AddInTree.BuildItems<DisplayBindingDescriptor>(DisplayBindingPath, null, true);
            display_binding_properties = propertyService.NestedProperties("DisplayBindingService");
        }

        #region IDisplayBindingService メンバー

        public IDisplayBinding GetBindingPerFileName(FileName filename)
        {
            main_thread.VerifyAccess();

            if(FileUtility.IsUrl(filename)){
                // The normal display binding dispatching code can't handle URLs (e.g. because it uses Path.GetExtension),
                // so we'll directly return null at the moment.
                return null;
            }

            var codon = GetDefaultCodonPerFileName(filename);
            return (codon == null) ? null : codon.Binding;
        }

        public DisplayBindingDescriptor GetDefaultCodonPerFileName(FileName filename)
        {
            main_thread.VerifyAccess();

            var default_command_id = display_binding_properties.Get("Default" + Path.GetExtension(filename).ToLowerInvariant(), string.Empty);
            if(!string.IsNullOrEmpty(default_command_id)){
                foreach(var descriptor in descriptors){
                    if(descriptor.Id == default_command_id && IsValidBindingForFileName(descriptor, filename))
                        return descriptor;
                }
            }

            //DisplayBindingDescriptor auto_detected_descriptor = null;
            foreach(var descriptor in descriptors){
                if(IsValidBindingForFileName(descriptor, filename)){
                    if(descriptor.Binding.IsPreferredBindingForFile(filename))
                        return descriptor;
                }
            }
            return null;
        }

        public void SetDefaultCodon(string extension, DisplayBindingDescriptor bindingDescriptor)
        {
            main_thread.VerifyAccess();

            if(bindingDescriptor == null)
                throw new ArgumentNullException("bindingDescriptor");

            if(extension == null)
                throw new ArgumentNullException("extension");

            if(!extension.StartsWith(".", StringComparison.Ordinal))
                throw new ArgumentException("extension must start with '.'");

            display_binding_properties.Set("Default" + extension.ToLowerInvariant(), bindingDescriptor.Id);
        }

        public IReadOnlyList<DisplayBindingDescriptor> GetCodonsPerFileName(FileName filename)
        {
            main_thread.VerifyAccess();

            var list = new List<DisplayBindingDescriptor>();
            foreach(var desc in descriptors){
                if(IsValidBindingForFileName(desc, filename))
                    list.Add(desc);
            }

            return list;
        }

        static bool IsValidBindingForFileName(DisplayBindingDescriptor binding, FileName filename)
        {
            return binding.CanOpenFile(filename) && binding.Binding != null && binding.Binding.CanHandle(filename);
        }

        #endregion
    }
}
