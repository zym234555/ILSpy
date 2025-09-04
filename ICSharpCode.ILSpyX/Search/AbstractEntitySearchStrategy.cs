// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Concurrent;
using System.IO;

namespace ICSharpCode.ILSpyX.Search
{
	using ICSharpCode.Decompiler.TypeSystem;
	using ICSharpCode.ILSpyX.Abstractions;

	public abstract class AbstractEntitySearchStrategy : AbstractSearchStrategy
	{
		protected readonly ILanguage language;
		protected readonly ApiVisibility apiVisibility;

		protected AbstractEntitySearchStrategy(ILanguage language, ApiVisibility apiVisibility,
			SearchRequest searchRequest, IProducerConsumerCollection<SearchResult> resultQueue)
			: base(searchRequest, resultQueue)
		{
			this.language = language;
			this.apiVisibility = apiVisibility;
		}

		protected bool CheckVisibility(IEntity? entity)
		{
			if (apiVisibility == ApiVisibility.All)
				return true;

			while (entity != null)
			{
				if (apiVisibility == ApiVisibility.PublicOnly)
				{
					if (!(entity.Accessibility == Accessibility.Public ||
						entity.Accessibility == Accessibility.Protected ||
						entity.Accessibility == Accessibility.ProtectedOrInternal))
						return false;
				}
				else if (apiVisibility == ApiVisibility.PublicAndInternal)
				{
					if (!language.ShowMember(entity))
						return false;
				}
				entity = entity.DeclaringTypeDefinition;
			}

			return true;
		}

		protected bool IsInNamespaceOrAssembly(IEntity entity)
		{
			if (searchRequest.InAssembly != null)
			{
				if (entity.ParentModule?.MetadataFile == null ||
					!(Path.GetFileName(entity.ParentModule.MetadataFile.FileName).Contains(searchRequest.InAssembly, StringComparison.OrdinalIgnoreCase)
					|| entity.ParentModule.FullAssemblyName.Contains(searchRequest.InAssembly, StringComparison.OrdinalIgnoreCase)))
				{
					return false;
				}
			}

			if (searchRequest.InNamespace != null)
			{
				if (searchRequest.InNamespace.Length == 0)
				{
					return entity.Namespace.Length == 0;
				}
				else if (!entity.Namespace.Contains(searchRequest.InNamespace, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}

			return true;
		}

		protected void OnFoundResult(IEntity entity)
		{
			OnFoundResult(searchRequest.SearchResultFactory.Create(entity));
		}
	}
}
