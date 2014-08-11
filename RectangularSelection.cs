using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penguin
{
    public class RectangularSelection : ISelection
    {
        private PenguinMap map;

        /// <summary>
        /// Rectangle's x coordinate
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Rectangle's y coordinate
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Rectangle's width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Rectangle's height
        /// </summary>
        public int Height { get; set; }

        public bool Contains(PenguinVector loc)
        {
            return loc.X >= X && loc.X < X + Width && loc.Y >= Y && loc.Y < Y + Height;
        }

        public IEnumerable<PenguinVector> GetSelectionArea()
        {
            throw new NotImplementedException();
        }

        public PenguinMap GetMap()
        {
            return map;
        }

        public RectangularSelection()
        {

        }

        public RectangularSelection(PenguinMap map, int x, int y, int width, int height)
        {
            this.map = map;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }


    }
}
