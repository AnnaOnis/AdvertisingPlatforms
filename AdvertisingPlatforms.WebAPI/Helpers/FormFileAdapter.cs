using System.Threading;
using AdvertisingPlatforms.Domain.Abstractions;

namespace AdvertisingPlatforms.Web.Helpers
{
    public class FormFileAdapter : IFileData
    {
        private readonly IFormFile _formFile;
        public FormFileAdapter(IFormFile formFile)
        {
            _formFile = formFile;
        }
        public string FileName => _formFile.FileName;

        public long FileLength => _formFile.Length;

        public string ContentType => _formFile.ContentType;

        public async Task<MemoryStream> FileToMemoryStreamAsync(CancellationToken cancellationToken)
        {
            var stream = new MemoryStream();
            await _formFile.CopyToAsync(stream, cancellationToken);
            stream.Position = 0;

            return stream;
        }
    }
}
