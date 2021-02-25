using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using MnistImageStore.Models;

namespace MnistImageStore.Persistence
{
    /// <summary>
    /// Reads MnistImage objects from files in the native MNIST format (specified at http://yann.lecun.com/exdb/mnist/).
    /// </summary>
    public class MnistImageNativeFormatFileReader : IMnistImageReader
    {
        /// <summary>The full path to the file containing the image data.</summary>
        protected String imageDataFilePath;
        /// <summary>The full path to the file containing the label data.</summary>
        protected String labelFilePath;

        /// <summary>
        /// Initialises a new instance of the MnistImageStore.Persistence.MnistImageNativeFormatFileReader class.
        /// </summary>
        /// <param name="imageDataFilePath">The full path to the file containing the image data.</param>
        /// <param name="labelFilePath">The full path to the file containing the label data.</param>
        public MnistImageNativeFormatFileReader(String imageDataFilePath, String labelFilePath)
        {
            if (String.IsNullOrWhiteSpace(imageDataFilePath) == true)
                throw new ArgumentException($"Parameter '{nameof(imageDataFilePath)}' cannot be null or blank.", nameof(imageDataFilePath));
            if (String.IsNullOrWhiteSpace(labelFilePath) == true)
                throw new ArgumentException($"Parameter '{nameof(labelFilePath)}' cannot be null or blank.", nameof(labelFilePath));

            this.imageDataFilePath = imageDataFilePath;
            this.labelFilePath = labelFilePath;
        }

        /// <include file='InterfaceDocumentationComments.xml' path='doc/members/member[@name="M:MnistImageStore.Persistence.IMnistImageReader.Read"]/*'/>
        public IEnumerable<MnistImage> Read()
        {
            using (FileStream imageDataFileStream = new FileStream(imageDataFilePath, FileMode.Open))
            using (FileStream labelFileStream = new FileStream(labelFilePath, FileMode.Open))
            using (BinaryReader imageDataReader = new BinaryReader(imageDataFileStream))
            using (BinaryReader labelReader = new BinaryReader(labelFileStream))
            {
                // Read the header information from each file
                Int32 imageCount = 0, labelCount = 0, imageRowPixelCount = 0, imageColumnPixelCount = 0;
                try
                {
                    // Consume the 'magic number'
                    imageDataReader.ReadInt32();
                    labelReader.ReadInt32();
                    // Read the item counts
                    imageCount = ConvertByteOrderFromBigEndian(imageDataReader.ReadInt32());
                    labelCount = ConvertByteOrderFromBigEndian(labelReader.ReadInt32());
                    // Read the dimensions of the images
                    imageRowPixelCount = ConvertByteOrderFromBigEndian(imageDataReader.ReadInt32());
                    imageColumnPixelCount = ConvertByteOrderFromBigEndian(imageDataReader.ReadInt32());
                }
                catch (Exception e)
                {
                    throw new Exception($"Error reading header information from image data file '{imageDataFilePath}'.", e);
                }

                if (imageCount != labelCount)
                    throw new Exception($"Number of images ({imageCount}) differs from number of labels ({labelCount}) in image file '{imageDataFilePath}' and label file '{labelFilePath}'.");
                if (imageCount < 0)
                    throw new Exception($"Number of images listed in header information of file '{imageDataFilePath}' is negative ({imageCount}).");
                if (imageRowPixelCount < 1)
                    throw new Exception($"Number of row pixels listed in header information of file '{imageDataFilePath}' is less than 1 ({imageRowPixelCount}).");
                if (imageColumnPixelCount < 1)
                    throw new Exception($"Number of column pixels listed in header information of file '{imageDataFilePath}' is less than 1 ({imageColumnPixelCount}).");

                Int32 imageAndLabelReadCount = 0;
                while (imageAndLabelReadCount < imageCount)
                {
                    // Read the next image data
                    var nextImageData = new Byte[imageRowPixelCount, imageColumnPixelCount];
                    try
                    {
                        for (Int32 currentRowIndex = 0; currentRowIndex < imageRowPixelCount; currentRowIndex++)
                        {
                            Byte[] currentRowData = imageDataReader.ReadBytes(imageRowPixelCount);
                            Int32 destinationOffset = currentRowIndex * imageRowPixelCount;
                            Buffer.BlockCopy(currentRowData, 0, nextImageData, destinationOffset, imageRowPixelCount);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to read MNIST image with index {imageAndLabelReadCount} from file '{imageDataFilePath}'.", e);
                    }

                    // Read the next label
                    Int32 nextLabel;
                    try
                    {
                        Byte labelAsByte = labelReader.ReadByte();
                        nextLabel = labelAsByte;
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to read MNIST label with index {imageAndLabelReadCount} from file '{labelFilePath}'.", e);
                    }

                    var nextMnistImage = new MnistImage(nextImageData, nextLabel);
                    yield return nextMnistImage;

                    imageAndLabelReadCount++;
                }
            }
        }

        # region Private/Protected Methods

        /// <summary>
        /// Converts the byte order (endianness) of the specified Int32 assuming it is big-endian.
        /// </summary>
        /// <param name="inputInteger">The big-endian Int32 to convert.</param>
        /// <returns>The Int32 converted to be valid for the endianness of the current CPU.</returns>
        /// <remarks>Int32s in the MINST file headers are encoded as big-endian.</remarks>
        protected Int32 ConvertByteOrderFromBigEndian(Int32 inputInteger)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                Byte[] integerBytes = BitConverter.GetBytes(inputInteger);
                Array.Reverse(integerBytes);

                return BitConverter.ToInt32(integerBytes, 0);
            }
            else
            {
                return inputInteger;
            }
        }

        #endregion
    }
}
