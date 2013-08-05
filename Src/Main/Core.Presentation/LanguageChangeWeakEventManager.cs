/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/27
 * Time: 13:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using ICSharpCode.Core;

namespace Core.Presentation
{
	public sealed class LanguageChangeWeakEventManager : WeakEventManager
	{
		/// <summary>
		/// Adds a weak event listener.
		/// </summary>
		public static void AddListener(IWeakEventListener listener)
		{
			CurrentManager.ProtectedAddListener(null, listener);
		}
		
		/// <summary>
		/// Removes a weak event listener.
		/// </summary>
		public static void RemoveListener(IWeakEventListener listener)
		{
			CurrentManager.ProtectedRemoveListener(null, listener);
		}
		
		/// <summary>
		/// Gets the current manager.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		private static LanguageChangeWeakEventManager CurrentManager {
			get {
				LanguageChangeWeakEventManager manager = (LanguageChangeWeakEventManager)GetCurrentManager(typeof(LanguageChangeWeakEventManager));
				if (manager == null) {
					manager = new LanguageChangeWeakEventManager();
					SetCurrentManager(typeof(LanguageChangeWeakEventManager), manager);
				}
				return manager;
			}
		}
		
		readonly IResourceService resourceService = ServiceSingleton.GetRequiredService<IResourceService>();
		
		protected override void StartListening(object source)
		{
			resourceService.LanguageChanged += DeliverEvent;
		}
		
		protected override void StopListening(object source)
		{
			resourceService.LanguageChanged -= DeliverEvent;
		}
	}
}
