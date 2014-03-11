using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion
{
    public interface ICancellablePopupEvent : IPopupEvent
    {
        void Cancel();
        bool IsCancelled{get;}
        bool IsTransient{get;}
    }
}
