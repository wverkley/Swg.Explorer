using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common.Files
{
    public class InteriorLayoutFile : ISWGFile
    {
        public class InteriorLayoutItem
        {
            public string Object { get; internal set; }
            public string Cell { get; internal set; }
            public Single[,] Matrix { get; internal set; }
            public Single[] W { get; internal set; }

            internal InteriorLayoutItem() { }

            public override string ToString()
            {
                return string.Format("Object={0}, Cell={1}", Object, Cell);
            }
        }

        public IEnumerable<InteriorLayoutItem> Items { get; internal set; }
        internal InteriorLayoutFile() { }

        public override string ToString()
        {
            return string.Format("Items.Count={0}", Items.Count());
        }
    }
}
