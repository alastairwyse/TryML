using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace TryML.Utilities.ExceptionHandling.UnitTests
{
    /// <summary>
    /// Unit tests for the TryML.Utilities.ExceptionHandling.ExceptionToHttpStatusCodeConverter class.
    /// </summary>
    public class ExceptionToHttpStatusCodeConverterUnitTests
    {
        private ExceptionToHttpStatusCodeConverter testExceptionToHttpStatusCodeConverter;

        [SetUp]
        protected void SetUp()
        {
            testExceptionToHttpStatusCodeConverter = new ExceptionToHttpStatusCodeConverter();
        }

        [Test]
        public void AddMapping_ExceptionTypeParameterNotException()
        {
            var e = Assert.Throws<ArgumentException>(delegate
            {
                testExceptionToHttpStatusCodeConverter.AddMapping(typeof(StringBuilder), HttpStatusCode.Unauthorized);
            });

            Assert.That(e.Message, Does.StartWith("Type 'System.Text.StringBuilder' specified in parameter 'exceptionType' is not assignable to 'System.Exception'."));
            Assert.AreEqual("exceptionType", e.ParamName);
        }

        [Test]
        public void AddMapping()
        {
            testExceptionToHttpStatusCodeConverter.AddMapping(typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized);
            testExceptionToHttpStatusCodeConverter.AddMapping(typeof(Exception), HttpStatusCode.NotFound);

            HttpStatusCode result = testExceptionToHttpStatusCodeConverter.Convert(new UnauthorizedAccessException());

            Assert.AreEqual(HttpStatusCode.Unauthorized, result);


            result = testExceptionToHttpStatusCodeConverter.Convert(new IOException());

            Assert.AreEqual(HttpStatusCode.NotFound, result);


            result = testExceptionToHttpStatusCodeConverter.Convert(new Exception());

            Assert.AreEqual(HttpStatusCode.NotFound, result);
        }

        [Test]
        public void Convert_ArgumentException()
        {
            HttpStatusCode result = testExceptionToHttpStatusCodeConverter.Convert(new ArgumentException());

            Assert.AreEqual(HttpStatusCode.BadRequest, result);
        }
        [Test]
        public void Convert_Exception()
        {
            HttpStatusCode result = testExceptionToHttpStatusCodeConverter.Convert(new Exception());

            Assert.AreEqual(HttpStatusCode.InternalServerError, result);
        }

        [Test]
        public void Convert_ExceptionTypeParameterIsDerivedClass()
        {
            HttpStatusCode result = testExceptionToHttpStatusCodeConverter.Convert(new ArgumentOutOfRangeException());

            Assert.AreEqual(HttpStatusCode.BadRequest, result);


            result = testExceptionToHttpStatusCodeConverter.Convert(new IOException());

            Assert.AreEqual(HttpStatusCode.InternalServerError, result);
        }
    }
}
