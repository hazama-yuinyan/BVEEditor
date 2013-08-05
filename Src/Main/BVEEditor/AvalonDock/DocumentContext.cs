using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace BVEEditor.AvalonDock
{
    public static class DocumentContext
    {
        public static void Init()
        {
            MessageBinder.SpecialValues.Add("$documentcontext", context => {
                LayoutDocument doc = null;
                if(context.EventArgs is DocumentClosingEventArgs)
                    doc = ((DocumentClosingEventArgs)context.EventArgs).Document;
                else if(context.EventArgs is DocumentClosedEventArgs)
                    doc = ((DocumentClosedEventArgs)context.EventArgs).Document;

                return doc.Content;
            });
        }
    }
}
