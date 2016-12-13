using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.HealthCheck
{
    public class FileServerSwitch : IServerSwitch
    {
        private readonly IFileProvider _fileProvider;
        private readonly ServerFileSwitchOptions _options;
        private bool _fileExists;

        public FileServerSwitch(IHostingEnvironment hostingEnvironment, IOptions<ServerFileSwitchOptions> options)
        {
            _options = options.Value;
            _fileProvider = _fileProvider = _options.FileProvider ?? hostingEnvironment.WebRootFileProvider;
            FileChanged();
            ChangeToken.OnChange(() => _fileProvider.Watch(_options.FilePath), FileChanged);
        }

        private void FileChanged()
        {
            var file = _fileProvider.GetFileInfo(_options.FilePath);
            _fileExists = file != null && file.Exists;
        }
        
        public Task CheckServerState(ServerSwitchContext context)
        {
            if (_fileExists)
            {
                context.Disable(_options.RetryDelay);
            }
            else
            {
                context.Enable();
            }

            return TaskCache.CompletedTask;
        }
    }
}