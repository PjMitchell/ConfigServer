using ConfigServer.Server;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class CommandBusTests
    {

        [Fact]
        public async Task ThrowsIfCallsCommandWithNoRegisteredCommandHandler()
        {
            var serviceCollection = new ServiceCollection();
            var commandBus = new CommandBus(serviceCollection.BuildServiceProvider());
            await Assert.ThrowsAsync<ArgumentException>(() => commandBus.SubmitAsync(new TestCommand()));
        }

        [Fact]
        public async Task ReturnsResultOfCommandHandler()
        {
            var commandResult = CommandResult.Success();
            var command = new TestCommand();
            var commandHandler = new Mock<ICommandHandler<TestCommand>>();
            commandHandler.Setup(ch => ch.Handle(command))
                .ReturnsAsync(() => commandResult);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ICommandHandler<TestCommand>>(commandHandler.Object);
            var commandBus = new CommandBus(serviceCollection.BuildServiceProvider());
            var result = await commandBus.SubmitAsync(command);
            Assert.Equal(commandResult, result);
        }
    }

    public class TestCommand : ICommand
    {
        public string CommandName => nameof(TestCommand);
    }
}
