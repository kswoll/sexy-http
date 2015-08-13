using System;
using System.Threading.Tasks;
using SexyHttp.HttpBodies;
using SexyHttp.TypeConverters;

namespace SexyHttp.ArgumentHandlers
{
    public class FormArgumentHandler : HttpArgumentHandler
    {
        public string Name { get; }

        public FormArgumentHandler(ITypeConverter typeConverter, string name = null) : base(typeConverter)
        {
            Name = name;
        }

        public override Task ApplyArgument(HttpApiRequest request, string name, object argument)
        {
            if (request.Body == null)
                request.Body = new FormHttpBody();
            else if (!(request.Body is FormHttpBody))
                throw new Exception("Cannot apply form argument because body is already assigned and is not an instance of FormHttpBody");

            var form = (FormHttpBody)request.Body;
            var value = TypeConverter.ConvertTo<string>(TypeConversionContext.Body, argument);
            form.Values[Name ?? name] = value;

            return base.ApplyArgument(request, name, argument);
        }
    }
}
