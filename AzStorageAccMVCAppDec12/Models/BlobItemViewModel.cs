using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzStorageAccMVCAppDec12.Models
{
    public class BlobItemViewModel
    {
        public string Name { get; set; }
        public Uri Uri { get; set; }
        public long? Size { get; set; }
        public string ContentType { get; set; }
        public DateTimeOffset? LastModified { get; set; }
    }
}