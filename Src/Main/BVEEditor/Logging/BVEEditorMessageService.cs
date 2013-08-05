/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/26
 * Time: 15:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Core.WinForms;

namespace BVEEditor.Logging
{
	sealed class BVEEditorMessageService : WinFormsMessageService
	{
		public override void ShowException(Exception ex, string message)
		{
			SD.Log.Error(message, ex);
			SD.Log.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			/*if (ex != null)
				ExceptionBox.ShowErrorBox(ex, message);
			else*/
				ShowError(message);
		}
	}
}
