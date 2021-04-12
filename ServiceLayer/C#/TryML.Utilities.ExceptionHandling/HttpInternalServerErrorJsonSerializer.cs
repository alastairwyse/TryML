using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TryML.Utilities.ExceptionHandling
{
    /// <summary>
    /// Serializes HttpInternalServerError instances to JSON documents.
    /// </summary>
    public class HttpInternalServerErrorJsonSerializer
    {
        protected const String errorPropertyName = "error";
        protected const String codePropertyName = "code";
        protected const String messagePropertyName = "message";
        protected const String targetPropertyName = "target";
        protected const String attributesPropertyName = "attributes";
        protected const String namePropertyName = "name";
        protected const String valuePropertyName = "value";
        protected const String innerErrorPropertyName = "innererror";

        /// <summary>
        /// Serializes the specified HttpInternalServerError to a JSON document.
        /// </summary>
        /// <param name="httpInternalServerError">The HttpInternalServerError object to serialize.</param>
        /// <returns>A JSON document representing the HttpInternalServerError.</returns>
        public JObject Serialize(HttpInternalServerError httpInternalServerError)
        {
            var returnDocument = new JObject();

            returnDocument.Add(errorPropertyName, SerializeError(httpInternalServerError));

            return returnDocument;
        }

        /// <summary>
        /// Serializes the 'error' and 'innererror' properties of the JSON document returned by the Serialize() method.
        /// </summary>
        /// <param name="httpInternalServerError">The HttpInternalServerError object to serialize.</param>
        /// <returns>The 'error' or 'innererror' property of the JSON document.</returns>
        protected JObject SerializeError(HttpInternalServerError httpInternalServerError)
        {
            var returnDocument = new JObject();

            returnDocument.Add(codePropertyName, httpInternalServerError.Code);
            returnDocument.Add(messagePropertyName, httpInternalServerError.Message);
            if (httpInternalServerError.Target != null)
            {
                returnDocument.Add(targetPropertyName, httpInternalServerError.Target);
            }
            var attributesJson = new JArray();
            foreach (Tuple<String, String> currentAttribute in httpInternalServerError.Attributes)
            {
                var currentAttributeJson = new JObject();
                currentAttributeJson.Add(namePropertyName, currentAttribute.Item1);
                currentAttributeJson.Add(valuePropertyName, currentAttribute.Item2);
                attributesJson.Add(currentAttributeJson);
            }
            if (attributesJson.Count > 0)
            {
                returnDocument.Add(attributesPropertyName, attributesJson);
            }
            if (httpInternalServerError.InnerError != null)
            {
                returnDocument.Add(innerErrorPropertyName, SerializeError(httpInternalServerError.InnerError));
            }

            return returnDocument;
        }
    }
}
