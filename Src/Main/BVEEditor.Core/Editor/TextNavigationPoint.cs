/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 07/01/2013
 * Time: 00:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using BVEEditor.Events;
using Caliburn.Micro;

namespace BVEEditor.Editor
{
	/// <summary>
	/// Description of TextNavigationPoint.
	/// </summary>
	public class TextNavigationPoint : DefaultNavigationPoint
	{
		const int Threshold = 5;
		
		#region constructor
		public TextNavigationPoint(IEventAggregator eventAggregator) : this(eventAggregator, String.Empty, 0, 0) {}
		public TextNavigationPoint(IEventAggregator eventAggregator, string fileName) : this(eventAggregator, fileName, 0, 0) {}
		public TextNavigationPoint(IEventAggregator eventAggregator, string fileName, int lineNumber, int column)
            : this(eventAggregator, fileName, lineNumber, column, String.Empty) {}
		public TextNavigationPoint(IEventAggregator eventAggregator, string fileName, int lineNumber, int column, string content)
            : base(eventAggregator, fileName, new Point(column, lineNumber))
		{
			if(String.IsNullOrEmpty(content)){
				this.content = String.Empty;
				return;
			}
			this.content = content.Trim();
		}
		#endregion
		
		// TODO: Navigation - eventually, we'll store a reference to the document
		//       itself so we can track filename changes, inserts (that affect
		//       line numbers), and dynamically retrieve the text at this.lineNumber
		//
		//       what happens to the doc reference when the document is closed?
		//
		string content;
		
		public int LineNumber {
			get {return ((Point)this.NavigationData).Y;}
		}
		
		public int Column {
			get {return ((Point)this.NavigationData).X;}
		}
		
		public override void JumpTo()
		{
			event_aggregator.Publish(new JumpLocationEvent(ICSharpCode.Core.FileName.Create(this.FileName),
			                               this.LineNumber,
			                               this.Column));
		}
		
		public override void ContentChanging(object sender, EventArgs e)
		{
			// TODO: Navigation - finish ContentChanging
//			if (e is DocumentEventArgs) {
//				DocumentEventArgs de = (DocumentEventArgs)e;
//				if (this.LineNumber >= 
//			}
		}
		
		#region IComparable
		public override int CompareTo(object obj)
		{
			int result = base.CompareTo(obj);
			if (0!=result) {
				return result;
			}
			TextNavigationPoint pt = obj as TextNavigationPoint;
			if (this.LineNumber == pt.LineNumber) {
				return 0;
			} else if (this.LineNumber > pt.LineNumber) {
				return 1;
			} else {
				return -1;
			}
		}
		#endregion
		
		#region Equality
		public override bool Equals(object obj)
		{
			TextNavigationPoint pt = obj as TextNavigationPoint;
			if(pt == null) return false;
			return this.FileName.Equals(pt.FileName)
				&& (Math.Abs(this.LineNumber - pt.LineNumber) <= Threshold);
		}
		
		public override int GetHashCode()
		{
			return this.FileName.GetHashCode() ^ this.LineNumber.GetHashCode();
		}
		#endregion
		
		public override string Description {
			get {
				return String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                     "{0}: {1}",
				                     this.LineNumber,
				                     this.content);
			}
		}
		
		public override string FullDescription {
			get {
				return String.Format(System.Globalization.CultureInfo.CurrentCulture,
				                     "{0} - {1}",
				                     Path.GetFileName(this.FileName),
				                     this.Description);
			}
		}
	}
}
