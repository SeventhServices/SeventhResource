using System;

namespace SeventhServices.Resource.Common.Abstractions
{
    public abstract class ConfigureFile : IConfigureFile
    {
        public DateTime LastModify { get; set; }
    }
}