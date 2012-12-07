using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public static class IFFExtentions
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
