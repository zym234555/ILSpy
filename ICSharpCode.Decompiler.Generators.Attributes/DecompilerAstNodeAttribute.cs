﻿using System;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public sealed class DecompilerAstNodeAttribute : Attribute
	{
		public DecompilerAstNodeAttribute(bool hasNullNode) { }
	}

	public sealed class ExcludeFromMatchAttribute : Attribute
	{
	}
}