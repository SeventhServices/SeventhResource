using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Enums;
using SeventhServices.Resource.Common.Files;

namespace SeventhServices.Resource.Common.Utilities
{
    public static class AccountHelper
    {
        private static readonly string KcFileName = GetKcAccountFileName();

        public static Account ReadFromFile(string pid, string savePath,
                            AccountFileType accountFileType = AccountFileType.Kc)
        {
            var filePath = GetAccountFilePath(pid, savePath, AccountFileType.Json);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} is not exist");
            }

            if (accountFileType != AccountFileType.Kc)
            {
                return JsonSerializer.Deserialize<Account>(
                    File.ReadAllText(filePath));
            }

            var accountDataFile = new FileDictionary(filePath);
            return new Account(accountDataFile["pid"], accountDataFile["id"]);

        }

        public static void ConvertToFile(Account account, string savePath)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }
            var stringBuilder = new StringBuilder(savePath);

            stringBuilder.Append($"/{account.Pid}/");

            var tempPath = stringBuilder.ToString();

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            tempPath = stringBuilder.Append(KcFileName).ToString();

            var accountDataFile = new FileDictionary(tempPath)
            {
                {"pid", account.EncPid}, {"id", account.Id}
            };

            accountDataFile.Save();
        }

        public static string GetAccountFilePath(string pid, string savePath,
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            return accountFileType switch
            {
                AccountFileType.Kc => Path.Combine(savePath, pid, $"{KcFileName}"),
                AccountFileType.Json => Path.Combine(savePath, pid, $"{pid}.json"),
                _ => throw new ArgumentOutOfRangeException(nameof(accountFileType), accountFileType, null)
            };
        }

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