using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penguin
{
    public class PenguinMap
    {
        private PenguinBlock[,,] mapData;

        /// <summary>
        /// Total width of the world
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Total height of the world
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// World key used as a token to build blocks in the world
        /// </summary>
        public string WorldKey { get; set; }

        /// <summary>
        /// Calculates if block if within conditional bounds
        /// </summary>
        /// <param name="loc">Location to test</param>
        public bool IsValid(PenguinVector loc)
        {
            return loc.X >= 0 && loc.X < Width && loc.Y >= 0 && loc.Y < Height && loc.Z >= 0 && loc.Z < 2;
        }

        /// <summary>
        /// Gets a block at a specified location
        /// </summary>
        /// <param name="loc"></param>
        /// <returns></returns>
        public PenguinBlock GetBlockAt(PenguinVector loc)
        {
            if (!IsValid(loc))
            {
                return null;
            }

            return mapData[loc.X, loc.Y, loc.Z];
        }


        public void AddBlock(PenguinBlock block)
        {
            mapData[block.X, block.Y, block.Z] = block;
        }

        public PenguinMap(int width, int height)
        {
            Width = width;
            Height = height;

            mapData = new PenguinBlock[width, height, 2];
        }

        public PenguinVector this[int i]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
