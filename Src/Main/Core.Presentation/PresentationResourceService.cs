/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/16
 * Time: 11:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;

namespace Core.Presentation
{
	/// <summary>
	/// Creates WPF BitmapSource objects from images in the ResourceService.
	/// </summary>
	public static class PresentationResourceService
	{
		static readonly Dictionary<string, BitmapSource> bitmapCache = new Dictionary<string, BitmapSource>();
		static readonly IResourceService resource_service;
		
		static PresentationResourceService()
		{
			resource_service = ServiceSingleton.GetRequiredService<IResourceService>();
			resource_service.LanguageChanged += OnLanguageChanged;
		}
		
		static void OnLanguageChanged(object sender, EventArgs e)
		{
			lock (bitmapCache) {
				bitmapCache.Clear();
			}
		}
		
		/// <summary>
		/// Creates a new System.Windows.Controls.Image object containing the image with the
		/// specified resource name.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		[Obsolete("Use SD.ResourceService.GetImage(name).CreateImage() instead, or just create the image manually")]
		public static System.Windows.Controls.Image GetImage(string name)
		{
			return new System.Windows.Controls.Image {
				Source = GetBitmapSource(name)
			};
		}
		
		/// <summary>
		/// Returns a BitmapSource from the resource database, it handles localization
		/// transparent for the user.
		/// </summary>
		/// <param name="name">
		/// The name of the requested bitmap.
		/// </param>
		/// <exception cref="ResourceNotFoundException">
		/// Is thrown when the GlobalResource manager can't find a requested resource.
		/// </exception>
		public static BitmapSource GetBitmapSource(string name)
		{
			if(resource_service == null)
				throw new ArgumentNullException("resource_service");
			
			lock(bitmapCache){
				BitmapSource bs;
				if(bitmapCache.TryGetValue(name, out bs))
					return bs;
				
				System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)resource_service.GetImageResource(name);
				if(bmp == null)
					throw new ResourceNotFoundException(name);
				
				IntPtr hBitmap = bmp.GetHbitmap();
				try {
					bs = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
					                                           Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					bs.Freeze();
					bitmapCache[name] = bs;
				} finally {
					NativeMethods.DeleteObject(hBitmap);
				}
				return bs;
			}
		}
	}
}