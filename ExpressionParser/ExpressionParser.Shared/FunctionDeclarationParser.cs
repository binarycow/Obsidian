using ExpressionParser.Operators;
using ExpressionParser.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace ExpressionParser
{
    public static class FunctionDeclarationParser
    {

        public static bool TryParseFunctionDeclaration(ASTNode node, [NotNullWhen(true)]out FunctionDeclaration? declaration)
        {
            declaration = null;
            if (!(node is BinaryASTNode binaryNode)) return false;
            if (!(binaryNode.Left is IdentifierNode identifierNode)) return false;
            if (!(binaryNode.Operator is SpecialOperator @operator)) return false;
            if (@operator.OperatorType != Configuration.SpecialOperatorType.MethodCall) return false;
            if (!(binaryNode.Right is ArgumentSetNode argumentSet)) return false;
            return TryParseFunctionDeclaration(identifierNode, argumentSet, out declaration);
        }

        private static bool TryParseFunctionDeclaration(IdentifierNode identifier, ArgumentSetNode argumentSet, [NotNullWhen(true)]out FunctionDeclaration? declaration)
        {
            declaration = default;
            var functionName = identifier.TextValue;
            var arguments = argumentSet.Arguments.Select(ParseParameterDeclaration).ToArray();

            // Check for a required parameter that exists after an optional parameter
            var firstOptionalIndex = int.MaxValue;
            for(int argIndex = 0; argIndex < arguments.Length; ++argIndex)
            {
                var arg = arguments[argIndex] ?? throw new NotImplementedException();
                if(arg.Optional)
                {
                    firstOptionalIndex = Math.Min(firstOptionalIndex, argIndex);
                }
                else if(firstOptionalIndex != int.MaxValue)
                {
                    // This is *NOT* optional, yet exists after an optional parameter.
                    throw new NotImplementedException(); 
                }
            }

            ParameterDeclaration[] nonNullArguments = arguments.Where(arg => arg != null).Select(arg => arg).ToArray()!;
            if (nonNullArguments.Length != arguments.Length) return false;
            declaration = new FunctionDeclaration(returnType: typeof(object), functionName, nonNullArguments);
            return true;
        }

        private static ParameterDeclaration? ParseParameterDeclaration(ASTNode argument)
        {
            switch(argument)
            {
                case IdentifierNode identifierNode:
                    return new ParameterDeclaration(identifierNode.TextValue);
                case BinaryASTNode binaryNode:
                    return ParseArgumentDefinition(binaryNode);
                default:
                    throw new NotImplementedException();
            }
        }

        private static ParameterDeclaration? ParseArgumentDefinition(BinaryASTNode argument)
        {
            if (!(argument.Left is IdentifierNode nameNode)) throw new NotImplementedException();
            if (!(argument.Operator is StandardOperator standardOperator)) throw new NotImplementedException();
            if (standardOperator.OperatorType != Configuration.OperatorType.Assign) throw new NotImplementedException();
            if (!(argument.Right is LiteralNode valueNode)) throw new NotImplementedException();

            return new ParameterDeclaration(nameNode.TextValue, valueNode.Value);
        }
    }
}
