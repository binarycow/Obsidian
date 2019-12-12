using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionParser.References
{
    internal class TestMethodGroup : MethodGroup
    {
        internal TestMethodGroup(UserDefinedTest testDefinition) : base(testDefinition.Declaration.Name)
        {
            TestDefinition = testDefinition;
        }

        internal UserDefinedTest TestDefinition { get; }

    }
}
