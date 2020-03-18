using System;
using System.Collections.Generic;
using System.Text;

namespace Seventh.Core.Dto.Response.Resource
{
    public class AssetFileInfoDto
    {
        public string Name { get; set; }
        public int Revision { get; set; }
        public long Size { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public bool IsExist { get; set;}
    }
}
