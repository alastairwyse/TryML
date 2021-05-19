using System;
using System.Collections.Generic;
using System.Text;
using MnistImageStore.Models;
using NUnit.Framework;

namespace MnistImageStore.Models.UnitTests
{
    /// <summary>
    /// Unit tests for the MnistImageStore.Models.MnistImage class.
    /// </summary>
    public class MnistImageTests
    {
        [Test]
        public void Constructor_LabelOutOfRange()
        {
            var e = Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                var testMnistImage = new MnistImage(new Byte[0, 0], -1);
            });

            Assert.That(e.Message, Does.StartWith("Parameter 'label' must be between 0 and 9 inclusive."));
            Assert.AreEqual("label", e.ParamName);


            e = Assert.Throws<ArgumentOutOfRangeException>(delegate
            {
                var testMnistImage = new MnistImage(new Byte[0, 0], 10);
            });

            Assert.That(e.Message, Does.StartWith("Parameter 'label' must be between 0 and 9 inclusive."));
            Assert.AreEqual("label", e.ParamName);
        }

        [Test]
        public void Constructor_ImageDataNotSquareMatrix()
        {
            var imageData = new Byte[,]
            {
                { 0, 0, 0 },
                { 0, 0, 0 },
            };

            var e = Assert.Throws<ArgumentException>(delegate
            {
                var testMnistImage = new MnistImage(imageData, 0);
            });

            Assert.That(e.Message, Does.StartWith("Parameter 'imageData' must contain equal dimensions (i.e. be a square 2-dimensional matrix)."));
            Assert.AreEqual("imageData", e.ParamName);
        }
    }
}
