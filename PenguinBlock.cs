using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penguin
{
    public class PenguinBlock
    {
        public PenguinVector Location { get; set; }

        public int X { get { return Location.X; } set { Location.X = value; } }

        public int Y { get; set; }

        public int Z { get; set; }

        public int ID { get; set; }

        public PenguinBlock(int x, int y, int z, int id)
        {
            X = x;
            Y = y;
            Z = z;
            ID = id;
        }
    }
}
