using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// Interface for insight window handlers.
    /// </summary>
    public interface IInsightWindowHandler
    {
        object Content{get; set;}
        void Show();
        void Close();
    }
}
