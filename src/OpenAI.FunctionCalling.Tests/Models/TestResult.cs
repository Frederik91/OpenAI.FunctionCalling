using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.FunctionCalling.Tests.Models;
internal sealed record TestResult(string Data, TestOptions? Option = null);