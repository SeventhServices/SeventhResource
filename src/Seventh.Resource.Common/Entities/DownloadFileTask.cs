﻿namespace Seventh.Resource.Common.Entities
{
    public class DownloadFileTask
    {
        public string FileName { get; set; }

        public bool NeedHash { get; set; }

        public int? Revision { get; set; }
    }
}