using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Camunda.Worker.Execution
{
    public sealed class ExternalTaskRouter : IExternalTaskRouter
    {
        private readonly IHandlerDelegateProvider _handlerDelegateProvider;
        private readonly ILogger<ExternalTaskRouter> _logger;

        public ExternalTaskRouter(IHandlerDelegateProvider handlerDelegateProvider,
            ILogger<ExternalTaskRouter> logger = null)
        {
            _handlerDelegateProvider = Guard.NotNull(handlerDelegateProvider, nameof(handlerDelegateProvider));
            _logger = logger ?? NullLogger<ExternalTaskRouter>.Instance;
        }

        public async Task RouteAsync(IExternalTaskContext context)
        {
            Guard.NotNull(context, nameof(context));

            var handlerDelegate = _handlerDelegateProvider.GetHandlerDelegate(context.Task);
            var externalTask = context.Task;

            _logger.LogInformation("Started processing of task {TaskId}", externalTask.Id);

            await handlerDelegate(context);

            _logger.LogInformation("Finished processing of task {TaskId}", externalTask.Id);
        }
    }
}
