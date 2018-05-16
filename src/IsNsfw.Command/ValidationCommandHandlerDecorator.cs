using System.Linq;
using IsNsfw.Command.Interface;
using ServiceStack.FluentValidation;

namespace IsNsfw.Command
{
    // TODO: Possible Race Condition. Post-validation, pre-command. Need to wrap the whole thing in a UOW?
    public class ValidationCommandHandlerDecorator<T> : ICommandHandler<T> where T : ICommand
    {
        private readonly ICommandHandler<T> _decoratee;
        private readonly ICommandValidator<T> _validator;

        public ValidationCommandHandlerDecorator(ICommandHandler<T> decoratee, ICommandValidator<T> validator)
        {
            _decoratee = decoratee;
            _validator = validator;
        }

        public void Handle(T command)
        {
            var results = _validator.Validate(command);

            if (!results.IsValid)
                throw new ValidationException(results.Errors);

            _decoratee.Handle(command);
        }
    }
}
