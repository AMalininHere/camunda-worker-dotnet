using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camunda.Worker.Client;
using Moq;
using Xunit;

namespace Camunda.Worker.Execution
{
    public class DefaultCamundaWorkerTest
    {
        private readonly Mock<IExternalTaskRouter> _routerMock = new Mock<IExternalTaskRouter>();
        private readonly Mock<ITopicsProvider> _topicsProviderMock = new Mock<ITopicsProvider>();
        private readonly Mock<IExternalTaskSelector> _selectorMock = new Mock<IExternalTaskSelector>();
        private readonly Mock<IContextFactory> _contextFactoryMock = new Mock<IContextFactory>();

        public DefaultCamundaWorkerTest()
        {
            _topicsProviderMock.Setup(provider => provider.GetTopics())
                .Returns(Enumerable.Empty<FetchAndLockRequest.Topic>());

            var contextMock = new Mock<IExternalTaskContext>();
            _contextFactoryMock.Setup(factory => factory.MakeContext(It.IsAny<ExternalTask>()))
                .Returns(contextMock.Object);
        }

        [Fact]
        public async Task TestRunWithoutTasks()
        {
            var cts = new CancellationTokenSource();

            ConfigureSelector(cts, new List<ExternalTask>());

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _routerMock.VerifyNoOtherCalls();
            _selectorMock.VerifyAll();
        }

        [Fact]
        public async Task TestRunWithTask()
        {
            var cts = new CancellationTokenSource();

            ConfigureSelector(cts, new List<ExternalTask>
            {
                new ExternalTask("test", "test", "test")
            });

            _routerMock
                .Setup(executor => executor.RouteAsync(It.IsAny<IExternalTaskContext>()))
                .Callback(cts.Cancel)
                .Returns(Task.CompletedTask);

            var worker = CreateWorker();

            await worker.Run(cts.Token);

            _contextFactoryMock.Verify(
                factory => factory.MakeContext(It.IsAny<ExternalTask>()),
                Times.Once()
            );
            _selectorMock.VerifyAll();
        }

        private void ConfigureSelector(CancellationTokenSource cts, IList<ExternalTask> externalTasks)
        {
            _selectorMock
                .Setup(selector => selector.SelectAsync(
                    It.IsAny<IEnumerable<FetchAndLockRequest.Topic>>(),
                    It.IsAny<CancellationToken>()
                ))
                .Callback(cts.Cancel)
                .ReturnsAsync(externalTasks);
        }

        private ICamundaWorker CreateWorker()
        {
            return new DefaultCamundaWorker(
                _routerMock.Object,
                _topicsProviderMock.Object,
                _selectorMock.Object,
                _contextFactoryMock.Object
            );
        }
    }
}
