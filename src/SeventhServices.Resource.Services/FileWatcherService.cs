using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SeventhServices.Resource.Common.Classes.Options;
using SeventhServices.Resource.Common.Extensions;
using SeventhServices.Resource.Common.Helpers;
using SeventhServices.Resource.Common.Utilities;

namespace SeventhServices.Resource.Services
{
    public class FileWatcherService
    {
        private readonly ILogger<FileWatcherService> _logger;
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();
        private readonly FileSystemWatcher _revWatcher = new FileSystemWatcher();
        private readonly PathOption _pathOption;

        public FileWatcherService(OptionService optionService, ILoggerFactory loggerFactory)
        {
            _pathOption = optionService.PathOption;
            _logger = loggerFactory.CreateLogger<FileWatcherService>();

            _watcher.Path = _pathOption.AssetPath.GameMirrorAssetPath;
            _watcher.Created += OnCreated;
            //_watcher.Changed += OnCreated;
        }

        public void StartWatch()
        {
            _watcher.EnableRaisingEvents = true;

            _logger.LogInformation($"Start watch {_watcher.Path}");
        }

        public async void OnCreated(object source, FileSystemEventArgs e)
        {
            _logger.LogInformation($"Found {e.Name}");

            // This delay can avoid the error using by another process.
            // (Copying file task after download file is using it now.)
            await Task.Delay(500);

            await AssetCryptHelper.DecryptWithRename(e.FullPath,
                _pathOption.AssetPath.SortedAssetPath
                    .AppendAndCreatePath("Card", "l"));

            _logger.LogInformation($"Decrypt {e.Name} complete");
        }
    }
}