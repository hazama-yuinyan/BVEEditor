/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/20
 * Time: 14:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;

namespace Core.Presentation
{
	static class NativeMethods
	{
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);
	}
}
