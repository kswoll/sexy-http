using System;

namespace SexyHttp.Utils
{
    internal static class FunctionalExtensions
    {
        public static bool IfNotNull<T>(this T operand, Action<T> ifNotNull) where T : class
        {
            if (operand != null)
            {
                ifNotNull(operand);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
