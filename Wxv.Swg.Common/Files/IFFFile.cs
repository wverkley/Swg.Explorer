using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public sealed class IFFFile : ISWGFile
    {
        public enum NodeType
        {
            Form = 0,
            Record = 1
        }

        public sealed class Node
        {
            private static readonly byte[] EmptyData = { };
            private static readonly Node[] EmptyChildren = { };

            public NodeType NodeType { get; internal set; }
            public Node Parent { get; internal set; }
            public string Type { get; internal set; }
            public int Size { get; internal set; }
            public byte[] Data { get; internal set; }
            public IEnumerable<Node> Children { get; internal set; }

            internal Node()
            {
                Data = EmptyData;
                Children = EmptyChildren;
            }

            public override string ToString()
            {
                return Type;
            }

            public void ToString(TextWriter writer, int indent)
            {
                writer.Write("{0}{1} : {2}", new string(' ', indent), Type, Size);
                if (NodeType == NodeType.Record && Data.IsText())
                    writer.Write(" : " + Data.ReadString());
                writer.WriteLine();
                foreach (var child in Children)
                    child.ToString(writer, indent + 1);
            }
        }

        public Node Root { get; internal set; }

        internal IFFFile() { }

        public override string ToString()
        {
            return Root.ToString();
        }

        public void ToString(TextWriter writer)
        {
            Root.ToString(writer, 0);
        }
    }
}
