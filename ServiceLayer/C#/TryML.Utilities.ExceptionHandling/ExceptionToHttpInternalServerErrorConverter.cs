using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TryML.Utilities.ExceptionHandling
{
    /// <summary>
    /// Converts an Exception (or object derived from Exception) to a HttpInternalServerError object.
    /// </summary>
    public class ExceptionToHttpInternalServerErrorConverter
    {
        /// <summary>Maps a type (assignable to Exception) to a conversion function which converts that type to an HttpInternalServerError.</summary>
        protected Dictionary<Type, Func<Exception, HttpInternalServerError>> typeToConversionFunctionMap;

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.ExceptionToHttpInternalServerErrorConverter class.
        /// </summary>
        public ExceptionToHttpInternalServerErrorConverter()
        {
            typeToConversionFunctionMap = new Dictionary<Type, Func<Exception, HttpInternalServerError>>();
            InitialiseTypeToConversionFunctionMap(typeToConversionFunctionMap);
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.ExceptionToHttpInternalServerErrorConverter class.
        /// </summary>
        /// <param name="typesAndConversionFunctions">A collection of tuples containing 2 values: A type (assignable to Exception) that the conversion function in the second component converts, and the conversion function which accepts an Exception object and returns a HttpInternalServerError.</param>
        /// <remarks>Note that the conversion functions should not handle the exception's 'InnerException' property, nor assign to the HttpInternalServerError's 'InnerError' property.</remarks>
        public ExceptionToHttpInternalServerErrorConverter(IEnumerable<Tuple<Type, Func<Exception, HttpInternalServerError>>> typesAndConversionFunctions)
            : base()
        {
            foreach (Tuple<Type, Func<Exception, HttpInternalServerError>> currentTypeAndConversionFunction in typesAndConversionFunctions)
            {
                AddConversionFunction(currentTypeAndConversionFunction.Item1, currentTypeAndConversionFunction.Item2);
            }
        }

        /// <summary>
        /// Adds a conversion function to the converter.
        /// </summary>
        /// <param name="exceptionType">The type (assignable to Exception) that the conversion function converts.</param>
        /// <param name="conversionFunction">The conversion function.  Accepts an Exception object and returns a HttpInternalServerError.</param>
        /// <remarks>Note that the conversion function should not handle the exception's 'InnerException' property, nor assign to the HttpInternalServerError's 'InnerError' property.</remarks>
        public void AddConversionFunction(Type exceptionType, Func<Exception, HttpInternalServerError> conversionFunction)
        {
            if (typeof(Exception).IsAssignableFrom(exceptionType) == false)
                throw new ArgumentException($"Type '{exceptionType.FullName}' specified in parameter '{nameof(exceptionType)}' is not assignable to '{typeof(Exception).FullName}'.", nameof(exceptionType));

            if (typeToConversionFunctionMap.ContainsKey(exceptionType) == true)
            {
                typeToConversionFunctionMap.Remove(exceptionType);
            }
            typeToConversionFunctionMap.Add(exceptionType, conversionFunction);
        }

        /// <summary>
        /// Converts the specified exception to an HttpInternalServerError.
        /// </summary>
        /// <param name="exception">The exception to convert.</param>
        /// <returns>The exception converted to an HttpInternalServerError.</returns>
        public HttpInternalServerError Convert(Exception exception)
        {
            return ConvertExceptionRecurse(exception);
        }

        /// <summary>
        /// Initialises the specified type to conversion function map with conversion functions for many of the common .NET exceptions.
        /// </summary>
        /// <param name="typeToConversionFunctionMap">The type to conversion map to initialise.</param>
        protected void InitialiseTypeToConversionFunctionMap(Dictionary<Type, Func<Exception, HttpInternalServerError>> typeToConversionFunctionMap)
        {
            typeToConversionFunctionMap.Add
            (
                typeof(Exception), 
                (Exception exception) =>
                {
                    if (exception.TargetSite == null)
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message);
                    }
                    else
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, exception.TargetSite.Name);
                    }
                }
            );
            typeToConversionFunctionMap.Add
            (
                typeof(ArgumentException),
                (Exception exception) =>
                {
                    var argumentException = (ArgumentException)exception;
                    var attributes = new List<Tuple<String, String>>()
                    {
                        new Tuple<String, String>("ParameterName", $"{argumentException.ParamName}")
                    };
                    if (exception.TargetSite == null)
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, attributes);
                    }
                    else
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, exception.TargetSite.Name, attributes);
                    }
                }
            );
            typeToConversionFunctionMap.Add
            (
                typeof(ArgumentOutOfRangeException),
                (Exception exception) =>
                {
                    var argumentOutOfRangeException = (ArgumentOutOfRangeException)exception;
                    var attributes = new List<Tuple<String, String>>()
                    {
                        new Tuple<String, String>("ParameterName", $"{argumentOutOfRangeException.ParamName}")
                    };
                    if (exception.TargetSite == null)
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, attributes);
                    }
                    else
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, exception.TargetSite.Name, attributes);
                    }
                }
            );
            typeToConversionFunctionMap.Add
            (
                typeof(ArgumentNullException),
                (Exception exception) =>
                {
                    var argumentNullException = (ArgumentNullException)exception;
                    var attributes = new List<Tuple<String, String>>()
                    {
                        new Tuple<String, String>("ParameterName", $"{argumentNullException.ParamName}")
                    };
                    if (exception.TargetSite == null)
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, attributes);
                    }
                    else
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, exception.TargetSite.Name, attributes);
                    }
                }
            );
            typeToConversionFunctionMap.Add
            (
                typeof(IndexOutOfRangeException),
                (Exception exception) =>
                {
                    var indexOutOfRangeException = (IndexOutOfRangeException)exception;
                    if (exception.TargetSite == null)
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message);
                    }
                    else
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, exception.TargetSite.Name);
                    }
                }
            );
            typeToConversionFunctionMap.Add
            (
                typeof(AggregateException), 
                (Exception exception) =>
                {
                    var aggregateException = (AggregateException)exception;
                    // Convert the inner exceptions
                    Int32 innerExceptionNumber = 1;
                    var innerExceptionDetails = new List<Tuple<String, String>>();
                    foreach (Exception currentInnerException in aggregateException.InnerExceptions)
                    {
                        innerExceptionDetails.Add(new Tuple<String, String>($"InnerException{innerExceptionNumber}Code", currentInnerException.GetType().Name));
                        innerExceptionDetails.Add(new Tuple<String, String>($"InnerException{innerExceptionNumber}Message", currentInnerException.Message));
                        innerExceptionNumber++;
                    }
                    if (exception.TargetSite == null)
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, innerExceptionDetails);
                    }
                    else
                    {
                        return new HttpInternalServerError(exception.GetType().Name, exception.Message, exception.TargetSite.Name, innerExceptionDetails);
                    }
                }
            );
        }

        /// <summary>
        /// Converts the specified exception to an HttpInternalServerError, recursively converting any inner exceptions.
        /// </summary>
        /// <param name="exception">The exception to convert.</param>
        /// <returns>The converted exception.</returns>
        protected HttpInternalServerError ConvertExceptionRecurse(Exception exception)
        {
            // Recursively call for any inner exceptions
            HttpInternalServerError innerError = null;
            if (exception.InnerException != null)
            {
                innerError = ConvertExceptionRecurse(exception.InnerException);
            }

            // Convert the exception using a matching function from the type to conversion function map
            HttpInternalServerError returnError = ConvertException(exception);
            if (innerError != null)
            {
                returnError.InnerError = innerError;
            }

            return returnError;
        }

        /// <summary>
        /// Converts an individual exception (i.e. not its inner exception hierarchy) to an HttpInternalServerError using a function from the type to conversion function map.
        /// </summary>
        /// <param name="exception">The exception to convert.</param>
        /// <returns>The converted exception.</returns>
        protected HttpInternalServerError ConvertException(Exception exception)
        {
            var currentType = exception.GetType();
            while (currentType != null)
            {
                if (typeToConversionFunctionMap.ContainsKey(currentType) == true)
                {
                    return typeToConversionFunctionMap[currentType].Invoke(exception);
                }
                else
                {
                    currentType = currentType.BaseType;
                }
            }
            throw new Exception($"No valid conversion function defined for exception type '{exception.GetType().FullName}'.");
        }
    }
}
