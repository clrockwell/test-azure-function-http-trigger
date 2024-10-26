using System;
using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DemoProjects
{
    public class HttpResponseTriggerTests
    {
        private readonly Mock<ILogger<HttpResponseTrigger>> _loggerMock;

        private readonly Mock<HttpRequestData> _requestMock;

        private readonly Mock<HttpResponseData> _responseMock;

        private readonly Mock<DurableTaskClient> _starterMock;

        private readonly HttpResponseTrigger _SUT_httpResponseTrigger;

        public HttpResponseTriggerTests()
        {
            _loggerMock = new Mock<ILogger<HttpResponseTrigger>>();
            _SUT_httpResponseTrigger = new HttpResponseTrigger(_loggerMock.Object);
            Mock<FunctionContext> functionContextMock = new Mock<FunctionContext>();
            _requestMock = new Mock<HttpRequestData>(functionContextMock.Object);
            _responseMock = new Mock<HttpResponseData>(functionContextMock.Object);
            _starterMock = new Mock<DurableTaskClient>("HttpResponseTrigger");

            _requestMock.Setup(r => r.CreateResponse()).Returns(_responseMock.Object);
            _responseMock.SetupProperty(r => r.StatusCode);
        }

        [Fact]
        public async Task HttpResponse_ShouldReturnUnprocessableEntity_OnDeserializationException()
        {
            var content = "invalid content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            _requestMock.Setup(r => r.Body).Returns(stream);

            var result = await _SUT_httpResponseTrigger.RunAsync(_requestMock.Object, _starterMock.Object);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
        }
    }
}
