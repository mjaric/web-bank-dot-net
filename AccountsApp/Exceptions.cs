using System;

namespace AccountsApp
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }

    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message)
        {
        }
    }

    public class NotFoundException : DomainException
    {
        public NotFoundException() : base("Not Found")
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }

    public class AlreadyCreatedException : DomainException
    {
        public AlreadyCreatedException() : base("Already created")
        {
        }

        public AlreadyCreatedException(string message) : base(message)
        {
        }
    }
}