using System;
using System.Linq;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// If you have T and want T[], this converter converts to that for you
    /// </summary>
    public class ArrayTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(Type convertTo, object obj, out object result)
        {
/*
            if (convertTo.IsArray && obj.GetType().IsArray)
            {
                var sourceArray = (Array)obj;
                var destinationArray = Array.CreateInstance(convertTo.GetElementType(), sourceArray.Length);
                for (var i = 0; i < sourceArray.Length; i++)
                {
                    var sourceElement = sourceArray.GetValue(i);
                    var destinationElement = ;
                }
                return sourceArray.Cast<object>().Select(x => result.ConvertTo<>())                
            }
*/
            if (convertTo.IsArray && convertTo.GetElementType().IsInstanceOfType(obj))
            {
                var array = Array.CreateInstance(convertTo.GetElementType(), 1);
                array.SetValue(obj, 0);
                result = array;
                return true;
            }
            result = null;
            return false;
        }
    }
}
