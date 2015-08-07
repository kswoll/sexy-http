using System.Threading.Tasks;
using SexyHttp.TypeConverters;
using SexyHttp.Utils;

namespace SexyHttp
{
    public abstract class HttpArgumentHandler : IHttpArgumentHandler
    {
        public ITypeConverter TypeConverter { get; }

        protected HttpArgumentHandler(ITypeConverter typeConverter)
        {
            TypeConverter = typeConverter;
        }

        public virtual Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            return TaskConstants.Completed;
        }

        public virtual Task ApplyArgument(HttpApiResponse response, string name, object argument)
        {
            return TaskConstants.Completed;
        }
    }
}
