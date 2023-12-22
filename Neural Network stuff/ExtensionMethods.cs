using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBNeuralNetwork
{
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// Removes all null or empty entries from the IEnumerable.
        /// </summary>
        /// <param name="collection">The IEnumerable to remove the empty and null entries from.</param>
        /// <returns>The IEnumerable with all null or empty entries removed.</returns>
        public static IEnumerable<string> RemoveEmptyEntries(this IEnumerable<string> collection)
        {            
            return collection.Where(x => !string.IsNullOrEmpty(x));
        }
    }
}
