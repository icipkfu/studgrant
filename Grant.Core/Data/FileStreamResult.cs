using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Grant.Core.Data
{
    public class FileStreamResult : IHttpActionResult
    {
        private readonly Stream fileStream;
        private readonly string fileName;

        public FileStreamResult(Stream fileStream, string fileName)
        {
            this.fileStream = fileStream;
            this.fileName = fileName;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(fileStream)
                };

                var contentType = MimeMapping.GetMimeMapping(Path.GetExtension(fileName));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                return response;
            }, cancellationToken);
        }
    }
}
