using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.FunctionCalling.Models;
public sealed class TypeRegistration
{
    public Type Type { get; }
    public Func<object> TypeFactory { get; }

    public TypeRegistration(Type type, Func<object> typeFactory)
    {
        Type = type;
        TypeFactory = typeFactory;
    }
}
