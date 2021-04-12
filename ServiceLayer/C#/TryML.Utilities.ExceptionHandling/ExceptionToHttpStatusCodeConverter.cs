using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace TryML.Utilities.ExceptionHandling
{
    /// <summary>
    /// Converts an Exception to an equivalent HttpStatusCode.
    /// </summary>
    public class ExceptionToHttpStatusCodeConverter
    {
        /// <summary>Maps a type (assignable to Exception) to a HttpStatusCode.</summary>
        protected Dictionary<Type, HttpStatusCode> typeToHttpStatusCodeMap;

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.ExceptionToHttpStatusCodeConverter class.
        /// </summary>
        public ExceptionToHttpStatusCodeConverter()
        {
            typeToHttpStatusCodeMap = new Dictionary<Type, HttpStatusCode>();
            InitialisetypeToStatusCodeMap(typeToHttpStatusCodeMap);
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.ExceptionToHttpStatusCodeConverter class.
        /// </summary>
        /// <param name="typesAndStatusCodes">A collection of tuples containing 2 values: A type (assignable to Exception), and an HTTP status code to map the exception type to.</param>
        public ExceptionToHttpStatusCodeConverter(IEnumerable<Tuple<Type, HttpStatusCode>> typesAndStatusCodes)
            : base()
        {
            foreach (var currentMapping in typesAndStatusCodes)
            {
                AddMapping(currentMapping.Item1, currentMapping.Item2);
            }
        }

        /// <summary>
        /// Adds a mapping to the converter.
        /// </summary>
        /// <param name="exceptionType">The type (assignable to Exception) to map from.</param>
        /// <param name="httpStatusCode">The HTTP status code to map the exception type to.</param>
        public void AddMapping(Type exceptionType, HttpStatusCode httpStatusCode)
        {
            if (typeof(Exception).IsAssignableFrom(exceptionType) == false)
                throw new ArgumentException($"Type '{exceptionType.FullName}' specified in parameter '{nameof(exceptionType)}' is not assignable to '{typeof(Exception).FullName}'.", nameof(exceptionType));

            if (typeToHttpStatusCodeMap.ContainsKey(exceptionType) == true)
            {
                typeToHttpStatusCodeMap.Remove(exceptionType);
            }
            typeToHttpStatusCodeMap.Add(exceptionType, httpStatusCode);
        }

        /// <summary>
        /// Converts the specified exception to its equivalent HTTP status code.
        /// </summary>
        /// <param name="exception">The exception to convert.</param>
        /// <returns>The HTTP status code.</returns>
        public HttpStatusCode Convert(Exception exception)
        {
            Type exceptionType = exception.GetType();
            while (typeToHttpStatusCodeMap.ContainsKey(exceptionType) == false)
            {
                exceptionType = exceptionType.BaseType;
            }

            return typeToHttpStatusCodeMap[exceptionType];
        }

        /// <summary>
        /// Initialises the specified type to HTTP status code map with default mappings.
        /// </summary>
        /// <param name="typeToHttpStatusCodeMap">The type to HTTP status code map to initialise.</param>
        protected void InitialisetypeToStatusCodeMap(Dictionary<Type, HttpStatusCode> typeToHttpStatusCodeMap)
        {
            typeToHttpStatusCodeMap.Add(typeof(ArgumentException), HttpStatusCode.BadRequest);
            typeToHttpStatusCodeMap.Add(typeof(Exception), HttpStatusCode.InternalServerError);
        }
    }
}
