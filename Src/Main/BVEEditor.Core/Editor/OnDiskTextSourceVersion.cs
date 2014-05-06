/*
 * Created by SharpDevelop.
 * User: HAZAMA_Akkarin
 * Date: 2013/06/27
 * Time: 16:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;

namespace BVEEditor.Editor
{
	/// <summary>
	/// Signifies that the text source matches the version of the file on disk.
	/// </summary>
	public sealed class OnDiskTextSourceVersion : ITextSourceVersion
	{
		public readonly DateTime LastWriteTime;
		
		public OnDiskTextSourceVersion(DateTime lastWriteTime)
		{
			this.LastWriteTime = lastWriteTime;
		}
		
		public bool BelongsToSameDocumentAs(ITextSourceVersion other)
		{
			return this == other;
		}
		
		public int CompareAge(ITextSourceVersion other)
		{
			if (this != other)
				throw new ArgumentException("other belongs to different document");
			return 0;
		}
		
		public IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other)
		{
			if (this != other)
				throw new ArgumentException("other belongs to different document");
			return EmptyList<TextChangeEventArgs>.Instance;
		}
		
		public int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement)
		{
			if (this != other)
				throw new ArgumentException("other belongs to different document");
			return oldOffset;
		}
	}
}
