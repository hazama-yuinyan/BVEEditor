﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Caliburn.Micro;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class for easy logging.
	/// </summary>
	public static class LoggingService
	{
		static ILoggingService Service {
			get { return IoC.Get<ILoggingService>(); }
		}
		
		public static void Debug(object message)
		{
			Service.Debug(message);
		}
		
		public static void DebugFormatted(string format, params object[] args)
		{
			Service.DebugFormatted(format, args);
		}
		
		public static void Info(object message)
		{
			Service.Info(message);
		}
		
		public static void InfoFormatted(string format, params object[] args)
		{
			Service.InfoFormatted(format, args);
		}
		
		public static void Warn(object message)
		{
			Service.Warn(message);
		}
		
		public static void Warn(object message, Exception exception)
		{
			Service.Warn(message, exception);
		}
		
		public static void WarnFormatted(string format, params object[] args)
		{
			Service.WarnFormatted(format, args);
		}
		
		public static void Error(object message)
		{
			Service.Error(message);
		}
		
		public static void Error(object message, Exception exception)
		{
			Service.Error(message, exception);
		}
		
		public static void ErrorFormatted(string format, params object[] args)
		{
			Service.ErrorFormatted(format, args);
		}
		
		public static void Fatal(object message)
		{
			Service.Fatal(message);
		}
		
		public static void Fatal(object message, Exception exception)
		{
			Service.Fatal(message, exception);
		}
		
		public static void FatalFormatted(string format, params object[] args)
		{
			Service.FatalFormatted(format, args);
		}
		
		public static bool IsDebugEnabled {
			get {
				return Service.IsDebugEnabled;
			}
		}
		
		public static bool IsInfoEnabled {
			get {
				return Service.IsInfoEnabled;
			}
		}
		
		public static bool IsWarnEnabled {
			get {
				return Service.IsWarnEnabled;
			}
		}
		
		public static bool IsErrorEnabled {
			get {
				return Service.IsErrorEnabled;
			}
		}
		
		public static bool IsFatalEnabled {
			get {
				return Service.IsFatalEnabled;
			}
		}
	}
}
