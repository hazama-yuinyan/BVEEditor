using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Workbench;

namespace BVEEditor.Events
{
    public class ActiveViewContentChangedEvent
    {
        public ViewContentViewModel Content{get; private set;}
        public ActiveViewContentChangedEvent(ViewContentViewModel content)
        {
            Content = content;
        }
    }
}
