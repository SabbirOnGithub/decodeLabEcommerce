using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
    public class FileActionResult : IHttpActionResult
    {
        public FileActionResult(byte[] bytes, string fileName)
        {
            this.Bytes = bytes;
            this.FileName = fileName;
        }

        public byte[] Bytes{ get; private set; }
        public string FileName { get; set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage();
            response.Content = new ByteArrayContent(Bytes);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = this.FileName
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") 
            {
                FileName = this.FileName
            };;

            // NOTE: Here I am just setting the result on the Task and not really doing any async stuff. 
            // But let's say you do stuff like contacting a File hosting service to get the file, then you would do 'async' stuff here.

            return Task.FromResult(response);
        }
    }
}
