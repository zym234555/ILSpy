// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents a class, enum, interface, struct, delegate, record or VB module.
	/// For partial classes, this represents the whole class.
	/// </summary>
	public interface ITypeDefinition : ITypeDefinitionOrUnknown, IType, IEntity
	{
		ExtensionInfo? ExtensionInfo { get; }
		IReadOnlyList<ITypeDefinition> NestedTypes { get; }
		IReadOnlyList<IMember> Members { get; }

		IEnumerable<IField> Fields { get; }
		IEnumerable<IMethod> Methods { get; }
		IEnumerable<IProperty> Properties { get; }
		IEnumerable<IEvent> Events { get; }

		/// <summary>
		/// Gets the known type code for this type definition.
		/// </summary>
		KnownTypeCode KnownTypeCode { get; }

		/// <summary>
		/// For enums: returns the underlying primitive type.
		/// For all other types: returns <see langword="null"/>.
		/// </summary>
		IType? EnumUnderlyingType { get; }

		/// <summary>
		/// For structs: returns whether this is a readonly struct.
		/// For all other types: returns false.
		/// </summary>
		bool IsReadOnly { get; }

		/// <summary>
		/// Gets the short type name as stored in metadata.
		/// That is, the short type name including the generic arity (`N) appended.
		/// </summary>
		/// <remarks>
		/// "Int32" for int
		/// "List`1" for List&lt;T&gt;
		/// "List`1" for List&lt;string&gt;
		/// </remarks>
		string MetadataName { get; }

		/// <summary>
		/// Gets/Sets the declaring type (incl. type arguments, if any).
		/// This property will return null for top-level types.
		/// </summary>
		new IType? DeclaringType { get; } // solves ambiguity between IType.DeclaringType and IEntity.DeclaringType

		/// <summary>
		/// Gets whether this type contains extension methods or C# 14 extensions.
		/// </summary>
		/// <remarks>This property is used to speed up the search for extension members.</remarks>
		bool HasExtensions { get; }

		/// <summary>
		/// The nullability specified in the [NullableContext] attribute on the type.
		/// This serves as default nullability for members of the type that do not have a [Nullable] attribute.
		/// </summary>
		Nullability NullableContext { get; }

		/// <summary>
		/// Gets whether the type has the necessary members to be considered a C# 9 record or C# 10 record struct type.
		/// </summary>
		bool IsRecord { get; }
	}
}
