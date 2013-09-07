// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace ICSharpCode.AvalonEdit.Search
{
	class SearchResultBackgroundRenderer : IBackgroundRenderer
	{
		TextSegmentCollection<SearchResult> currentResults = new TextSegmentCollection<SearchResult>();
		
		public TextSegmentCollection<SearchResult> CurrentResults {
			get { return currentResults; }
		}
		
		public KnownLayer Layer {
			get {
				// draw behind selection
				return KnownLayer.Selection;
			}
		}
		
		public SearchResultBackgroundRenderer()
		{
			markerBrush = Brushes.LightGreen;
			markerPen = new Pen(markerBrush, 1);
		}
		
		Brush markerBrush;
		Pen markerPen;
		
		public Brush MarkerBrush {
			get { return markerBrush; }
			set {
				this.markerBrush = value;
				markerPen = new Pen(markerBrush, 1);
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if(textView == null)
				throw new ArgumentNullException("textView");
			if(drawingContext == null)
				throw new ArgumentNullException("drawingContext");
			
			if(currentResults == null || !textView.VisualLinesValid)
				return;
			
			var visual_lines = textView.VisualLines;
			if(visual_lines.Count == 0)
				return;
			
			int view_start = visual_lines.First().FirstDocumentLine.Offset;
			int view_end = visual_lines.Last().LastDocumentLine.EndOffset;
			
			foreach(SearchResult result in currentResults.FindOverlappingSegments(view_start, view_end - view_start)){
				BackgroundGeometryBuilder geo_builder = new BackgroundGeometryBuilder();
				geo_builder.AlignToMiddleOfPixels = true;
				geo_builder.CornerRadius = 3;
				geo_builder.AddSegment(textView, result);
				Geometry geometry = geo_builder.CreateGeometry();
				if(geometry != null)
					drawingContext.DrawGeometry(markerBrush, markerPen, geometry);
			}
		}
	}
}
