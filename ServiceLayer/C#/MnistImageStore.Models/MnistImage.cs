using System;
using System.ComponentModel;

namespace MnistImageStore.Models
{
    /// <summary>
    /// Pixel data of a handwritten digit from the MNIST database (http://yann.lecun.com/exdb/mnist/).
    /// </summary>
    public class MnistImage
    {
        protected Byte[,] imageData;
        protected Int32 label;

        /// <summary>
        /// The pixel data of the image repesented as a square matrix of bytes, where each byte represents the greyscale brightness of that pixel.
        /// </summary>
        public Byte[,] ImageData
        {
            get { return imageData; }
        }

        /// <summary>
        /// The number represented by the image.
        /// </summary>
        public Int32 Label
        {
            get { return label; }
        }

        /// <summary>
        /// Initialises a new instance of the MnistImageStore.Models.MnistImage class.
        /// </summary>
        /// <param name="imageData">The pixel data of the image repesented as a square matrix of bytes, where each byte represents the greyscale brightness of that pixel.</param>
        /// <param name="label">The number represented by the image.</param>
        public MnistImage(Byte[,] imageData, Int32 label)
        {
            if (imageData.GetLength(0) != imageData.GetLength(1))
                throw new ArgumentException($"Parameter '{imageData}' must contain equal dimensions (i.e. be a square 2-dimensional matrix).", nameof(imageData));

            this.imageData = imageData;
            this.label = label;
        }
    }
}
