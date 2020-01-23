using SeventhServices.Resource.Common.Abstractions;

namespace SeventhServices.Resource.Common.Classes
{
    public class Status
    {
        public string Version { get; set; } = "6.8.1";
        public string DownloadPath { get; set; } = string.Empty;
        public int Revision { get; set; } = 368;
        public string Pid { get; set; } = SecretKey.Implement.DefaultEncPid;
        public string Uuid { get; set; } = SecretKey.Implement.DefaultId;

        public void SetGameVersion(GameVersion gameVersion)
        {
            if (gameVersion == null)
            {
                return;
            }
            Version = gameVersion.Version;
            DownloadPath = gameVersion.DownloadPath;
        }

        public GameVersion GetGameVersion()
        {
            return new GameVersion
            {
                Version = Version,
                DownloadPath = DownloadPath
            };
        }

        public void SetAccount(Account account)
        {
            if (account == null)
            {
                return;
            }
            Pid = account.Pid;
            Uuid = account.Uuid;
        }

        public Account GetAccount()
        {
            return new Account(Pid, Uuid,false);
        }
    }
}