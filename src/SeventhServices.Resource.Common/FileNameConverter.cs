﻿using System;
using System.Globalization;
using SeventhServices.Resource.Common.Crypts;

namespace SeventhServices.Resource.Common
{
    public static class FileNameConverter
    {
        public static string ToHashName(string fileName)
        {
            throw new NotImplementedException();
        }

        public static string ToWithHashName(string fileName)
        {
            return AssetCrypt.ConvertFileName(fileName,
                AssetCrypt.EncVersion.Ver1,
                AssetCrypt.EncVersion.Ver2);
        }

        public static string ToLargeCardFile(int cardId)
        {
            return ToWithHashName($"card_l_{cardId.ToString("D5", CultureInfo.CurrentCulture)}.jpg.enc");
        }
    }
}