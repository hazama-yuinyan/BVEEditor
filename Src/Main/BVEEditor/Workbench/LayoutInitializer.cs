/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/07/25
 * Time: 12:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Xceed.Wpf.AvalonDock.Layout;

namespace BVEEditor.Workbench
{
	/// <summary>
	/// Description of LayoutInitializer.
	/// </summary>
	public class LayoutInitializer : ILayoutUpdateStrategy
	{
        public LayoutInitializer()
        {}

		public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
		{
			throw new NotImplementedException();
		}
		
		public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
		{
			throw new NotImplementedException();
		}
		
		public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
		{
			throw new NotImplementedException();
		}
		
		public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
		{
			throw new NotImplementedException();
		}
	}
}
