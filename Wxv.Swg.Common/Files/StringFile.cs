using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    /// <summary>
    /// String file
    /// </summary>
    public class StringFile
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

        public static StringFile Load(Stream stream)
        {
            var header = stream.ReadInt32();
            var byteFlag = (byte) stream.ReadByte();
            var nextIndex = stream.ReadInt32();
            var count = stream.ReadInt32();

            var items = new StringFileItem[count];
            for (var i = 0; i < count; i++)
            {
                var id = stream.ReadInt32();
                stream.ReadInt32(); // ?
                var length = stream.ReadInt32() * 2;
                var data = stream.ReadBytes(length);
                var value = Encoding.ASCII.GetString(Encoding.Convert(Encoding.Unicode, Encoding.ASCII, data));

                items[i] = new StringFileItem
                {
                    Id = id,
                    Value = value
                };
            }
            for (var i = 0; i < count; i++)
            {
                var id = stream.ReadInt32();
                var length = stream.ReadInt32();
                var name = Encoding.ASCII.GetString(stream.ReadBytes(length));
                var item = items.FirstOrDefault(i0 => i0.Id == id);
                if (item != null)
                    items[i].Name = name;
            }

            return new StringFile
            {
                Header = header,
                ByteFlag = byteFlag,
                Items = items
            };
        }

        public static StringFile Load(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return Load(stream);
        }

        public override string ToString()
        {
            return string.Format("Items.Count={0}", Items.Count());
        }

    }
}
