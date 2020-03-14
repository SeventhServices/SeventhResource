using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Seventh.Resource.Common.Entities;
using Seventh.Resource.Common.Enums;
using Seventh.Resource.Common.Extensions;
using Seventh.Resource.Common.Files;

namespace Seventh.Resource.Common.Helpers
{
    public static class AccountHelper
    {
        private static readonly string KcFileName = GetKcAccountFileName();

        public static string ExportToFile(Account account, string saveDirectory,
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            var savePath = saveDirectory
                .AppendAndCreatePath(account.Pid)
                .AppendPath(accountFileType == AccountFileType.Json 
                    ? $"{account.Pid}.json" : KcFileName);

            ConvertToFile(account, savePath);
            return savePath;
        }

        public static void ConvertToFile(Account account, string savePath, 
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));

            if (accountFileType == AccountFileType.Json)
            {
                File.WriteAllText(savePath,JsonSerializer.Serialize(account));
            }
            var accountDataFile = new FileDictionary(savePath)
            {
                {"pid", account.EncPid}, {"id", account.Id}
            };
            accountDataFile.Save();
        }

        public static Account ReadFromFile(string pid, string saveDirectory,
                            AccountFileType accountFileType = AccountFileType.Kc)
        {
            var filePath = GetExportAccountFilePath(pid, saveDirectory, accountFileType);
            return ReadFromFile(filePath, accountFileType);
        }

        public static Account ReadFromFile(string filePath, AccountFileType accountFileType = AccountFileType.Kc)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} is not exist");
            }

            if (accountFileType == AccountFileType.Json)
            {
                return JsonSerializer.Deserialize<Account>(
                    File.ReadAllText(filePath));
            }

            var accountDataFile = new FileDictionary(filePath);
            return new Account(accountDataFile["pid"], accountDataFile["id"]);
        }

        public static string GetExportAccountFilePath(string pid, string saveDirectory,
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            return accountFileType switch
            {
                AccountFileType.Kc => Path.Combine(saveDirectory, pid, $"{KcFileName}"),
                AccountFileType.Json => Path.Combine(saveDirectory, pid, $"{pid}.json"),
                _ => throw new ArgumentOutOfRangeException(nameof(accountFileType), accountFileType, null)
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5350:不要使用弱加密算法", Justification = "<挂起>")]
        private static string GetKcAccountFileName()
        {
            var stringBuilder = new StringBuilder();
            using var sHa1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            var array = sHa1CryptoServiceProvider.ComputeHash(
                Encoding.ASCII.GetBytes(SecretKey.Implement.SaveDataServices));
            foreach (var b in array)
            {
                stringBuilder.Append($"{b,0:x2}");
            }
            return stringBuilder + ".kc";
        }
    }
}