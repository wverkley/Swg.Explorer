using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class StringFile : ISWGFile
    {
        public class StringFileItem
        {
            public int Id { get; internal set; }
            public string Name { get; internal set; }
            public string Value { get; internal set; }
            internal StringFileItem() { }

            public override string ToString()
            {
                return string.Format("ID={0}, Name={1}, Value={2}", Id, Name, Value);
            }
        }

        public int Header { get; internal set; }
        public byte ByteFlag { get; internal set; }

        public IEnumerable<StringFileItem> Items { get; internal set; }
        internal StringFile() { }

        public override string ToString()
        {
            return string.Format("Items.Count={0}", Items.Count());
        }

    }
}
