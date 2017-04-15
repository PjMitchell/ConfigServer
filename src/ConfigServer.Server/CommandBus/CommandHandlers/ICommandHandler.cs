using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    interface ICommandHandler<TCommand>
    {
        Task<CommandResult> Handle(TCommand command);
    }
}
