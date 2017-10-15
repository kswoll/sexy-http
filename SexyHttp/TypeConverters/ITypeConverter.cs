using System;

namespace SexyHttp.TypeConverters
{
    /// <summary>
    /// Provides a method to convert an object from one type to another.
    /// </summary>
    public interface ITypeConverter
    {
        /// <summary>
        /// Converts the specified value into the type indicated by <c>convertTo</c>.  The result of the conversion
        /// is stored in result and the value returned indicates whether conversion was successful.<br/>
        /// </summary>
        /// <param name="root">The top-level instance of ITypeConverter that initiated the conversion.  Implementors
        /// should use this to perform additional conversions from within your type converter. Callers should not be
        /// invoking this method in the first place.</param>
        /// <param name="context">The usage scenario of the type conversion (path, query string, etc.)</param>
        /// <param name="convertTo">The type to which the <c>value</c> should be converted.</param>
        /// <param name="value">The object that should be converted to <c>convertTo</c>.</param>
        /// <param name="result">The result of the conversion is stored here.</param>
        /// <returns>True if this type converter could accomodate the conversion and False if it could not.</returns>
        /// <remarks>
        /// Callers should never invoke this method directly.  Rather, you should invoke one of
        /// the methods in TypeConverterExtensions instead which will take care of specifying the ITypeConverter root.
        /// </remarks>
        bool TryConvertTo(ITypeConverter root, TypeConversionContext context, Type convertTo, object value, out object result);
    }
}
