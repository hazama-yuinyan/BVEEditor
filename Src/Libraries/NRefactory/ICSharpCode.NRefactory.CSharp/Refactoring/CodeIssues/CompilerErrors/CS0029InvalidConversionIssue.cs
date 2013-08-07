// 
// CS0029InvalidConversionIssue.cs
// 
// Author:
//      Daniel Grunwald <daniel@danielgrunwald.de>
// 
// Copyright (c) 2013 Daniel Grunwald <daniel@danielgrunwald.de>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Refactoring;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[IssueDescription("CS0029: Cannot implicitly convert type 'A' to 'B'.",
	                  Description = "This error occurs when trying to assign a value of an incompatible type.",
	                  Category = IssueCategories.CompilerErrors,
	                  Severity = Severity.Error,
	                  IssueMarker = IssueMarker.Underline)]
	public class CS0029InvalidConversionIssue : ICodeIssueProvider
	{
		// This class handles both
		// CS0029: Cannot implicitly convert type 'type' to 'type'
		// and
		// CS0266: Cannot implicitly convert type 'type1' to 'type2'. An explicit conversion exists (are you missing a cast?)
		
		public IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context)
		{
			return new GatherVisitor(context).GetIssues();
		}

		class GatherVisitor : GatherVisitorBase<CS0029InvalidConversionIssue>
		{
			readonly CSharpConversions conversion;

			public GatherVisitor(BaseRefactoringContext ctx)
				: base(ctx)
			{
				conversion = new CSharpConversions(ctx.Compilation);
			}

			// Currently, we only checks assignments
			public override void VisitAssignmentExpression(AssignmentExpression assignmentExpression)
			{
				base.VisitAssignmentExpression(assignmentExpression);
				if (assignmentExpression.Operator != AssignmentOperatorType.Assign)
					return;
				var variableType = ctx.Resolve(assignmentExpression.Left).Type;
				CheckConversion(variableType, assignmentExpression.Right);
			}
			
			public override void VisitVariableInitializer(VariableInitializer variableInitializer)
			{
				base.VisitVariableInitializer(variableInitializer);
				if (!variableInitializer.Initializer.IsNull) {
					var variableType = ctx.Resolve(variableInitializer).Type;
					CheckConversion(variableType, variableInitializer.Initializer);
				}
			}

			public override void VisitReturnStatement(ReturnStatement returnStatement)
			{
				base.VisitReturnStatement(returnStatement);
				CheckConversion(ctx.GetExpectedType (returnStatement.Expression), returnStatement.Expression);			
			}

			public override void VisitInvocationExpression(InvocationExpression invocationExpression)
			{
				base.VisitInvocationExpression(invocationExpression);
				foreach (var expr in invocationExpression.Arguments) {
					CheckConversion(ctx.GetExpectedType(expr), expr);		
				}
			}

			public override void VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression)
			{
				base.VisitArrayInitializerExpression(arrayInitializerExpression);
				foreach (var expr in arrayInitializerExpression.Elements) {
					CheckConversion(ctx.GetExpectedType(expr), expr);		
				}
			}
			
			void CheckConversion(IType variableType, Expression expression)
			{
				Console.WriteLine(variableType);
				if (variableType.Kind == TypeKind.Unknown)
					return; // ignore error if the variable type is unknown
				if (ctx.GetConversion(expression).IsValid)
					return; // don't complain if the code is valid
				
				var rr = ctx.Resolve(expression);
				if (rr.Type.Kind == TypeKind.Unknown)
					return; // ignore error if expression type is unknown
				
				var foundConversion = conversion.ExplicitConversion(rr, variableType);
				
				var builder = ctx.CreateTypeSystemAstBuilder(expression);
				AstType variableTypeNode = builder.ConvertType(variableType);
				AstType expressionTypeNode = builder.ConvertType(rr.Type);
				
				if (foundConversion.IsValid) {
					// CS0266: An explicit conversion exists -> suggested fix is to insert the cast
					string title = string.Format(ctx.TranslateString("Cannot implicitly convert type `{0}' to `{1}'. An explicit conversion exists (are you missing a cast?)"),
					                             expressionTypeNode, variableTypeNode);
					string fixTitle = string.Format(ctx.TranslateString("Cast to '{0}'"), variableTypeNode);
					Action<Script> fixAction = script => {
						var right = expression.Clone();
						var castRight = right.CastTo(variableTypeNode);
						script.Replace(expression, castRight);
					};
					AddIssue(expression, title, new CodeAction(fixTitle, fixAction, expression));
				} else {
					// CS0029: No explicit conversion -> Issue without suggested fix
					string title = string.Format(ctx.TranslateString("Cannot implicitly convert type `{0}' to `{1}'"),
					                             expressionTypeNode, variableTypeNode);
					AddIssue(expression, title);
				}
			}
		}
	}
}