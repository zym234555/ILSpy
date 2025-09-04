using System.Collections.Generic;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	class RemoveCompilerAttribute : DepthFirstAstVisitor, IAstTransform
	{
		public override void VisitAttribute(CSharp.Syntax.Attribute attribute)
		{
			var section = (AttributeSection)attribute.Parent;
			SimpleType type = attribute.Type as SimpleType;
			if (section.AttributeTarget == "assembly" &&
				(type.Identifier == "CompilationRelaxations" || type.Identifier == "RuntimeCompatibility" || type.Identifier == "SecurityPermission" || type.Identifier == "PermissionSet" || type.Identifier == "AssemblyVersion" || type.Identifier == "Debuggable" || type.Identifier == "TargetFramework"))
			{
				attribute.Remove();
				if (section.Attributes.Count == 0)
					section.Remove();
			}
			if (section.AttributeTarget == "module" && type.Identifier is "UnverifiableCode" or "RefSafetyRules")
			{
				attribute.Remove();
				if (section.Attributes.Count == 0)
					section.Remove();
			}
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			rootNode.AcceptVisitor(this);
		}
	}

	public class RemoveNamespaceMy : DepthFirstAstVisitor, IAstTransform
	{
		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			if (namespaceDeclaration.Name == "My")
			{
				namespaceDeclaration.Remove();
			}
			else
			{
				base.VisitNamespaceDeclaration(namespaceDeclaration);
			}
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			rootNode.AcceptVisitor(this);
		}
	}
}
