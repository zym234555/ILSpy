// Copyright (c) 2016 Daniel Grunwald
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

using System.Diagnostics;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;

namespace ICSharpCode.Decompiler
{
	[SetUpFixture]
	public class TestTraceListener : DefaultTraceListener
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Trace.Listeners.Insert(0, this);
		}

		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{
			Trace.Listeners.Remove(this);
		}

		public override void Fail(string message, string detailMessage)
		{
			Assert.Fail(message + " " + detailMessage);
		}
	}

	[SetUpFixture]
	public class ToolsetSetup
	{
		[OneTimeSetUp]
		public async Task RunBeforeAnyTests()
		{
			await Tester.Initialize().ConfigureAwait(false);
		}
	}
}
