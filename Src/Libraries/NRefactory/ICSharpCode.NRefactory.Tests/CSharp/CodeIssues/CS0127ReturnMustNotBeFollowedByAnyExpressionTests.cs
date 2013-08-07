﻿//
// CS0127ReturnMustNotBeFollowedByAnyExpressionTests.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
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
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.CodeActions;

namespace ICSharpCode.NRefactory.CSharp.CodeIssues
{
	[TestFixture]
	public class CS0127ReturnMustNotBeFollowedByAnyExpressionTests : InspectionActionTestBase
	{
		[Test]
		public void TestSimpleCase ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		return str;
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
			CheckFix (context, issues, @"class Foo
{
	void Bar (string str)
	{
		return ;
	}
}");
		}
		
		[Test]
		public void TestSimpleInvalidCase ()
		{
			var input = @"class Foo
{
	string Bar (string str)
	{
		return str;
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestProperty ()
		{
			var input = @"class Foo {
	string Bar 
	{
		get {
			return ""Hello World "";
		}
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestIndexer ()
		{
			var input = @"class Foo {
	string this [int idx]
	{
		get {
			return ""Hello World "";
		}
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestAnonymousMethod ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		System.Func<string> func = delegate {
			return str;
		};
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestLambdaMethod ()
		{
			var input = @"class Foo
{
	void Bar (string str)
	{
		System.Func<string> func = () => {
			return str;
		};
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}
		
		[Test]
		public void TestOperatorFalsePositives ()
		{
			var input = @"class Foo
{
	public static bool operator == (Foo left, Foo right)
	{
		return true;
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (0, issues.Count);
		}

		[Test]
		public void TestConstructor ()
		{
			var input = @"class Foo
{
	Foo ()
	{
		return 1;
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}
		
		[Test]
		public void TestDestructor ()
		{
			var input = @"class Foo
{
	~Foo ()
	{
		return 1;
	}
}";
			
			TestRefactoringContext context;
			var issues = GetIssues (new CS0127ReturnMustNotBeFollowedByAnyExpression (), input, out context);
			Assert.AreEqual (1, issues.Count);
		}
	}
}

