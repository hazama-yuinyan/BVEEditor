using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Events;
using BVEEditor.Workbench;
using Caliburn.Micro;
using ICSharpCode.Core;

namespace BVEEditor.Views.Main
{
    /// <summary>
    /// The view model for the status bar.
    /// </summary>
    public class StatusBarViewModel : PropertyChangedBase, IHandle<CaretPositionChangedEvent>, IHandle<StatusBarMessageChangedEvent>,
        IHandle<ActiveViewDocumentChangedEvent>
    {
        #region Binding sources
        bool is_text_file_active;
        public bool IsTextFileActive{
            get{return is_text_file_active;}
            private set{
                if(is_text_file_active != value){
                    is_text_file_active = value;
                    NotifyOfPropertyChange(() => is_text_file_active);
                }
            }
        }

        string msg_text;
        public string MessageText{
            get{return msg_text;}
            private set{
                if(msg_text != value){
                    msg_text = value;
                    NotifyOfPropertyChange(() => msg_text);
                }
            }
        }

        string cursor_info;
        public string CursorPositionText{
            get{return cursor_info;}
            private set{
                if(cursor_info != value){
                    cursor_info = value;
                    NotifyOfPropertyChange(() => cursor_info);
                }
            }
        }

        bool is_insert_mode;
        public bool IsInsertMode{
            get{return is_insert_mode;}
            private set{
                if(is_insert_mode != value){
                    is_insert_mode = value;
                    NotifyOfPropertyChange(() => is_insert_mode);
                }
            }
        }

        string encoding_name;
        public string EncodingName{
            get{return encoding_name;}
            private set{
                if(encoding_name != value){
                    encoding_name = value;
                    NotifyOfPropertyChange(() => encoding_name);
                }
            }
        }
        #endregion

        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
        }

        #region IHandle<CaretPositionChangedEvent> メンバー

        public void Handle(CaretPositionChangedEvent message)
        {
            CursorPositionText = StringParser.Parse(
                "${res:BVEEditor:StringResources:StatusBar.CursorPanelText}",
                new StringTagPair("line", string.Format("{0,-10}", message.Line)),
                new StringTagPair("column", string.Format("{0,-5}", message.Column)),
                new StringTagPair("character", string.Format("{0,-5}", message.CharNumber))
            );
        }

        #endregion

        #region IHandle<StatusBarMessageChangedEvent> メンバー

        public void Handle(StatusBarMessageChangedEvent message)
        {
            MessageText = message.Message;
        }

        #endregion

        #region IHandle<ActiveViewDocumentChangedEvent> メンバー

        public void Handle(ActiveViewDocumentChangedEvent message)
        {
            IsTextFileActive = message.ViewDocument is IPositionable;
        }

        #endregion
    }
}
