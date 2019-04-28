using System;
using System.Linq;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class TopicBasedFactoryProviderTest
    {
        [Fact]
        public void TestGetKnownHandlerFactory()
        {
            var handlerMock = new Mock<IExternalTaskHandler>();

            var provider = new TopicBasedFactoryProvider(new[]
            {
                new HandlerDescriptor(p => handlerMock.Object, new HandlerMetadata(new[] {"topic1"}))
            });

            var factory = provider.GetHandlerFactory(new ExternalTask("test", "test", "topic1"));
            Assert.NotNull(factory);
        }

        [Fact]
        public void TestGetUnknownHandlerFactory()
        {
            var provider = new TopicBasedFactoryProvider(Enumerable.Empty<HandlerDescriptor>());
            Assert.Throws<ArgumentException>(
                () => provider.GetHandlerFactory(new ExternalTask("test", "test", "topic1"))
            );
        }
    }
}
