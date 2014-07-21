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
	public class SearchResultBackgroundRenderer : IBackgroundRenderer
	{
		public TextSegmentCollection<SearchResult> CurrentResults {
            get; set;
		}
		
		public KnownLayer Layer {
			get {
				// draw behind selection
				return KnownLayer.Selection;
			}
		}
		
		public SearchResultBackgroundRenderer()
		{
			marker_brush = Brushes.LightGreen;
			marker_pen = new Pen(marker_brush, 1);
		}
		
		Brush marker_brush;
		Pen marker_pen;
		
		public Brush MarkerBrush {
			get { return marker_brush; }
			set {
				this.marker_brush = value;
				marker_pen = new Pen(marker_brush, 1);
			}
		}
		
		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if(textView == null)
				throw new ArgumentNullException("textView");
			if(drawingContext == null)
				throw new ArgumentNullException("drawingContext");
			
			if(CurrentResults == null || !textView.VisualLinesValid)
				return;
			
			var visual_lines = textView.VisualLines;
			if(visual_lines.Count == 0)
				return;
			
			int view_start = visual_lines.First().FirstDocumentLine.Offset;
			int view_end = visual_lines.Last().LastDocumentLine.EndOffset;
			
			foreach(SearchResult result in CurrentResults.FindOverlappingSegments(view_start, view_end - view_start)){
				BackgroundGeometryBuilder geo_builder = new BackgroundGeometryBuilder();
				geo_builder.AlignToMiddleOfPixels = true;
				geo_builder.CornerRadius = 3;
				geo_builder.AddSegment(textView, result);
				Geometry geometry = geo_builder.CreateGeometry();
				if(geometry != null)
					drawingContext.DrawGeometry(marker_brush, marker_pen, geometry);
			}
		}
	}
}
