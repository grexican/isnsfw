using System;
using System.Collections.Generic;
using System.Text;

namespace IsNsfw.Command.Interface
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
