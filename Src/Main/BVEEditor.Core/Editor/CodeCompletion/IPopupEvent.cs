﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BVEEditor.Editor.CodeCompletion
{
    /// <summary>
    /// Interface for Popup events.
    /// </summary>
    public interface IPopupEvent
    {
        EventType Type{get;}
        EventSource Source{get;}
        object EventArgs{get;}
    }
}
