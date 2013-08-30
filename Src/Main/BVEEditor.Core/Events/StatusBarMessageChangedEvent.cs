using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Core;

namespace BVEEditor.Events
{
    /// <summary>
    /// Event fired when the message of the status bar is changed.
    /// </summary>
    public class StatusBarMessageChangedEvent
    {
        public string MessageKey{get; private set;}
        public StringTagPair[] CustomTags{get; private set;}

        /// <summary>
        /// Creates a new <see cref="BVEEditor.Events.StatusBarMessageChangedEvent"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="message"/> will be passed to <see cref="ICSharpCode.Core.StringParser.Parse"/> method
        /// before set.
        /// </remarks>
        public StatusBarMessageChangedEvent(string message) : this(message, null){}

        /// <summary>
        /// Creates a new <see cref="BVEEditor.Events.StatusBarMessageChangedEvent"/> using
        /// <see cref="ICSharpCode.Core.StringParser.Parse"/> method.
        /// </summary>
        public StatusBarMessageChangedEvent(string message, params StringTagPair[] customTags)
        {
            MessageKey = message;
            CustomTags = customTags;
        }
    }
}
