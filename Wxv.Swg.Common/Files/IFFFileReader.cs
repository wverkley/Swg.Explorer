using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public sealed class IFFFileReader : SWGFileReader<IFFFile>
    {
        private const string FormTag = "FORM";

        private void LoadFormNode(Stream stream, IFFFile.Node formNode)
        {
            var start = stream.Position;
            var children = new List<IFFFile.Node>();

            while (stream.Position < (start + formNode.Size - 4))
            {
                var tag = stream.ReadString(4);
                var size = stream.ReadInt32BE();

                //Console.WriteLine(tag + " " + size + " " + reader.BaseStream.Position);

                IFFFile.Node child;
                if (tag.Equals(FormTag))
                {
                    var type = stream.ReadString(4);
                    child = new IFFFile.Node
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
                    child = new IFFFile.Node
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

        public override IFFFile Load(Stream stream)
        {
            var tag = stream.ReadString(4);
            if (!tag.Equals(FormTag))
                throw new IOException("IFF File does not contain valid FORM data");
            var size = stream.ReadInt32BE();
            var type = Encoding.ASCII.GetString(stream.ReadBytes(4));

            var root = new IFFFile.Node
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
    }
}
