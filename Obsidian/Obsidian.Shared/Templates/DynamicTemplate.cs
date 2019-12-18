using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionParser.Scopes;
using Obsidian.AST;
using Obsidian.AST.Nodes;
using Obsidian.AST.Nodes.Statements;
using Obsidian.Transforming;

namespace Obsidian.Templates
{
    internal class DynamicTemplate : DynamicObject, ITemplate
    {

        private DynamicTemplate(JinjaEnvironment environment, TemplateNode templateNode, string? templateName, string? templatePath)
        {
            TemplateNode = templateNode;
            Environment = environment;
            TemplateName = templateName;
            TemplatePath = templatePath;
            Macros = NodeFinderVisitor.FindNodes<MacroNode>(templateNode).ToArrayWithoutInstantiation();

        }
        public JinjaEnvironment Environment { get; }

        public string? TemplateName { get; }

        public string? TemplatePath { get; }
        internal TemplateNode TemplateNode { get; }

        internal IEnumerable<MacroNode> Macros { get; }

        public string Render()
        {
            return Render(new Dictionary<string, object?>());
        }
        public string Render(IDictionary<string, object?> variables)
        {
            var renderer = new StringBuilderTransformer(Environment, variables);
            TemplateNode.Transform(renderer);
            return renderer.StringBuilder.ToString();
        }

        internal static DynamicTemplate LoadTemplate(JinjaEnvironment environment, string templateText, string? templateName, string? templatePath)
        {
            var node = ASTNode.GetTemplateNode(environment, templateText);
            if(node is TemplateNode templateNode)
            {
                return new DynamicTemplate(environment, templateNode, templateName, templatePath);
            }
            throw new NotImplementedException();
        }

        public bool Validate(IDictionary<string, object?> variables)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return base.GetDynamicMemberNames();
        }
        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }
        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }
        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return base.TryDeleteIndex(binder, indexes);
        }
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return base.TryDeleteMember(binder);
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return base.TryGetIndex(binder, indexes, out result);
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var macro = Macros.FirstOrDefault(mac => mac.FunctionDeclaration.Name == binder.Name);
            if(macro != null)
            {
                result = macro;
                return true;
            }
            return base.TryGetMember(binder, out result);
        }
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return base.TryInvokeMember(binder, args, out result);
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return base.TrySetIndex(binder, indexes, value);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return base.TrySetMember(binder, value);
        }
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }

    }
}
