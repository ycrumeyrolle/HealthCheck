using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ServerFileSwitchOptions
    {
        public string FilePath { get; set; }

        public IFileProvider FileProvider { get; set; }

        public int? RetryDelay { get; set; }
    }
}