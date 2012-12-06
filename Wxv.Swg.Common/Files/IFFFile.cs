using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public sealed class IFFFile
    {
        private const string FormTag = "FORM";

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

        internal IFFFile()
        {
        }

        public override string ToString()
        {
            return Root.ToString();
        }

        public void ToString(TextWriter writer)
        {
            Root.ToString(writer, 0);
        }

        private static void LoadFormNode(Stream stream, Node formNode)
        {
            var start = stream.Position;
            var children = new List<Node>();

            while (stream.Position < (start + formNode.Size - 4))
            {
                var tag = stream.ReadString(4);
                var size = stream.ReadInt32BE();

                //Console.WriteLine(tag + " " + size + " " + reader.BaseStream.Position);

                Node child;
                if (tag.Equals(FormTag))
                {
                    var type = stream.ReadString(4);
                    child = new Node
                    {
                        NodeType = IFFFile.NodeType.Form,
                        Parent = formNode,
                        Type = type,
                        Size = size
                    };
                    LoadFormNode(stream, child);
                }
                else
                {
                    child = new Node
                    {
                        NodeType = IFFFile.NodeType.Record,
                        Parent = formNode,
                        Type = tag,
                        Size = size,
                        Data = stream.ReadBytes(size)
                    };
                }

                children.Add(child);
            }

            formNode.Children = children.ToArray();
        }

        public static IFFFile Load(Stream stream)
        {
            var tag = stream.ReadString(4);
            if (!tag.Equals(FormTag))
                throw new IOException("IFF File does not contain valid FORM data");
            var size = stream.ReadInt32BE();
            var type = Encoding.ASCII.GetString(stream.ReadBytes(4));

            var root = new Node
            {
                NodeType = IFFFile.NodeType.Form,
                Parent = null,
                Type = type,
                Size = size
            };
            LoadFormNode(stream, root);

            return new IFFFile
            {
                Root = root
            };
        }

        public static IFFFile Load(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
                return Load(stream);
        }

        public static IFFFile Load(byte[] data)
        {
            using (var stream = new MemoryStream(data))
                return Load(stream);
        }
    }

    public static class IFFFileExtentions
    {
        public static IEnumerable<IFFFile.Node> Children(this IEnumerable<IFFFile.Node> nodes, string type = null)
        {
            foreach (var node in nodes)
            {
                foreach (var child in node.Children(type))
                {
                    yield return child;
                }
            }
        }

        public static IEnumerable<IFFFile.Node> Children(this IFFFile.Node node, string type = null)
        {
            foreach (var child in node.Children)
            {
                if (type == null || string.Equals(type, child.Type, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;
            }
        }

        public static IEnumerable<IFFFile.Node> Descendents(this IEnumerable<IFFFile.Node> nodes, string type = null)
        {
            foreach (var node in nodes)
            {
                foreach (var descendent in node.Descendents(type))
                {
                    yield return descendent;
                }
            }
        }

        public static IEnumerable<IFFFile.Node> Descendents(this IFFFile.Node node, string type = null)
        {
            foreach (var child in node.Children)
            {
                if (type == null || string.Equals(type, child.Type, StringComparison.InvariantCultureIgnoreCase))
                    yield return child;

                foreach (var descendent in child.Descendents(type))
                {
                    yield return descendent;
                }
            }
        }
    }
}
