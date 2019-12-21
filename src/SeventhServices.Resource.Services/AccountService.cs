using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SeventhServices.Resource.Common;
using SeventhServices.Resource.Common.Classes;
using SeventhServices.Resource.Common.Enums;
using SeventhServices.Resource.Common.Files;

namespace SeventhServices.Resource.Services
{
    public class AccountService
    {
        public AccountService()
        {

        }

        public void ConvertToFile(Account account, string savePath)
        {
            ToFile(account, savePath);
        }

        public Account ReadFromFile(string pid, string savePath, AccountFileType accountFileType = AccountFileType.Kc)
        {
            return ToAccount(GetFilePath(savePath, pid, accountFileType));
        }

        public static string GetFilePath(string savePath, string pid,
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            return accountFileType switch
            {
                AccountFileType.Kc => Path.Combine(savePath, pid, $"{GetKcFileName()}"),
                AccountFileType.Json => Path.Combine(savePath, pid, $"{pid}.json"),
                _ => throw new ArgumentOutOfRangeException(nameof(accountFileType), accountFileType, null)
            };
        }

        private static string GetKcFileName()
        {
            var stringBuilder = new StringBuilder();
            var sHa1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] array = sHa1CryptoServiceProvider.ComputeHash(
                Encoding.ASCII.GetBytes(SecretKey.Implement.SaveDataServices));
            byte[] array2 = array;
            foreach (byte b in array2)
            {
                stringBuilder.Append($"{b,0:x2}");
            }

            return stringBuilder + ".kc";
        }

        public static Account ToAccount(string filePath,
            AccountFileType accountFileType = AccountFileType.Kc)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{filePath} is not exist");
            }

            if (accountFileType == AccountFileType.Kc)
            {
                var accountDataFile = new FileDictionary(filePath);
                return new Account(accountDataFile["pid"], accountDataFile["id"]);
            }

            return System.Text.Json.JsonSerializer.Deserialize<Account>(
                File.ReadAllText(filePath));
        }

        public static void ToFile(Account account, string path)
        {
            var stringBuilder = new StringBuilder(path);

            stringBuilder.Append($"/{account.Pid}/");

            var tempPath = stringBuilder.ToString();

            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            var sHa1CryptoServiceProvider = new SHA1CryptoServiceProvider();
            byte[] array = sHa1CryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(SecretKey.Implement.SaveDataServices));
            byte[] array2 = array;
            foreach (byte b in array2)
            {
                stringBuilder.Append($"{b,0:x2}");
            }

            tempPath = stringBuilder.Append(GetKcFileName()).ToString();

            var accountDataFile = new FileDictionary(tempPath)
                {
                    {"pid", account.EncPid}, {"id", account.Id}
                };
            accountDataFile.Save();
        }

    }
}