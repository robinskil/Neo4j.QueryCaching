using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest.QueryCaching
{
    public class Model
    {
        public string UniqueId { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        public string ShortName { get; set; }
        public long Order { get; set; }
    }
}
