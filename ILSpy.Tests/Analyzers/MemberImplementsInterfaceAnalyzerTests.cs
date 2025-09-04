// Copyright (c) 2020 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

using NSubstitute;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class MemberImplementsInterfaceAnalyzerTests
	{
		static readonly SymbolKind[] ValidSymbolKinds = { SymbolKind.Event, SymbolKind.Indexer, SymbolKind.Method, SymbolKind.Property };
		static readonly SymbolKind[] InvalidSymbolKinds =
			Enum.GetValues(typeof(SymbolKind)).Cast<SymbolKind>().Except(ValidSymbolKinds).ToArray();

		static readonly TypeKind[] ValidTypeKinds = { TypeKind.Class, TypeKind.Struct };
		static readonly TypeKind[] InvalidTypeKinds = Enum.GetValues(typeof(TypeKind)).Cast<TypeKind>().Except(ValidTypeKinds).ToArray();

		private ICompilation testAssembly;

		[OneTimeSetUp]
		public void Setup()
		{
			string fileName = GetType().Assembly.Location;

			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var module = new PEFile(fileName, stream, PEStreamOptions.PrefetchEntireImage, MetadataReaderOptions.None);

				testAssembly = new SimpleCompilation(module.WithOptions(TypeSystemOptions.Default), MinimalCorlib.Instance);
			}
		}

		[Test]
		public void VerifyDoesNotShowForNoSymbol()
		{
			// Arrange
			var analyzer = new MemberImplementsInterfaceAnalyzer();

			// Act
			var shouldShow = analyzer.Show(symbol: null);

			// Assert
			Assert.That(!shouldShow, $"The analyzer will be unexpectedly shown for no symbol");
		}

		[Test]
		[TestCaseSource(nameof(InvalidSymbolKinds))]
		public void VerifyDoesNotShowForNonMembers(SymbolKind symbolKind)
		{
			// Arrange
			var symbolMock = Substitute.For<ISymbol>();
			symbolMock.SymbolKind.Returns(symbolKind);
			var analyzer = new MemberImplementsInterfaceAnalyzer();

			// Act
			var shouldShow = analyzer.Show(symbolMock);

			// Assert
			Assert.That(!shouldShow, $"The analyzer will be unexpectedly shown for symbol '{symbolKind}'");
		}

		[Test]
		[TestCaseSource(nameof(ValidSymbolKinds))]
		public void VerifyDoesNotShowForStaticMembers(SymbolKind symbolKind)
		{
			// Arrange
			var memberMock = SetupMemberMock(symbolKind, TypeKind.Unknown, isStatic: true);
			var analyzer = new MemberImplementsInterfaceAnalyzer();

			// Act
			var shouldShow = analyzer.Show(memberMock);

			// Assert
			Assert.That(!shouldShow, $"The analyzer will be unexpectedly shown for static symbol '{symbolKind}'");
		}

		[Test]
		[Pairwise]
		public void VerifyDoesNotShowForUnsupportedTypes(
			[ValueSource(nameof(ValidSymbolKinds))] SymbolKind symbolKind,
			[ValueSource(nameof(InvalidTypeKinds))] TypeKind typeKind)
		{
			// Arrange
			var memberMock = SetupMemberMock(symbolKind, typeKind, isStatic: true);
			var analyzer = new MemberImplementsInterfaceAnalyzer();

			// Act
			var shouldShow = analyzer.Show(memberMock);

			// Assert
			Assert.That(!shouldShow, $"The analyzer will be unexpectedly shown for symbol '{symbolKind}' and '{typeKind}'");
		}

		[Test]
		[Pairwise]
		public void VerifyShowsForSupportedTypes(
			[ValueSource(nameof(ValidSymbolKinds))] SymbolKind symbolKind,
			[ValueSource(nameof(ValidTypeKinds))] TypeKind typeKind)
		{
			// Arrange
			var memberMock = SetupMemberMock(symbolKind, typeKind, isStatic: false);
			var analyzer = new MemberImplementsInterfaceAnalyzer();

			// Act
			var shouldShow = analyzer.Show(memberMock);

			// Assert
			Assert.That(shouldShow, $"The analyzer will not be shown for symbol '{symbolKind}' and '{typeKind}'");
		}

		[Test]
		public void VerifyReturnsOnlyInterfaceMembers()
		{
			// Arrange
			var symbol = SetupSymbolForAnalysis(typeof(TestClass), nameof(TestClass.TestMethod));
			var analyzer = new MemberImplementsInterfaceAnalyzer();

			// Act
			var results = analyzer.Analyze(symbol, new AnalyzerContext() { AssemblyList = new ILSpyX.AssemblyList(), Language = new CSharpLanguage() });

			// Assert
			Assert.That(results, Is.Not.Null);
			Assert.That(results.Count(), Is.EqualTo(1));
			var result = results.FirstOrDefault() as IMethod;
			Assert.That(result, Is.Not.Null);
			Assert.That(result.DeclaringTypeDefinition, Is.Not.Null);
			Assert.That(result.DeclaringTypeDefinition.Kind, Is.EqualTo(TypeKind.Interface));
			Assert.That(result.DeclaringTypeDefinition.Name, Is.EqualTo(nameof(ITestInterface)));
		}

		private ISymbol SetupSymbolForAnalysis(Type type, string methodName)
		{
			var typeDefinition = testAssembly.FindType(type).GetDefinition();
			return typeDefinition.Methods.First(m => m.Name == methodName);
		}

		private static IMember SetupMemberMock(SymbolKind symbolKind, TypeKind typeKind, bool isStatic)
		{
			var memberMock = Substitute.For<IMember>();
			memberMock.SymbolKind.Returns(symbolKind);
			memberMock.DeclaringTypeDefinition.Kind.Returns(typeKind);
			memberMock.IsStatic.Returns(isStatic);
			return memberMock;
		}

		private interface ITestInterface
		{
			void TestMethod();
		}

		private class BaseClass
		{
			public virtual void TestMethod() => throw new NotImplementedException();
		}

		private class TestClass : BaseClass, ITestInterface
		{
			public override void TestMethod() => throw new NotImplementedException();
		}
	}
}
