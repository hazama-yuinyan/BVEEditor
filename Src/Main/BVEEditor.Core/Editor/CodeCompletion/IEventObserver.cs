using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// Interface for event observers.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TCancel"></typeparam>
    /// <typeparam name="TSubject"></typeparam>
    public interface IEventObserver<in TEvent, in TCancel, in TSubject>
    {
        void Preview(IEnumerable<TEvent> events, TCancel current, TSubject view);
        void Handle(IEnumerable<TEvent> events, TSubject view);
    }
}
