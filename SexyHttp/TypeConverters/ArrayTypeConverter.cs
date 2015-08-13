using System;

namespace SexyHttp.TypeConverters
{
    public class ArrayTypeConverter : ITypeConverter
    {
        public bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object obj, out object result)
        {
            if (convertTo.IsArray && obj.GetType().IsArray)
            {
                var sourceArray = (Array)obj;
                var destinationArray = Array.CreateInstance(convertTo.GetElementType(), sourceArray.Length);
                for (var i = 0; i < sourceArray.Length; i++)
                {
                    var sourceElement = sourceArray.GetValue(i);
                    object destinationElement;
                    if (!root.TryConvertTo(root, context, convertTo.GetElementType(), sourceElement, out destinationElement))
                    {
                        result = null;
                        return false;
                    }
                    destinationArray.SetValue(destinationElement, i);
                }
                result = destinationArray;
                return true;
            }
            result = null;
            return false;
        }
    }
}
