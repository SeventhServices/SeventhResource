using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SeventhServices.Resource.Common;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Enums;
using SeventhServices.Resource.Common.Files;

namespace SeventhServices.Resource.Services
{
    public class AccountService
    {

        public Account ReadFromFile(string pid, string savePath, 
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            var filePath = GetAccountFilePath(pid, savePath, AccountFileType.Json);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} is not exist");
            }

            if (accountFileType == AccountFileType.Kc)
            {
                var accountDataFile = new FileDictionary(filePath);
                return new Account(accountDataFile["pid"], accountDataFile["id"]);
            }

            return JsonSerializer.Deserialize<Account>(
                File.ReadAllText(filePath));
        }

        public void ConvertToFile(Account account, string savePath)
        {
            var stringBuilder = new StringBuilder(savePath);

            stringBuilder.Append($"/{account.Pid}/");

            var tempPath = stringBuilder.ToString();

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            var sHa1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            var array = sHa1CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(SecretKey.Implement.SaveDataServices));
            
            foreach (var b in array)
            {
                stringBuilder.Append($"{b,0:x2}");
            }

            tempPath = stringBuilder.Append(GetKcAccountFileName()).ToString();

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
                AccountFileType.Kc => Path.Combine(savePath, pid, $"{GetKcAccountFileName()}"),
                AccountFileType.Json => Path.Combine(savePath, pid, $"{pid}.json"),
                _ => throw new ArgumentOutOfRangeException(nameof(accountFileType), accountFileType, null)
            };
        }

        private static string GetKcAccountFileName()
        {
            var stringBuilder = new StringBuilder();
            var sHa1CryptoServiceProvider = new SHA1CryptoServiceProvider();
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