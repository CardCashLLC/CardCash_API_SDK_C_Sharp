using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CardCash_API
{

    internal class JsonContent : HttpContent
    {
        private readonly MemoryStream _memoryStream = new MemoryStream();

        public JsonContent(object value)
        {

            Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var JSONTextWriter = new JsonTextWriter(new StreamWriter(_memoryStream)) { Formatting = Formatting.Indented };

            var serializer = new JsonSerializer();

            serializer.Serialize(JSONTextWriter, value);
            JSONTextWriter.Flush();

            _memoryStream.Position = 0;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return _memoryStream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _memoryStream.Length;

            return true;
        }
    }

}
