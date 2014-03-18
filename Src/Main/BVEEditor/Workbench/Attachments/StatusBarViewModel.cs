using System.ComponentModel;
using BVEEditor.Editor;
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
        StatusBarMessageChangedEvent last_msg;
        CaretPositionChangedEvent last_caret_pos;

        #region Binding sources
        bool is_text_file_active;
        /// <summary>
        /// Gets whether the active ViewDocument is a text file or not.
        /// </summary>
        public bool IsTextFileActive{
            get{return is_text_file_active;}
            private set{
                if(is_text_file_active != value){
                    is_text_file_active = value;
                    NotifyOfPropertyChange(() => IsTextFileActive);
                }
            }
        }

        string msg_text;
        public string MessageText{
            get{return msg_text;}
            private set{
                if(msg_text != value){
                    msg_text = value;
                    NotifyOfPropertyChange(() => MessageText);
                }
            }
        }

        string cursor_info;
        public string CursorPositionText{
            get{return cursor_info;}
            private set{
                if(cursor_info != value){
                    cursor_info = value;
                    NotifyOfPropertyChange(() => CursorPositionText);
                }
            }
        }

        bool is_insert_mode;
        public bool IsInsertMode{
            get{return is_insert_mode;}
            private set{
                if(is_insert_mode != value){
                    is_insert_mode = value;
                    NotifyOfPropertyChange(() => IsInsertMode);
                }
            }
        }

        string encoding_name;
        public string EncodingName{
            get{return encoding_name;}
            private set{
                if(encoding_name != value){
                    encoding_name = value;
                    NotifyOfPropertyChange(() => EncodingName);
                }
            }
        }
        #endregion

        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe(this);
            WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.PropertyChanged += LanguageChanged;
        }

        public void LanguageChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Culture"){
                if(last_msg != null)
                    SetMessage(last_msg.MessageKey, last_msg.CustomTags);
                
                if(last_caret_pos != null)
                    SetCaretPosition(last_caret_pos.Line, last_caret_pos.Column, last_caret_pos.CharNumber);
            }
        }

        void SetMessage(string messageKey, StringTagPair[] customTags)
        {
            MessageText = StringParser.Parse(messageKey, customTags);
        }

        void SetCaretPosition(int line, int column, int charNumber)
        {
            CursorPositionText = StringParser.Parse(
                "${res:BVEEditor:StringResources:StatusBar.CursorPanelText}",
                new StringTagPair("line", string.Format("{0,-10}", line)),
                new StringTagPair("column", string.Format("{0,-5}", column)),
                new StringTagPair("character", string.Format("{0,-5}", charNumber))
            );
        }

        #region IHandle<CaretPositionChangedEvent> メンバー

        public void Handle(CaretPositionChangedEvent message)
        {
            SetCaretPosition(message.Line, message.Column, message.CharNumber);
            last_caret_pos = message;
        }

        #endregion

        #region IHandle<StatusBarMessageChangedEvent> メンバー

        public void Handle(StatusBarMessageChangedEvent message)
        {
            SetMessage(message.MessageKey, message.CustomTags);
            last_msg = message;
        }

        #endregion

        #region IHandle<ActiveViewDocumentChangedEvent> メンバー

        public void Handle(ActiveViewDocumentChangedEvent message)
        {
            IsTextFileActive = message.ViewDocument is IPositionable;
            var encoding_provider = message.ViewDocument as ITextEncodingProvider;
            if(encoding_provider != null)
                EncodingName = encoding_provider.TextEncoding.EncodingName;
        }

        #endregion
    }
}
