using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using Seventh.Resource.Common.Entities;

namespace Seventh.Resource.Services
{
    public class QueueDownloadService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITypedHttpClientFactory<DownloadClient> _downloadClientFactory;
        private readonly ITypedHttpClientFactory<OneByOneDownloadClient> _oneByOneDownloadClientFactory;
        private readonly SortService _sortService;
        private readonly AssetInfoProvider _infoProvider;
        private readonly ResourceLocation _location;
        private readonly Queue<DownloadFileTask> _taskQueue = new Queue<DownloadFileTask>();
        public bool IsFree { get; set; } = true;

        private DownloadClient _client;
        private DownloadService _downloadService;

        private OneByOneDownloadClient _onebyOneClient;
        private BasicDownloadService _basicDownloadService;

        public QueueDownloadService(IHttpClientFactory clientFactory,
            ITypedHttpClientFactory<DownloadClient> downloadClientFactory,
            ITypedHttpClientFactory<OneByOneDownloadClient> oneByOneDownloadClientFactory,
            SortService sortService, AssetInfoProvider infoProvider, ResourceLocation location)
        {
            _clientFactory = clientFactory;
            _downloadClientFactory = downloadClientFactory;
            _oneByOneDownloadClientFactory = oneByOneDownloadClientFactory;
            _sortService = sortService;
            _infoProvider = infoProvider;
            _location = location;
        }

        public void Enqueue(DownloadFileTask task)
        {
            if (_taskQueue.Any(t =>
                t.FileName.Equals(task.FileName)))
            {
                return;
            }
            _taskQueue.Enqueue(task);
        }

        public async void DequeueAll()
        {
            if (!IsFree)
            {
                return;
            }
            IsFree = false;
            while (_taskQueue.TryDequeue(out var task))
            {
                await RunATask(task);
            }
            _client = null;
            _downloadService = null;
            IsFree = true;
        }

        private async Task RunATask(DownloadFileTask task)
        {
            if (task == null) return;

            if (task.IsBasicDownload)
            {
                _onebyOneClient ??= _oneByOneDownloadClientFactory.CreateClient(_clientFactory.CreateClient(nameof(QueueDownloadService)));
                _basicDownloadService ??= new BasicDownloadService(_onebyOneClient, _sortService, _infoProvider, _location);

                var eventArgs = new DownloadCompleteEventArgs();

                (eventArgs.Result, eventArgs.Info) =
                    await _basicDownloadService.TryDownloadAndSortBasicZipAssetAsync(task.FileName, task.OverWrite);
                
                DownloadCompete?.Invoke(this, eventArgs);
            }
            else
            {
                _client ??= _downloadClientFactory.CreateClient(_clientFactory.CreateClient(nameof(QueueDownloadService)));
                _downloadService ??= new DownloadService(_client, _sortService, _infoProvider, _location);

                var eventArgs = new DownloadCompleteEventArgs();
                if (task.Revision != null)
                {
                    (eventArgs.Result, eventArgs.Info) =
                        await _downloadService.TryDownloadAtRevisionAndSortAsync(
                            task.FileName, task.Revision.Value, task.NeedHash == true);
                }
                else
                {
                    (eventArgs.Result, eventArgs.Info) =
                        await _downloadService.TryDownloadAtMirrorAndSortAsync(
                                task.FileName, task.NeedHash);
                }
                DownloadCompete?.Invoke(this, eventArgs);
            }

            GC.Collect();
        }

        public class DownloadCompleteEventArgs
        {
            public bool Result { get; set; }
            public AssetInfo Info { get; set; }
        }

        public event EventHandler<DownloadCompleteEventArgs> DownloadCompete;
    }

}