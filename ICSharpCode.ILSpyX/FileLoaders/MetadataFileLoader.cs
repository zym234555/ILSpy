// Copyright (c) 2024 Siegfried Pammer
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
using System.Reflection.Metadata;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

using static ICSharpCode.Decompiler.Metadata.MetadataFile;

namespace ICSharpCode.ILSpyX.FileLoaders
{
	public sealed class MetadataFileLoader : IFileLoader
	{
		public Task<LoadResult?> Load(string fileName, Stream stream, FileLoadContext settings)
		{
			try
			{
				var kind = Path.GetExtension(fileName).Equals(".pdb", StringComparison.OrdinalIgnoreCase)
					? MetadataFileKind.ProgramDebugDatabase : MetadataFileKind.Metadata;
				var metadata = MetadataReaderProvider.FromMetadataStream(stream, MetadataStreamOptions.PrefetchMetadata | MetadataStreamOptions.LeaveOpen);
				var metadataFile = new MetadataFile(kind, fileName, metadata);
				return Task.FromResult<LoadResult?>(new LoadResult { MetadataFile = metadataFile });
			}
			catch (BadImageFormatException)
			{
				return Task.FromResult<LoadResult?>(null);
			}
		}
	}
}
