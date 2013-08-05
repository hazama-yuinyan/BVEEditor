/*
 * Created by SharpDevelop.
 * User: HAZAMA
 * Date: 2013/06/24
 * Time: 13:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel.Design;

namespace BVEEditor.Editor.LanguageBinding
{
	public class DefaultLanguageBinding : ILanguageBinding
	{
		public static readonly DefaultLanguageBinding DefaultInstance = new DefaultLanguageBinding(true);
		
		protected readonly ServiceContainer container;
		
		DefaultLanguageBinding(bool isDefault)
		{
			if (isDefault) {
				this.container = new ServiceContainer();
				this.container.AddService(typeof(IFormattingStrategy), DefaultFormattingStrategy.DefaultInstance);
				this.container.AddService(typeof(IBracketSearcher), DefaultBracketSearcher.DefaultInstance);
				//this.container.AddService(typeof(CodeGenerator), CodeGenerator.DummyCodeGenerator);
			} else {
				this.container = new ServiceContainer(DefaultInstance);
			}
		}
		
		public DefaultLanguageBinding()
			: this(false)
		{
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				return this.GetService<IFormattingStrategy>();
			}
		}
		
		public IBracketSearcher BracketSearcher {
			get {
				return this.GetService<IBracketSearcher>();
			}
		}
		
		/*public CodeGenerator CodeGenerator {
			get {
				return this.GetService<CodeGenerator>();
			}
		}
		
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				return this.GetService<System.CodeDom.Compiler.CodeDomProvider>();
			}
		}*/
		
		public object GetService(Type serviceType)
		{
			return container.GetService(serviceType);
		}
	}
}
