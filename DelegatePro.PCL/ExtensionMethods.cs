using System;
using System.Threading.Tasks;

namespace DelegatePro.PCL
{
    public static class ExtensionMethods
    {
        public static void WithoutAwait(this Task t)
        {
            //Used to explicity call out when a task is not awaited without throwing warnings.
        }
    }
}

