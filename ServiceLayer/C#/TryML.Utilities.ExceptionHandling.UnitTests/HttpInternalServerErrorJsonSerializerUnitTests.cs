using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Newtonsoft.Json.Linq;

namespace TryML.Utilities.ExceptionHandling.UnitTests
{
    /// <summary>
    /// Unit tests for the TryML.Utilities.ExceptionHandling.HttpInternalServerErrorJsonSerializer class.
    /// </summary>
    public class HttpInternalServerErrorJsonSerializerUnitTests
    {
        private HttpInternalServerErrorJsonSerializer testHttpInternalServerErrorJsonSerializer;

        [SetUp]
        protected void SetUp()
        {
            testHttpInternalServerErrorJsonSerializer = new HttpInternalServerErrorJsonSerializer();
        }

        [Test]
        public void Serialize_HttpInternalServerErrorWithCodeAndMessage()
        {
            var serverError = new HttpInternalServerError
            (
                typeof(ArgumentException).Name,
                "Argument 'recordCount' must be greater than or equal to 0."
            );
            string expectedJsonString = @"
            {
                ""error"" : {
                    ""code"" : ""ArgumentException"", 
                    ""message"" : ""Argument 'recordCount' must be greater than or equal to 0.""
                }
            }
            ";
            var expectedJson = JObject.Parse(expectedJsonString);

            JObject result = testHttpInternalServerErrorJsonSerializer.Serialize(serverError);

            Assert.AreEqual(expectedJson, result);
        }

        [Test]
        public void Serialize_HttpInternalServerErrorWithCodeMessageAndTarget()
        {
            var serverError = new HttpInternalServerError
            (
                typeof(ArgumentException).Name,
                "Argument 'recordCount' must be greater than or equal to 0.",
                "recordCount"
            );
            string expectedJsonString = @"
            {
                ""error"" : {
                    ""code"" : ""ArgumentException"", 
                    ""message"" : ""Argument 'recordCount' must be greater than or equal to 0."", 
                    ""target"" : ""recordCount"", 
                }
            }
            ";
            var expectedJson = JObject.Parse(expectedJsonString);

            JObject result = testHttpInternalServerErrorJsonSerializer.Serialize(serverError);

            Assert.AreEqual(expectedJson, result);
        }

        [Test]
        public void Serialize_HttpInternalServerErrorWithCodeMessageAndAttributes()
        {
            var serverError = new HttpInternalServerError
            (
                typeof(ArgumentException).Name,
                "A mapping between user 'user1' and group 'group1' already exists.",
                new List<Tuple<String, String>>()
                {
                    new Tuple<String, String>("user", "user1"),
                    new Tuple<String, String>("group", "group1")
                }
            );
            string expectedJsonString = @"
            {
                ""error"" : {
                    ""code"" : ""ArgumentException"", 
                    ""message"" : ""A mapping between user 'user1' and group 'group1' already exists."", 
                    ""attributes"" : 
                    [
                        { ""name"": ""user"", ""value"": ""user1"" }, 
                        { ""name"": ""group"", ""value"": ""group1"" }
                    ]
                }
            }
            ";
            var expectedJson = JObject.Parse(expectedJsonString);

            JObject result = testHttpInternalServerErrorJsonSerializer.Serialize(serverError);

            Assert.AreEqual(expectedJson, result);
        }


        [Test]
        public void Serialize_HttpInternalServerErrorWithCodeMessageAndInnerError()
        {
            var serverError = new HttpInternalServerError
            (
                typeof(ArgumentException).Name,
                "A mapping between user 'user1' and group 'group1' already exists.",
                new HttpInternalServerError
                (
                    typeof(ArgumentException).Name,
                    "An edge already exists between vertices 'child' and 'parent'."
                )
            );
            string expectedJsonString = @"
            {
                ""error"" : {
                    ""code"" : ""ArgumentException"", 
                    ""message"" : ""A mapping between user 'user1' and group 'group1' already exists."", 
                    ""innererror"" : 
                    {
                        ""code"" : ""ArgumentException"", 
                        ""message"" : ""An edge already exists between vertices 'child' and 'parent'."",
                    }
                }
            }
            ";
            var expectedJson = JObject.Parse(expectedJsonString);

            JObject result = testHttpInternalServerErrorJsonSerializer.Serialize(serverError);

            Assert.AreEqual(expectedJson, result);
        }

        [Test]
        public void Serialize_HttpInternalServerErrorWithAllProperties()
        {
            var serverError = new HttpInternalServerError
            (
                typeof(ArgumentException).Name,
                "Failed to add edge to graph.", 
                "graph",
                new List<Tuple<String, String>>()
                {
                    new Tuple<String, String>("fromVertex", "child"),
                    new Tuple<String, String>("toVertex", "parent")
                },
                new HttpInternalServerError
                (
                    typeof(ArgumentException).Name,
                    "An edge already exists between vertices 'child' and 'parent'."
                )
            );
            string expectedJsonString = @"
            {
                ""error"" : {
                    ""code"" : ""ArgumentException"", 
                    ""message"" : ""Failed to add edge to graph."", 
                    ""target"" : ""graph"", 
                    ""attributes"" : 
                    [
                        { ""name"": ""fromVertex"", ""value"": ""child"" }, 
                        { ""name"": ""toVertex"", ""value"": ""parent"" }
                    ], 
                    ""innererror"" : 
                    {
                        ""code"" : ""ArgumentException"", 
                        ""message"" : ""An edge already exists between vertices 'child' and 'parent'."",
                    }
                }
            }
            ";
            var expectedJson = JObject.Parse(expectedJsonString);

            JObject result = testHttpInternalServerErrorJsonSerializer.Serialize(serverError);

            Assert.AreEqual(expectedJson, result);
        }
    }
}
