﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace TryML.Utilities.ExceptionHandling.UnitTests
{
    /// <summary>
    /// Unit tests for the TryML.Utilities.ExceptionHandling.ExceptionToHttpInternalServerErrorConverter class.
    /// </summary>
    public class ExceptionToHttpInternalServerErrorConverterUnitTests
    {
        private ExceptionToHttpInternalServerErrorConverter testExceptionToHttpInternalServerErrorConverter;

        [SetUp]
        protected void SetUp()
        {
            testExceptionToHttpInternalServerErrorConverter = new ExceptionToHttpInternalServerErrorConverter();
        }

        [Test]
        public void AddConversionFunction_ExceptionTypeParameterNotException()
        {
            var e = Assert.Throws<ArgumentException>(delegate
            {
                testExceptionToHttpInternalServerErrorConverter.AddConversionFunction(typeof(StringBuilder), (Exception exception) => { return new HttpInternalServerError("code", "message"); });
            });

            Assert.That(e.Message, Does.StartWith("Type 'System.Text.StringBuilder' specified in parameter 'exceptionType' is not assignable to 'System.Exception'."));
            Assert.AreEqual("exceptionType", e.ParamName);
        }

        [Test]
        public void AddConversionFunction()
        {
            Func<Exception, HttpInternalServerError> exceptionHandler = (Exception exception) =>
            {
                return new HttpInternalServerError("CustomCode", "CustomeMessage");
            };

            testExceptionToHttpInternalServerErrorConverter.AddConversionFunction(typeof(Exception), exceptionHandler);

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(new Exception("Fake Exception"));
            Assert.AreEqual("CustomCode", result.Code);
            Assert.AreEqual("CustomeMessage", result.Message);
            Assert.IsNull(result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_Exception()
        {
            // Need to actually throw the test exception otherwise the 'TargetSite' property is not set.
            Exception testException = null;
            try
            {
                throw new Exception("Test exception message.");
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("Exception", result.Code);
            Assert.AreEqual("Test exception message.", result.Message);
            Assert.AreEqual("Convert_Exception", result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_UnthrownException()
        {
            // Unlikely to encounter this case in the real world, but tests that the 'TargetSite' property of the Exception is not included when null (i.e. when the Exception is just 'newed' rather th than being thrown).
            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(new Exception("Test exception message."));

            Assert.AreEqual("Exception", result.Code);
            Assert.AreEqual("Test exception message.", result.Message);
            Assert.IsNull(result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_ArgumentException()
        {
            Exception testException = null;
            try
            {
                throw new ArgumentException("Test argument exception message.", "TestArgumentName");
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("ArgumentException", result.Code);
            Assert.AreEqual("Test argument exception message. (Parameter 'TestArgumentName')", result.Message);
            Assert.AreEqual("Convert_ArgumentException", result.Target);
            Assert.AreEqual(1, result.Attributes.Count());
            var attributes = new List<Tuple<String, String>>(result.Attributes);
            Assert.AreEqual("ParameterName", attributes[0].Item1);
            Assert.AreEqual("TestArgumentName", attributes[0].Item2);
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_ArgumentOutOfRangeException()
        {
            Exception testException = null;
            try
            {
                throw new ArgumentOutOfRangeException("TestArgumentName", "Test argument out of range exception message.");
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("ArgumentOutOfRangeException", result.Code);
            Assert.AreEqual("Test argument out of range exception message. (Parameter 'TestArgumentName')", result.Message);
            Assert.AreEqual("Convert_ArgumentOutOfRangeException", result.Target);
            Assert.AreEqual(1, result.Attributes.Count());
            var attributes = new List<Tuple<String, String>>(result.Attributes);
            Assert.AreEqual("ParameterName", attributes[0].Item1);
            Assert.AreEqual("TestArgumentName", attributes[0].Item2);
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_ArgumentNullException()
        {
            Exception testException = null;
            try
            {
                throw new ArgumentNullException("TestArgumentName", "Test argument null exception message.");
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("ArgumentNullException", result.Code);
            Assert.AreEqual("Test argument null exception message. (Parameter 'TestArgumentName')", result.Message);
            Assert.AreEqual("Convert_ArgumentNullException", result.Target);
            Assert.AreEqual(1, result.Attributes.Count());
            var attributes = new List<Tuple<String, String>>(result.Attributes);
            Assert.AreEqual("ParameterName", attributes[0].Item1);
            Assert.AreEqual("TestArgumentName", attributes[0].Item2);
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_IndexOutOfRangeException()
        {
            Exception testException = null;
            try
            {
                throw new IndexOutOfRangeException("Test index out of range exception message.");
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("IndexOutOfRangeException", result.Code);
            Assert.AreEqual("Test index out of range exception message.", result.Message);
            Assert.AreEqual("Convert_IndexOutOfRangeException", result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_InnerExceptionStack()
        {
            Exception testException = null;
            try
            {
                try
                {
                    try
                    {
                        throw new IndexOutOfRangeException("Test index out of range exception message.");
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("Test argument exception message.", "testParameterName", e); ;
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Outermost exception.", e); ;
                }
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("Exception", result.Code);
            Assert.AreEqual("Outermost exception.", result.Message);
            Assert.AreEqual("Convert_InnerExceptionStack", result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNotNull(result.InnerError);
            Assert.AreEqual("ArgumentException", result.InnerError.Code);
            Assert.AreEqual("Test argument exception message. (Parameter 'testParameterName')", result.InnerError.Message);
            Assert.AreEqual("Convert_InnerExceptionStack", result.InnerError.Target);
            Assert.AreEqual(1, result.InnerError.Attributes.Count());
            var attributes = new List<Tuple<String, String>>(result.InnerError.Attributes);
            Assert.AreEqual("ParameterName", attributes[0].Item1);
            Assert.AreEqual("testParameterName", attributes[0].Item2);
            Assert.IsNotNull(result.InnerError.InnerError);
            Assert.AreEqual("IndexOutOfRangeException", result.InnerError.InnerError.Code);
            Assert.AreEqual("Test index out of range exception message.", result.InnerError.InnerError.Message);
            Assert.AreEqual("Convert_InnerExceptionStack", result.InnerError.InnerError.Target);
            Assert.AreEqual(0, result.InnerError.InnerError.Attributes.Count());
            Assert.IsNull(result.InnerError.InnerError.InnerError);
        }

        [Test]
        public void Convert_GenericException()
        {
            Func<Exception, HttpInternalServerError> genericExceptionConversionFunction = (Exception exception) =>
            {
                var genericException = (GenericException<Int32>)exception;
                if (exception.TargetSite == null)
                {
                    return new HttpInternalServerError
                    (
                        exception.GetType().Name,
                        exception.Message,
                        new List<Tuple<String, String>>()
                        {
                        new Tuple<String, String>("Int32 Value", genericException.GenericParameter.ToString())
                        }
                    );
                }
                else
                {
                    return new HttpInternalServerError
                    (
                        exception.GetType().Name,
                        exception.Message,
                        exception.TargetSite.Name,
                        new List<Tuple<String, String>>()
                        {
                        new Tuple<String, String>("Int32 Value", genericException.GenericParameter.ToString())
                        }
                    );
                }
            };
            testExceptionToHttpInternalServerErrorConverter.AddConversionFunction(typeof(GenericException<Int32>), genericExceptionConversionFunction);

            Exception testException = null;
            try
            {
                throw new GenericException<Int32>("Test generic string type exception message.", 123);
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("GenericException`1", result.Code);
            Assert.AreEqual("Test generic string type exception message.", result.Message);
            Assert.AreEqual("Convert_GenericException", result.Target);
            Assert.AreEqual(1, result.Attributes.Count());
            var attributes = new List<Tuple<String, String>>(result.Attributes);
            Assert.AreEqual("Int32 Value", attributes[0].Item1);
            Assert.AreEqual("123", attributes[0].Item2);
            Assert.IsNull(result.InnerError);


            // Test that the conversion function for plain Exceptions is used if the generic type doesn't match
            try
            {
                throw new GenericException<Char>("Test generic char type exception message.", 'A');
            }
            catch (Exception e)
            {
                testException = e;
            }

            result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("GenericException`1", result.Code);
            Assert.AreEqual("Test generic char type exception message.", result.Message);
            Assert.AreEqual("Convert_GenericException", result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_NoConversionFunctionDefined()
        {
            Exception testException = null;
            try
            {
                throw new DerivedException("Test derived exception message.", 123);
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("DerivedException", result.Code);
            Assert.AreEqual("Test derived exception message.", result.Message);
            Assert.AreEqual("Convert_NoConversionFunctionDefined", result.Target);
            Assert.AreEqual(0, result.Attributes.Count());
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_ConversionFunctionDefinedForBaseClass()
        {
            Func<Exception, HttpInternalServerError> derivedExceptionConversionFunction = (Exception exception) =>
            {
                var derivedException = (DerivedException)exception;
                if (exception.TargetSite == null)
                {
                    return new HttpInternalServerError
                    (
                        exception.GetType().Name,
                        exception.Message,
                        new List<Tuple<String, String>>()
                        {
                        new Tuple<String, String>(nameof(derivedException.NumericProperty), derivedException.NumericProperty.ToString())
                        }
                    );
                }
                else
                {
                    return new HttpInternalServerError
                    (
                        exception.GetType().Name,
                        exception.Message,
                        exception.TargetSite.Name,
                        new List<Tuple<String, String>>()
                        {
                        new Tuple<String, String>(nameof(derivedException.NumericProperty), derivedException.NumericProperty.ToString())
                        }
                    );
                }
            };
            testExceptionToHttpInternalServerErrorConverter.AddConversionFunction(typeof(DerivedException), derivedExceptionConversionFunction);

            Exception testException = null;
            try
            {
                throw new SecondLevelDerivedException("Second level derived exception message.", 456, 'B');
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("SecondLevelDerivedException", result.Code);
            Assert.AreEqual("Second level derived exception message.", result.Message);
            Assert.AreEqual("Convert_ConversionFunctionDefinedForBaseClass", result.Target);
            Assert.AreEqual(1, result.Attributes.Count());
            var attributes = new List<Tuple<String, String>>(result.Attributes);
            Assert.AreEqual("NumericProperty", attributes[0].Item1);
            Assert.AreEqual("456", attributes[0].Item2);
            Assert.IsNull(result.InnerError);
        }

        [Test]
        public void Convert_AggregateException()
        {
            var innerException1 = new Exception("Plain exception inner exception.");
            var innerException2 = new ArgumentException("Argument exception inner exception.", "ArgumentExceptionParameterName");
            var innerException3 = new ArgumentNullException("ArgumentNullExceptionParameterName", "Argument null exception inner exception.");
            Exception testException = null;
            try
            {
                throw new AggregateException
                (
                    "Test aggreate exception message.",
                    new List<Exception>()
                    { innerException1, innerException2, innerException3 }
                );
            }
            catch (Exception e)
            {
                testException = e;
            }

            HttpInternalServerError result = testExceptionToHttpInternalServerErrorConverter.Convert(testException);

            Assert.AreEqual("AggregateException", result.Code);
            Assert.AreEqual(testException.Message, result.Message);
            Assert.AreEqual("Convert_AggregateException", result.Target);
            Assert.AreEqual(6, result.Attributes.Count()); 
            var attributes = new List<Tuple<String, String>>(result.Attributes);
            Assert.AreEqual("InnerException1Code", attributes[0].Item1);
            Assert.AreEqual("Exception", attributes[0].Item2);
            Assert.AreEqual("InnerException1Message", attributes[1].Item1);
            Assert.AreEqual("Plain exception inner exception.", attributes[1].Item2);
            Assert.AreEqual("InnerException2Code", attributes[2].Item1);
            Assert.AreEqual("ArgumentException", attributes[2].Item2);
            Assert.AreEqual("InnerException2Message", attributes[3].Item1);
            Assert.AreEqual("Argument exception inner exception. (Parameter 'ArgumentExceptionParameterName')", attributes[3].Item2);
            Assert.AreEqual("InnerException3Code", attributes[4].Item1);
            Assert.AreEqual("ArgumentNullException", attributes[4].Item2);
            Assert.AreEqual("InnerException3Message", attributes[5].Item1);
            Assert.AreEqual("Argument null exception inner exception. (Parameter 'ArgumentNullExceptionParameterName')", attributes[5].Item2);
            Assert.IsNotNull(result.InnerError);
            Assert.AreEqual("Exception", result.InnerError.Code);
            Assert.AreEqual("Plain exception inner exception.", result.InnerError.Message);
            Assert.AreEqual(0, result.InnerError.Attributes.Count());
            Assert.IsNull(result.InnerError.InnerError);
        }

        #region Nested Classes

        private class DerivedException : Exception
        {
            protected Int32 numericProperty;

            public Int32 NumericProperty
            {
                get { return numericProperty; }
            }

            public DerivedException(String message, Int32 numericProperty)
                : base(message)
            {
                this.numericProperty = numericProperty;
            }
        }

        private class SecondLevelDerivedException : DerivedException
        {
            protected Char charProperty;

            public Char CharProperty
            {
                get { return charProperty; }
            }

            public SecondLevelDerivedException(String message, Int32 numericProperty, Char charProperty)
                : base(message, numericProperty)
            {
                this.charProperty = charProperty;
            }
        }

        private class GenericException<T> : Exception
        {
            protected T genericParameter;

            public T GenericParameter
            {
                get { return genericParameter; }
            }

            public GenericException(T genericParameter)
                : base()
            {
                this.genericParameter = genericParameter;
            }

            public GenericException(String message, T genericParameter)
                : base(message)
            {
                this.genericParameter = genericParameter;
            }

            public GenericException()
                :base()
            {

            }
        }

        #endregion
    }
}
