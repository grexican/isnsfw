using System;
using System.Collections.Generic;
using System.Text;

namespace IsNsfw.Command.Interface
{
    public interface ICommandWithResult<TIdType> : ICommand
    {
        TIdType Id { get; set; }
    }

    public interface ICommandWithIntResult: ICommandWithResult<int> { }
}
