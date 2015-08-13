using System;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// Provides a set of four methods for effectively interacting with an ITypeConverter.  The actual 
    /// <c>TryConvertTo</c> method defined on <c>ITypeConverter</c> should never actually be called (it's called
    /// by extension methods provided here).  The four methods allow you to -- either with generics or not; and 
    /// using "Try" semantics or exception semantics -- convert a value to a target type.
    /// </summary>
    public static class TypeConverterExtensions
    {
        /// <summary>
        /// Converts the specified value into the type indicated by <c>T</c>.  The result of the conversion
        /// is stored in result and the value returned indicates whether conversion was successful.<br/>
        /// </summary>
        /// <param name="typeConverter">The type converter performing the conversion (i.e. "this").</param>
        /// <param name="context">The usage scenario of the type conversion (path, query string, etc.)</param>
        /// <param name="convertTo">The type to which the <c>value</c> should be converted.</param>
        /// <param name="value">The object that should be converted to <c>T</c>.</param>
        /// <returns>The <c>value</c> converted into type <c>T</c>.</returns>
        /// <exception cref="TypeConversionException">When the type converter could not convert the value to the specified 
        /// type.</exception>
        public static object ConvertTo(this ITypeConverter typeConverter, TypeConversionContext context, Type convertTo, object value)
        {
            object result;
            if (!typeConverter.TryConvertTo(typeConverter, context, convertTo, value, out result))
                throw new TypeConversionException(value, convertTo);
            return result;
        }

        /// <summary>
        /// Converts the specified value into the type indicated by <c>T</c>.  The result of the conversion
        /// is stored in result and the value returned indicates whether conversion was successful.<br/>
        /// </summary>
        /// <param name="typeConverter">The type converter performing the conversion (i.e. "this").</param>
        /// <param name="context">The usage scenario of the type conversion (path, query string, etc.)</param>
        /// <param name="convertTo">The type to which the <c>value</c> should be converted.</param>
        /// <param name="value">The object that should be converted to <c>T</c>.</param>
        /// <param name="result">The result of the conversion is stored here.</param>
        /// <returns>True if this type converter could accomodate the conversion and False if it could not.</returns>
        public static bool TryConvertTo(this ITypeConverter typeConverter, TypeConversionContext context, Type convertTo, object value, out object result)
        {
            return typeConverter.TryConvertTo(typeConverter, context, convertTo, value, out result);
        }

        /// <summary>
        /// Converts the specified value into the type indicated by <c>T</c>.  The result of the conversion
        /// is stored in result and the value returned indicates whether conversion was successful.<br/>
        /// </summary>
        /// <typeparam name="T">The type to which the <c>value</c> should be converted.</typeparam>
        /// <param name="typeConverter">The type converter performing the conversion (i.e. "this").</param>
        /// <param name="context">The usage scenario of the type conversion (path, query string, etc.)</param>
        /// <param name="value">The object that should be converted to <c>T</c>.</param>
        /// <param name="result">The result of the conversion is stored here.</param>
        /// <returns>True if this type converter could accomodate the conversion and False if it could not.</returns>
        public static bool TryConvertTo<T>(this ITypeConverter typeConverter, TypeConversionContext context, object value, out T result)
        {
            object objectResult;
            var converted = typeConverter.TryConvertTo(typeConverter, context, typeof(T), value, out objectResult);
            if (converted)
            {
                result = (T)objectResult;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Converts the specified value into the type indicated by <c>T</c>.  The result of the conversion
        /// is stored in result and the value returned indicates whether conversion was successful.<br/>
        /// </summary>
        /// <typeparam name="T">The type to which the <c>value</c> should be converted.</typeparam>
        /// <param name="typeConverter">The type converter performing the conversion (i.e. "this").</param>
        /// <param name="context">The usage scenario of the type conversion (path, query string, etc.)</param>
        /// <param name="value">The object that should be converted to <c>T</c>.</param>
        /// <returns>The <c>value</c> converted into type <c>T</c>.</returns>
        /// <exception cref="TypeConversionException">When the type converter could not convert the value to the specified 
        /// type.</exception>
        public static T ConvertTo<T>(this ITypeConverter typeConverter, TypeConversionContext context, object value)
        {
            return (T)typeConverter.ConvertTo(context, typeof(T), value);
        }
    }
}
