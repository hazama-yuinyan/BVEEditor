/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/20
 * Time: 13:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace BVEEditor.Editor.LanguageBinding
{
	/// <summary>
	/// Provides access to language specific features (independent of files).
	/// </summary>
	public interface ILanguageBinding : IServiceProvider
	{
		/// <summary>
		/// Provides access to the formatting strategy for this language.
		/// </summary>
		IFormattingStrategy FormattingStrategy { 
			get;
		}
		
		/// <summary>
		/// Provides access to the bracket search logic for this language.
		/// </summary>
		IBracketSearcher BracketSearcher {
			get;
		}
		
		/// <summary>
		/// Provides access to the code generator for this language.
		/// </summary>
		/*CodeGenerator CodeGenerator {
			get;
		}
		
		/// <summary>
		/// Provides access to the <see cref="System.CodeDom.Compiler.CodeDomProvider" /> for this language.
		/// Can be null, if not available.
		/// </summary>
		System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get;
		}*/
	}
}
