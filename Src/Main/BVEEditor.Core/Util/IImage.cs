/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/19
 * Time: 15:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Media;

//using BVEEditor.WinForms;
using Core.Presentation;
using ICSharpCode.Core;

namespace BVEEditor
{
	/// <summary>
	/// Represents an image.
	/// </summary>
	public interface IImage
	{
		/// <summary>
		/// Gets the image as WPF ImageSource.
		/// </summary>
		ImageSource ImageSource { get; }
		
		/// <summary>
		/// Gets the image as System.Drawing.Bitmap.
		/// </summary>
		Bitmap Bitmap { get; }
		
		/// <summary>
		/// Gets the image as System.Drawing.Icon.
		/// </summary>
		Icon Icon { get; }
	}
	
	/// <summary>
	/// Represents an image that gets loaded from a ResourceService.
	/// </summary>
	public class ResourceServiceImage : IImage
	{
		readonly string resourceName;
        readonly IResourceService resource_service;
		
		/// <summary>
		/// Creates a new ResourceServiceImage.
		/// </summary>
		/// <param name="resourceName">The name of the image resource.</param>
		[Obsolete("Use SD.ResourceService.GetImage() instead")]
		public ResourceServiceImage(string resourceName)
		{
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			this.resourceName = resourceName;
		}
		
		internal ResourceServiceImage(IResourceService resourceService, string resourceName)
		{
			this.resourceName = resourceName;
            resource_service = resourceService;
		}
		
		/// <inheritdoc/>
		public ImageSource ImageSource {
			get {
				return PresentationResourceService.GetBitmapSource(resourceName);
			}
		}
		
		/// <inheritdoc/>
		public Bitmap Bitmap {
			get {
				return resource_service.GetBitmap(resourceName);
			}
		}
		
		/// <inheritdoc/>
		public Icon Icon {
			get {
				return resource_service.GetIcon(resourceName);
			}
		}
		
		public override bool Equals(object obj)
		{
			ResourceServiceImage other = obj as ResourceServiceImage;
			if (other == null)
				return false;
			return this.resourceName == other.resourceName;
		}
		
		public override int GetHashCode()
		{
			return resourceName.GetHashCode();
		}
		
		public override string ToString()
		{
			return string.Format("[ResourceServiceImage {0}]", resourceName);
		}
	}
}
