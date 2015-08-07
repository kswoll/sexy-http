using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class FormArgumentHandler : HttpArgumentHandler
    {
        public FormArgumentHandler(ITypeConverter typeConverter) : base(typeConverter)
        {
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body == null)
                request.Body = new FormHttpBody();
            else if (!(request.Body is FormHttpBody))
                throw new Exception("Cannot apply form argument because body is already assigned and is not an instance of FormHttpBody");

            var form = (FormHttpBody)request.Body;
            var value = TypeConverter.ConvertTo<string>(argument);
            form.Values[name] = value;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
