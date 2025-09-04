// Copyright (c) 2018 Siegfried Pammer
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

using System.Collections.Generic;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Analyzers
{
	/// <summary>
	/// Base interface for all analyzers. You can register an analyzer for any <see cref="ISymbol"/> by implementing
	/// this interface and adding an <see cref="ExportAnalyzerAttribute"/>.
	/// </summary>
	public interface IAnalyzer
	{
		/// <summary>
		/// Returns true, if the analyzer should be shown for a symbol, otherwise false.
		/// </summary>
		bool Show(ISymbol symbol);

		/// <summary>
		/// Returns all symbols found by this analyzer.
		/// </summary>
		IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context);
	}

	public interface IAnalyzerMetadata
	{
		string Header { get; }

		int Order { get; }
	}
}
