﻿using System;
using System.Collections.Generic;

namespace TryML.Utilities.ExceptionHandling
{
    /// <summary>
    /// Container class holding the data returned from a REST API when a HTTP 500 error is generated.
    /// </summary>
    /// <remarks>Based on the Microsoft REST API Guidelines... https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#7102-error-condition-responses.</remarks>
    public class HttpInternalServerError
    {
        /// <summary>An internal code representing the error.</summary>
        /// <remarks>Typically this should contain the class name of the exception which caused the server error.</remarks>
        protected String code;
        /// <summary>A description of the error.</summary>
        protected String message;
        /// <summary>The target of the error.</summary>
        protected String target;
        /// <summary>A collection of key/value pairs which give additional details of the error.</summary>
        protected List<Tuple<String, String>> attributes;
        /// <summary>The error which caused this error.</summary>
        protected HttpInternalServerError innerError;

        /// <summary>
        /// An internal code representing the error.
        /// </summary>
        /// <remarks>Typically this should contain the class name of the exception which caused the server error.</remarks>
        public String Code
        {
            get { return code; }
        }

        /// <summary>
        /// A description of the error.
        /// </summary>
        public String Message
        {
            get { return message; }
        }

        /// <summary>
        /// The target of the error.
        /// </summary>
        public String Target
        {
            get { return target; }
        }

        /// <summary>
        /// A collection of key/value pairs which give additional details of the error.
        /// </summary>
        public IEnumerable<Tuple<String, String>> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// The error which caused this error.
        /// </summary>
        public HttpInternalServerError InnerError
        {
            get { return innerError; }
            set { innerError = value; }
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        public HttpInternalServerError(String code, String message)
        {
            // Ordinarily would have exception handlers here for null or whitespace 'code' and 'message' parameters...
            //   However since this instances of this class will likely be created as part of exception handling code, we don't want to throw further exceptions and risk hiding/losing the original exception details.

            this.code = code;
            this.message = message;
            target = null;
            attributes = new List<Tuple<String, String>>();
            innerError = null;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="target">The target of the error.</param>
        public HttpInternalServerError(String code, String message, String target)
            : this (code, message)
        {
            this.target = target;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="attributes">A collection of key/value pairs which give additional details of the error.</param>
        public HttpInternalServerError(String code, String message, List<Tuple<String, String>> attributes)
            : this(code, message)
        {
            this.attributes = attributes;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="innerError">The error which caused this error.</param>
        public HttpInternalServerError(String code, String message, HttpInternalServerError innerError)
            : this(code, message)
        {
            this.innerError = innerError;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="target">The target of the error.</param>
        /// <param name="attributes">A collection of key/value pairs which give additional details of the error.</param>
        public HttpInternalServerError(String code, String message, String target, List<Tuple<String, String>> attributes)
            : this(code, message)
        {
            this.target = target;
            this.attributes = attributes;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="attributes">A collection of key/value pairs which give additional details of the error.</param>
        /// <param name="innerError">The error which caused this error.</param>
        public HttpInternalServerError(String code, String message, List<Tuple<String, String>> attributes, HttpInternalServerError innerError)
            : this(code, message)
        {
            this.attributes = attributes;
            this.innerError = innerError;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="target">The target of the error.</param>
        /// <param name="innerError">The error which caused this error.</param>
        public HttpInternalServerError(String code, String message, String target, HttpInternalServerError innerError)
            : this(code, message)
        {
            this.target = target;
            this.innerError = innerError;
        }

        /// <summary>
        /// Initialises a new instance of the TryML.Utilities.ExceptionHandling.HttpInternalServerError class.
        /// </summary>
        /// <param name="code">An internal code representing the error.</param>
        /// <param name="message">A description of the error.</param>
        /// <param name="target">The target of the error.</param>
        /// <param name="attributes">A collection of key/value pairs which give additional details of the error.</param>
        /// <param name="innerError">The error which caused this error.</param>
        public HttpInternalServerError(String code, String message, String target, List<Tuple<String, String>> attributes, HttpInternalServerError innerError)
            : this(code, message)
        {
            this.target = target;
            this.attributes = attributes;
            this.innerError = innerError;
        }
    }
}
