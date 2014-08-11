using System.Collections.Generic;

namespace Penguin
{
    /// <summary>
    /// Selection requirements for different selections
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Calculates if location is within selection bounds
        /// </summary>
        /// <param name="loc">Location to calculate</param>
        public bool Contains(PenguinVector loc);

        /// <summary>
        /// Gets the selection area
        /// </summary>
        public IEnumerable<PenguinVector> GetSelectionArea();

        /// <summary>
        /// Gets the everybody edits map
        /// </summary>
        /// <returns></returns>
        public PenguinMap GetMap();
    }
}
