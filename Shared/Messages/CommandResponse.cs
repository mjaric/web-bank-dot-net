using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Events;

namespace Shared.Messages
{
    public interface ICommandResponse
    {
        string Message { get; }
    }

    public class CommandResponse
    {
        public string Message { get; }
        public IEnumerable<IDomainEvent> Events { get; }

        public CommandResponse(string message, IEnumerable<IDomainEvent> events = null)
        {
            Events = new List<IDomainEvent>(events ?? new List<IDomainEvent>());
            Message = message;
        }
    }

    public class ErrorCommandResponse : ICommandResponse
    {
        public string Message { get; }
        public string Exception { get; }

        public ErrorCommandResponse(string message, Exception ex = null)
        {
            Message = message;
        }
    }
}