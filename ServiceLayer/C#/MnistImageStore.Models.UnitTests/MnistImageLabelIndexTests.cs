using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MnistImageStore.Models;
using NUnit.Framework;

namespace MnistImageStore.Models.UnitTests
{
    /// <summary>
    /// Unit tests for the MnistImageStore.Models.MnistImageLabelIndex class.
    /// </summary>
    public class MnistImageLabelIndexTests
    {
        [Test]
        public void LabelIndexer_LabelOutOfRange()
        {
            var testMnistImageLabelIndex = new MnistImageLabelIndex(Enumerable.Empty<MnistImage>());

            var e = Assert.Throws<IndexOutOfRangeException>(delegate
            {
                var result = testMnistImageLabelIndex[-1];
            });

            Assert.That(e.Message, Does.StartWith("Invalid index -1.  Indexer 'label' must be between 0 and 9 inclusive."));


            e = Assert.Throws<IndexOutOfRangeException>(delegate
            {
                var result = testMnistImageLabelIndex[10];
            });

            Assert.That(e.Message, Does.StartWith("Invalid index 10.  Indexer 'label' must be between 0 and 9 inclusive."));
        }
    }
}
