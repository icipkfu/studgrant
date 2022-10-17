using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Grant.Core;
using System.Threading.Tasks;
using Grant.Utils.Extensions;

namespace Grant.WebApi.Controllers
{
    public class FileOriginalController : BaseController
    {
        private IFileManager _manager;

        private IFileManager manager
        {
            get { return (_manager ?? (_manager = Container.Get<IFileManager>())); }
        }

        public async Task<IHttpActionResult> PostFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsStreamAsync();

                var result = await manager.UploadOriginal(buffer, filename);

                return Ok(result);
            }

            return BadRequest("Ошибка заливки файла");
        }
    }
}
