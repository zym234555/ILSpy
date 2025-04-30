using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.ViewModels
{
	using System.Linq;

	using ICSharpCode.Decompiler;
	using ICSharpCode.Decompiler.TypeSystem;
	using ICSharpCode.ILSpy.TreeNodes;

	class CompareViewModel : ObservableObject
	{
		private readonly AssemblyTreeModel assemblyTreeModel;
		private LoadedAssembly leftAssembly;
		private LoadedAssembly rightAssembly;
		private LoadedAssembly[] assemblies;
		private ComparisonEntryTreeNode root;

		public CompareViewModel(AssemblyTreeModel assemblyTreeModel, LoadedAssembly left, LoadedAssembly right)
		{
			MessageBus<CurrentAssemblyListChangedEventArgs>.Subscribers += (sender, e) => this.Assemblies = assemblyTreeModel.AssemblyList.GetAssemblies();

			leftAssembly = left;
			rightAssembly = right;
			assemblies = assemblyTreeModel.AssemblyList.GetAssemblies();

			var leftTree = CreateEntityTree(new DecompilerTypeSystem(leftAssembly.GetMetadataFileOrNull(), leftAssembly.GetAssemblyResolver()));
			var rightTree = CreateEntityTree(new DecompilerTypeSystem(rightAssembly.GetMetadataFileOrNull(), rightAssembly.GetAssemblyResolver()));

			this.root = new ComparisonEntryTreeNode(MergeTrees(leftTree.Item2, rightTree.Item2));
		}

		public LoadedAssembly[] Assemblies {
			get => assemblies;
			set {
				if (assemblies != value)
				{
					assemblies = value;
					OnPropertyChanged();
				}
			}
		}

		public LoadedAssembly LeftAssembly {
			get => leftAssembly;
			set {
				if (leftAssembly != value)
				{
					leftAssembly = value;
					OnPropertyChanged();
				}
			}
		}

		public LoadedAssembly RightAssembly {
			get => rightAssembly;
			set {
				if (rightAssembly != value)
				{
					rightAssembly = value;
					OnPropertyChanged();
				}
			}
		}

		public ComparisonEntryTreeNode RootEntry {
			get => root;
			set {
				if (root != value)
				{
					root = value;
					OnPropertyChanged();
				}
			}
		}

		Entry MergeTrees(Entry a, Entry b)
		{
			var m = new Entry() {
				Entity = a.Entity,
				Signature = a.Signature,
			};

			if (a.Children?.Count > 0 && b.Children?.Count > 0)
			{
				var diff = CalculateDiff(a.Children, b.Children);
				m.Children ??= new();

				foreach (var (left, right) in diff)
				{
					if (left != null && right != null)
						m.Children.Add(MergeTrees(left, right));
					else if (left != null)
						m.Children.Add(left);
					else if (right != null)
						m.Children.Add(right);
					else
						Debug.Fail("wh00t?");
					m.Children[^1].Parent = m;
				}
			}
			else if (a.Children?.Count > 0)
			{
				m.Children ??= new();
				foreach (var child in a.Children)
				{
					child.Parent = m;
					m.Children.Add(child);
				}
			}
			else if (b.Children?.Count > 0)
			{
				m.Children ??= new();
				foreach (var child in b.Children)
				{
					child.Parent = m;
					m.Children.Add(child);
				}
			}

			return m;
		}

		(List<Entry>, Entry) CreateEntityTree(DecompilerTypeSystem typeSystem)
		{
			var module = typeSystem.MainModule;
			var metadata = module.MetadataFile.Metadata;
			var ambience = new CSharpAmbience();
			ambience.ConversionFlags = ICSharpCode.Decompiler.Output.ConversionFlags.All & ~ICSharpCode.Decompiler.Output.ConversionFlags.ShowDeclaringType;

			List<Entry> results = new();
			Dictionary<TypeDefinitionHandle, Entry> typeEntries = new();
			Dictionary<string, Entry> namespaceEntries = new(StringComparer.Ordinal);

			Entry root = new Entry { Entity = null!, Signature = module.FullAssemblyName };

			// typeEntries need a different signature: must include list of base types

			Entry? TryCreateEntry(IEntity entity)
			{
				if (entity.EffectiveAccessibility() != Accessibility.Public)
					return null;

				Entry? parent = null;

				if (entity.DeclaringTypeDefinition != null
					&& !typeEntries.TryGetValue((TypeDefinitionHandle)entity.DeclaringTypeDefinition.MetadataToken, out parent))
				{
					return null;
				}

				var entry = new Entry {
					Signature = ambience.ConvertSymbol(entity),
					Entity = entity,
					Parent = parent,
				};

				if (parent != null)
				{
					parent.Children ??= new();
					parent.Children.Add(entry);
				}

				return entry;
			}

			foreach (var typeDefHandle in metadata.TypeDefinitions)
			{
				var typeDef = module.GetDefinition(typeDefHandle);

				if (typeDef.EffectiveAccessibility() != Accessibility.Public)
					continue;

				var entry = typeEntries[typeDefHandle] = new Entry {
					Signature = ambience.ConvertSymbol(typeDef),
					Entity = typeDef
				};

				if (typeDef.DeclaringType == null)
				{
					if (!namespaceEntries.TryGetValue(typeDef.Namespace, out var nsEntry))
					{
						namespaceEntries[typeDef.Namespace] = nsEntry = new Entry { Parent = root, Signature = typeDef.Namespace, Entity = null! };
						root.Children ??= new();
						root.Children.Add(nsEntry);
					}

					entry.Parent = nsEntry;
					nsEntry.Children ??= new();
					nsEntry.Children.Add(entry);
				}
			}

			foreach (var fieldHandle in metadata.FieldDefinitions)
			{
				var fieldDef = module.GetDefinition(fieldHandle);
				var entry = TryCreateEntry(fieldDef);

				if (entry != null)
					results.Add(entry);
			}

			foreach (var eventHandle in metadata.EventDefinitions)
			{
				var eventDef = module.GetDefinition(eventHandle);
				var entry = TryCreateEntry(eventDef);

				if (entry != null)
					results.Add(entry);
			}

			foreach (var propertyHandle in metadata.PropertyDefinitions)
			{
				var propertyDef = module.GetDefinition(propertyHandle);
				var entry = TryCreateEntry(propertyDef);

				if (entry != null)
					results.Add(entry);
			}

			foreach (var methodHandle in metadata.MethodDefinitions)
			{
				var methodDef = module.GetDefinition(methodHandle);

				if (methodDef.AccessorOwner != null)
					continue;

				var entry = TryCreateEntry(methodDef);

				if (entry != null)
					results.Add(entry);
			}

			return (results, root);
		}

		List<(Entry? Left, Entry? Right)> CalculateDiff(List<Entry> left, List<Entry> right)
		{
			Dictionary<string, List<Entry>> leftMap = new();
			Dictionary<string, List<Entry>> rightMap = new();

			foreach (var item in left)
			{
				string key = item.Signature;
				if (leftMap.ContainsKey(key))
					leftMap[key].Add(item);
				else
					leftMap[key] = [item];
			}

			foreach (var item in right)
			{
				string key = item.Signature;
				if (rightMap.ContainsKey(key))
					rightMap[key].Add(item);
				else
					rightMap[key] = [item];
			}

			List<(Entry? Left, Entry? Right)> results = new();

			foreach (var (key, items) in leftMap)
			{
				if (rightMap.TryGetValue(key, out var rightEntries))
				{
					foreach (var item in items)
					{
						var other = rightEntries.Find(_ => EntryComparer.Instance.Equals(_, item));
						results.Add((item, other));
						if (other == null)
						{
							item.Kind = DiffKind.Remove;
						}
					}
				}
				else
				{
					foreach (var item in items)
					{
						item.Kind = DiffKind.Remove;
						results.Add((item, null));
					}
				}
			}

			foreach (var (key, items) in rightMap)
			{
				if (leftMap.TryGetValue(key, out var leftEntries))
				{
					foreach (var item in items)
					{
						if (!leftEntries.Any(_ => EntryComparer.Instance.Equals(_, item)))
						{
							results.Add((null, item));
							item.Kind = DiffKind.Add;
						}
					}
				}
				else
				{
					foreach (var item in items)
					{
						item.Kind = DiffKind.Add;
						results.Add((null, item));
					}
				}
			}

			return results;
		}
	}

	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public class Entry
	{
		private DiffKind kind = DiffKind.None;

		public DiffKind Kind {
			get {
				if (Children == null || Children.Count == 0)
				{
					return kind;
				}

				int addCount = 0, removeCount = 0, updateCount = 0;

				foreach (var item in Children)
				{
					switch (item.Kind)
					{
						case DiffKind.Add:
							addCount++;
							break;
						case DiffKind.Remove:
							removeCount++;
							break;
						case DiffKind.Update:
							updateCount++;
							break;
					}
				}

				if (addCount == Children.Count)
					return DiffKind.Add;
				if (removeCount == Children.Count)
					return DiffKind.Remove;
				if (addCount > 0 || removeCount > 0 || updateCount > 0)
					return DiffKind.Update;
				return DiffKind.None;
			}
			set {
				if (Children == null || Children.Count == 0)
				{
					kind = value;
				}
			}
		}
		public required string Signature { get; init; }
		public required IEntity Entity { get; init; }

		public Entry? Parent { get; set; }
		public List<Entry>? Children { get; set; }

		private string GetDebuggerDisplay()
		{
			return $"Entry{Kind}{Entity?.ToString() ?? Signature}";
		}
	}

	public class EntryComparer : IEqualityComparer<Entry>
	{
		public static EntryComparer Instance = new();

		public bool Equals(Entry? x, Entry? y)
		{
			return x?.Signature == y?.Signature;
		}

		public int GetHashCode([DisallowNull] Entry obj)
		{
			return obj.Signature.GetHashCode();
		}
	}

	public enum DiffKind
	{
		None = ' ',
		Add = '+',
		Remove = '-',
		Update = '~'
	}

	class ComparisonEntryTreeNode : ILSpyTreeNode
	{
		private readonly Entry entry;

		public ComparisonEntryTreeNode(Entry entry)
		{
			this.entry = entry;
			this.LazyLoading = entry.Children != null;
		}

		protected override void LoadChildren()
		{
			if (entry.Children == null)
				return;

			foreach (var item in entry.Children)
			{
				this.Children.Add(new ComparisonEntryTreeNode(item));
			}
		}

		public override object Text => entry.Signature;

		public override object Icon {
			get {
				switch (entry.Entity)
				{
					case ITypeDefinition t:
						return TypeTreeNode.GetIcon(t);
					case IMethod m:
						return MethodTreeNode.GetIcon(m);
					case IField f:
						return FieldTreeNode.GetIcon(f);
					case IProperty p:
						return PropertyTreeNode.GetIcon(p);
					case IEvent e:
						return EventTreeNode.GetIcon(e);
					default:
						throw new NotSupportedException();
				}
			}
		}

		public DiffKind Difference => entry.Kind;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
		}
	}
}
