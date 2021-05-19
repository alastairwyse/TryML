using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MnistImageStore
{
    /// <summary>
    /// Configuration parameters and options for the MnistImageStore service (following the ASP.NET Core Options pattern).
    /// </summary>
    public class MnistImageStoreOptions
    {
        /// <summary>A URI containing the location of the MNIST data file.</summary>
        public String MnistImageDataFileUri { get; set; }

        /// <summary>A URI containing the location of the MNIST label file.</summary>
        public String MnistLabelFileUri { get; set; }

        /// <summary>Whether to show a developer-friendly HTML notification page in case of uncaught exception.</summary>
        public Boolean ShowDeveloperExceptionPage { get; set; }
    }
}
