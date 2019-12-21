using System;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SeventhServices.Client.Common;
using SeventhServices.Client.Network.Interfaces;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Enums;
using SeventhServices.Resource.Services.Abstractions;
using WebApiClient;

namespace SeventhServices.Resource.Services
{
    public class GameVersionCheckService : ICheckService
    {
        private readonly StatusService _statusService;
        private readonly ICheckUpdateApiClient _apiClient;

        public GameVersionCheckService(StatusService statusService, ICheckUpdateApiClient apiClient)
        {
            _statusService = statusService;
            _apiClient = apiClient;
        }

        public GameVersion NowVersion { get; set; } = new GameVersion();

        public void NoticeStatus()
        {
            _statusService.GameVersion = new GameVersion
            {
                DownloadPath = NowVersion.DownloadPath,
                Version = NowVersion.Version
            };
        }



        public async Task<GameVersion> TryUpdate(bool isDownload = false,
            GameVersionCheckSource source = GameVersionCheckSource.OneApp)
        {
            var updater = new Updater(_apiClient, source);
            var (checkResult, newVersion) = await updater.CheckUpdate();
            if (!checkResult)
            {
                throw new Exception("check failed");
            }

            if (newVersion <= NowVersion)
            {
                return newVersion;
            }

            if (isDownload)
            {
                await updater.DownloadUpdate(
                    newVersion.DownloadPath,
                    GetApkPath(_statusService.PathOption.AssetPath.ApkDownloadTempPath));
            }

            UpdateStatus(newVersion);
            NoticeStatus();

            return newVersion;
        }


        private string GetApkPath(string savePath)
        {
            return Path.Combine(savePath, $"{NowVersion}.apk");
        }

        private void UpdateStatus(GameVersion nowGameVersion)
        {
            NowVersion.Version = nowGameVersion.Version;
            NowVersion.DownloadPath = nowGameVersion.DownloadPath;
        }




        private class Updater
        {
            private readonly ICheckUpdateApiClient _apiClient;

            public Updater(ICheckUpdateApiClient apiClient, GameVersionCheckSource source = GameVersionCheckSource.OneApp)
            {
                _apiClient = apiClient;
                Source = source;
            }

            private GameVersionCheckSource Source { get; set; }


            public async Task DownloadUpdate(string url, string savePath)
            {
                var responseFile = await _apiClient.DownloadUpdateAsync(url)
                    .Retry(3, i => TimeSpan.FromSeconds(i))
                    .WhenCatch<Exception>();
                await responseFile.SaveAsAsync(savePath);
            }


            public async Task<(bool, GameVersion)> CheckUpdate()
            {

                var html = Source switch
                {
                    GameVersionCheckSource.OneApp => await _apiClient.From1App(),
                    GameVersionCheckSource.QooApp => await _apiClient.FromQooApp(),
                    GameVersionCheckSource.GooglePlay => throw new NotImplementedException(),
                    GameVersionCheckSource.ApkMirror => throw new NotImplementedException(),
                    _ => throw new ArgumentNullException()
                };


                var (checkResult, newVersion) = Analyze(html, Source);
                return (checkResult, newVersion);
            }

            private static (bool, GameVersion) Analyze(string html, GameVersionCheckSource source)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                return source switch
                {
                    GameVersionCheckSource.OneApp => AnalyzeFrom1App(htmlDocument),
                    GameVersionCheckSource.QooApp => AnalyzeFromQooApp(htmlDocument),
                    GameVersionCheckSource.GooglePlay => throw new NotImplementedException(),
                    _ => throw new ArgumentNullException()
                };
            }

            private static (bool, GameVersion) AnalyzeFrom1App(HtmlDocument htmlDocument)
            {
                string SelectHtmlToStringFor1App(string selectString, string param = " ", bool attributes = false)
                {
                    const string newVersionDivSelectString = "//body/div[2]/div[last()]/div[1]";

                    if (attributes)
                        return htmlDocument.DocumentNode.SelectSingleNode(
                            newVersionDivSelectString +
                            selectString).Attributes[param].Value;

                    return htmlDocument.DocumentNode.SelectSingleNode(
                            newVersionDivSelectString +
                            selectString).InnerText
                        .Replace("\t", "")
                        .Replace(" ", "")
                        .Replace("\r", "")
                        .Replace("\n", "")
                        .Replace(param, "");
                }

                try
                {

                    var tempVersion = SelectHtmlToStringFor1App("/div[1]/div[1]/a/span");
                    return (true, new GameVersion
                    {
                        Version = tempVersion,
                        DownloadPath = UriConst.ApkFrom1AppBaseUrl + SelectHtmlToStringFor1App("/div[2]/div/a", "href", true)
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($@"Check Update Error! : {e.Message}");
                    return (false, null);
                }
            }

            private static (bool, GameVersion) AnalyzeFromQooApp(HtmlDocument htmlDocument)
            {
                try
                {
                    const string tempVersion = "//body/div[1]/div[1]/section[2]/div[2]/ul/li/span[2]/var";
                    var version = htmlDocument.DocumentNode.SelectSingleNode(tempVersion).InnerText;

                    return (true, new GameVersion
                    {
                        Version = version
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($@"Check Update Error! : {e.Message}");
                    return (false, null);
                }
            }
        }
    }
}