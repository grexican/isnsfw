using System;
using System.Collections.Generic;
using System.Text;
using IsNsfw.Command.Interface;
using ServiceStack.FluentValidation;

namespace IsNsfw.Command
{
    public abstract class CommandValidatorBase<TCommand> : AbstractValidator<TCommand>, ICommandValidator<TCommand> where TCommand : ICommand
    {
    }
}
