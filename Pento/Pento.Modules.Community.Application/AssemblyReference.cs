using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Modules.Community.Application;
internal class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;

}
