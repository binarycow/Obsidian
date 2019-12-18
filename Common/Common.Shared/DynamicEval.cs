using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common
{
    public static class DynamicEval
    {

        public static bool TryGetDynamicMember(object item, string memberName, out object? result)
        {
            try
            {
                var binder = Binder.GetMember(CSharpBinderFlags.None, memberName, item.GetType(),
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
                result = callsite.Target(callsite, item);
                return true;
            }
            catch(RuntimeBinderException)
            {
                result = default;
                return false;
            }
        }
    }
}
