using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal interface ICommandBus
    {
        Task<CommandResult> SubmitAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }

    internal class CommandBus : ICommandBus
    {
        private IServiceProvider serviceProvider;

        public CommandBus(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task<CommandResult> SubmitAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = serviceProvider.GetService<ICommandHandler<TCommand>>();
            if(handler == null)
                throw new ArgumentException($"Handler for Command type {typeof(TCommand).Name} not found", nameof(command));
            return handler.Handle(command);
        }
    }
}
