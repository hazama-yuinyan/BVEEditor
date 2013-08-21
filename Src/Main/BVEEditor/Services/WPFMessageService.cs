using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BVEEditor.Result;
using ICSharpCode.Core;

namespace BVEEditor.Services
{
    public class WPFMessageService : IMessageService
    {
        #region IMessageService メンバー

        public void ShowException(Exception ex, string message = null)
        {
            MessageBox.Show(ex.Message, "Exception!", MessageBoxButton.OK);
        }

        public void ShowHandledException(Exception ex, string message = null)
        {
            MessageBox.Show(ex.Message, "Unhandled exception!", MessageBoxButton.OK);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK);
        }

        public void ShowErrorFormatted(string formatstring, params object[] formatitems)
        {
            MessageBox.Show(string.Format(formatstring, formatitems), "Error", MessageBoxButton.OK);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK);
        }

        public void ShowWarningFormatted(string formatstring, params object[] formatitems)
        {
            MessageBox.Show(string.Format(formatstring, formatitems), "Warning", MessageBoxButton.OK);
        }

        public void ShowMessage(string message, string caption = null)
        {
            MessageBox.Show(message, caption ?? "Information", MessageBoxButton.OK);
        }

        public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
        {
            MessageBox.Show(string.Format(formatstring, formatitems), caption, MessageBoxButton.OK);
        }

        public bool AskQuestion(string question, string caption = null)
        {
            var res = MessageBox.Show(question, caption ?? "Question", MessageBoxButton.YesNo);
            return res == System.Windows.MessageBoxResult.Yes;
        }

        public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
        {
            throw new NotImplementedException();
        }

        public string ShowInputBox(string caption, string dialogText, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public string DefaultMessageBoxTitle
        {
            get { throw new NotImplementedException(); }
        }

        public string ProductName
        {
            get { throw new NotImplementedException(); }
        }

        public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
        {
            throw new NotImplementedException();
        }

        public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
