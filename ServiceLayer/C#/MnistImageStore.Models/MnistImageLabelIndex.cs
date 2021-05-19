using System;
using System.Collections.Generic;
using System.Text;

namespace MnistImageStore.Models
{
    /// <summary>
    /// An index for MNIST images, grouping and storing them by label.
    /// </summary>
    /// <remarks>Main purpose of this class is to avoid putting a generic object like a Dictionary into the dependency injection service collection (and therefore potentially preclude that same generic object from being used for another purpose elsewhere in the application).</remarks>
    public class MnistImageLabelIndex
    {
        /// <summary>
        /// A dictionary containing the index of MNIST images.  The key contains the label of the images, and the value contains a list of the index numbers of the images with that label.
        /// </summary>
        protected Dictionary<Int32, IList<Int32>> index;

        /// <summary>
        /// Gets the list of indices of MNIST images with the specified label.
        /// </summary>
        /// <param name="label">The label of the image indcies to get.</param>
        /// <returns>The list of indices of MNIST images with the specified label.</returns>
        public IList<Int32> this[Int32 label]
        {
            get
            {
                if (label < 0 || label > 9)
                    throw new IndexOutOfRangeException($"Invalid index {label}.  Indexer '{nameof(label)}' must be between 0 and 9 inclusive.");
                if (index.ContainsKey(label) == false)
                    throw new ArgumentException($"No images exist with label {label}.");

                return index[label];
            }
        }

        /// <summary>
        /// Initialises a new instance of the MnistImageStore.Models.MnistImageLabelIndex class.
        /// </summary>
        /// <param name="images">A collection of images to store in the index.</param>
        public MnistImageLabelIndex(IEnumerable<MnistImage> images)
        {
            index = new Dictionary<Int32, IList<Int32>>();

            Int32 currentImageIndex = 0;
            foreach (MnistImage currentImage in images)
            {
                if (index.ContainsKey(currentImage.Label) == false)
                {
                    index.Add(currentImage.Label, new List<Int32>());
                }
                index[currentImage.Label].Add(currentImageIndex);
                currentImageIndex++;
            }
        }

        /// <summary>
        /// Checks whether the index contains the specified label.
        /// </summary>
        /// <param name="label">The label to check for.</param>
        /// <returns>True if the label exists.  False otherwise.</returns>
        public Boolean ContainsLabel(Int32 label)
        {
            return index.ContainsKey(label);
        }
    }
}
