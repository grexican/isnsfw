using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.FluentValidation;

namespace IsNsfw.Command.Interface
{
    public interface ICommandValidator<in TCommand> : IValidator<TCommand> where TCommand : ICommand
    {
    }
}
