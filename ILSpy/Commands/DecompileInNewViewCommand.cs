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
using System.Collections.Generic;
using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

using TomsToolbox.Essentials;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportContextMenuEntry(Header = nameof(Resources.DecompileToNewPanel), InputGestureText = "MMB", Icon = "images/Search", Category = nameof(Resources.Analyze), Order = 90)]
	[Shared]
	internal sealed class DecompileInNewViewCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes != null || context.Reference?.Reference is IEntity;
		}

		public bool IsEnabled(TextViewContext context)
		{
			return GetNodes(context).Any();
		}

		public void Execute(TextViewContext context)
		{
			DecompileNodes(GetNodes(context).ToArray());
		}

		IEnumerable<ILSpyTreeNode> GetNodes(TextViewContext context)
		{
			if (context.SelectedTreeNodes != null)
			{
				if (context.TreeView.DataContext != assemblyTreeModel)
				{
					return context.SelectedTreeNodes.OfType<IMemberTreeNode>().Select(FindTreeNode).ExceptNullItems();
				}
				else
				{
					return context.SelectedTreeNodes.OfType<ILSpyTreeNode>();
				}
			}
			else if (context.Reference?.Reference is IEntity entity)
			{
				if (assemblyTreeModel.FindTreeNode(entity) is { } node)
				{
					return new[] { node };
				}
			}
			return Array.Empty<ILSpyTreeNode>();

			ILSpyTreeNode FindTreeNode(IMemberTreeNode node)
			{
				if (node is ILSpyTreeNode ilspyNode)
					return ilspyNode;
				return assemblyTreeModel.FindTreeNode(node.Member);
			}
		}

		void DecompileNodes(ILSpyTreeNode[] nodes)
		{
			if (nodes.Length == 0)
				return;

			dockWorkspace.ActiveTabPage = dockWorkspace.AddTabPage();

			if (assemblyTreeModel.SelectedItems.SequenceEqual(nodes))
				assemblyTreeModel.DecompileSelectedNodes();
			else
				assemblyTreeModel.SelectNodes(nodes);
		}
	}
}
