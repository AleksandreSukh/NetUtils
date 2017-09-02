using System;
using System.Linq;

namespace NetUtils
{
    public static class ExceptionExtensions
    {
        public static string GetMessagesRecursiveToInnerEx(this Exception exception)
        {
            var messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.ToString());
            return string.Join(Environment.NewLine, messages.ToArray());
        }
    }
}