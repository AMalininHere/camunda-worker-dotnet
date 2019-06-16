using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Camunda.Worker.Execution;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Camunda.Worker
{
    public class CamundaWorkerBuilderTest
    {
        [Fact]
        public void TestAddDescriptor()
        {
            var services = new ServiceCollection();
            var handlerMock = new Mock<IExternalTaskHandler>();

            var builder = new CamundaWorkerBuilder(services);

            builder.AddHandlerDescriptor(new HandlerDescriptor(provider => handlerMock.Object,
                new HandlerMetadata(new[] {"testTopic"})));

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ImplementationInstance != null);
        }

        [Fact]
        public void TestAddNullDescriptor()
        {
            var services = new ServiceCollection();
            var builder = new CamundaWorkerBuilder(services);

            Assert.Throws<ArgumentNullException>(() => builder.AddHandlerDescriptor(null));
        }

        [Fact]
        public void TestAddFactoryProvider()
        {
            var services = new ServiceCollection();
            var builder = new CamundaWorkerBuilder(services);

            builder.AddFactoryProvider<HandlerDelegateProvider>();

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(IHandlerDelegateProvider) &&
                                           d.ImplementationType == typeof(HandlerDelegateProvider));
        }

        [Fact]
        public void TestAddTopicsProvider()
        {
            var services = new ServiceCollection();
            var builder = new CamundaWorkerBuilder(services);

            builder.AddTopicsProvider<TopicsProvider>();

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(ITopicsProvider) &&
                                           d.ImplementationType == typeof(TopicsProvider));
        }

        [Fact]
        public void TestAddTaskSelector()
        {
            var services = new ServiceCollection();
            var builder = new CamundaWorkerBuilder(services);

            builder.AddTaskSelector<ExternalTaskSelector>();

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Transient &&
                                           d.ServiceType == typeof(IExternalTaskSelector) &&
                                           d.ImplementationType == typeof(ExternalTaskSelector));
        }

        [Fact]
        public void TestConfigurePipeline()
        {
            var services = new ServiceCollection();
            var builder = new CamundaWorkerBuilder(services);

            builder.ConfigurePipeline(pipeline => { });

            Assert.Contains(services, d => d.Lifetime == ServiceLifetime.Singleton &&
                                           d.ServiceType == typeof(PipelineDescriptor));
        }

        private class HandlerDelegateProvider : IHandlerDelegateProvider
        {
            public ExternalTaskDelegate GetHandlerDelegate(ExternalTask externalTask)
            {
                throw new NotImplementedException();
            }
        }

        private class TopicsProvider : ITopicsProvider
        {
            public IEnumerable<FetchAndLockRequest.Topic> GetTopics()
            {
                throw new NotImplementedException();
            }
        }

        private class ExternalTaskSelector : IExternalTaskSelector
        {
            public Task<IEnumerable<ExternalTask>> SelectAsync(IEnumerable<FetchAndLockRequest.Topic> topics,
                CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
