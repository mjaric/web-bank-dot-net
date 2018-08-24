using Shared.Commands;

namespace Shared.Messages
{
    public class CommandEnvelope
    {
        public string AggregateId { get; }
        public ICommand Command { get; }

        public CommandEnvelope(string aggregateId, ICommand command)
        {
            AggregateId = aggregateId;
            Command = command;
        }
    }
}