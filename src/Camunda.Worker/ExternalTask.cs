using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Camunda.Worker
{
    public class ExternalTask
    {
        [JsonConstructor]
        public ExternalTask(string id, string workerId, string topicName)
        {
            Id = Guard.NotNull(id, nameof(id));
            WorkerId = Guard.NotNull(workerId, nameof(workerId));
            TopicName = Guard.NotNull(topicName, nameof(topicName));
            CreationTime = DateTime.Now;
        }

        public string Id { get; }
        public string WorkerId { get; }
        public string TopicName { get; }
        [JsonIgnore] public DateTime CreationTime { get; }
        public int Priority { get; set; }
        public string ExecutionId { get; set; }
        public string ActivityId { get; set; }
        public string ActivityInstanceId { get; set; }
        public string TenantId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public string ProcessDefinitionKey { get; set; }
        public string ProcessInstanceId { get; set; }
        public string BusinessKey { get; set; }
        public int? Retries { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public IDictionary<string, Variable> Variables { get; set; }
    }
}
