using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Obsidian.TestCore
{
    public class DynamicTemplateRenderer : DynamicObject
    {
        public DynamicTemplateRenderer(ITemplate template)
        {
            Template = template;
        }

        public ITemplate Template { get; }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            args = args ?? throw new ArgumentNullException(nameof(args));
            binder = binder ?? throw new ArgumentNullException(nameof(binder));
            if (binder.Name != nameof(ITemplate.Render)) throw new NotImplementedException();
            if (binder.CallInfo.ArgumentNames.Count != args.Length) throw new NotImplementedException();

            var dictionary = new Dictionary<string, object?>();
            for(var argIndex = 0; argIndex < binder.CallInfo.ArgumentNames.Count; ++argIndex)
            {
                dictionary.Add(binder.CallInfo.ArgumentNames[argIndex], args[argIndex]);
            }
            result = Template.Render(dictionary);
            return true;
        }
    }
}
