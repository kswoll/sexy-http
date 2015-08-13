using System;
using System.Collections.Generic;
using System.Text;

namespace SexyHttp.TypeConverters
{
    [Flags]
    public enum TypeConversionContext
    {
        None = 0,
        Path = 1,
        Query = 2,
        Body = 4,
        Header = 8,
        All = Path | Query | Body | Header
    }
}
