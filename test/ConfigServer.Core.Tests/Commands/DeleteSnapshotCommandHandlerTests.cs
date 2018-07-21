using ConfigServer.Server;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class DeleteSnapshotCommandHandlerTests
    {
        Mock<IConfigurationSnapshotRepository> repo;
        ICommandHandler<DeleteSnapshotCommand> target;

        public DeleteSnapshotCommandHandlerTests()
        {
            repo = new Mock<IConfigurationSnapshotRepository>();
            target = new DeleteSnapshotCommandHandler(repo.Object);
        }

        [Fact]
        public async Task Handle_CallsRepo()
        {
            var snapshotId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var result = await target.Handle(new DeleteSnapshotCommand { SnapshotId = snapshotId });
            Assert.True(result.IsSuccessful);
            repo.Verify(r => r.DeleteSnapshot(snapshotId));

        }
    }
}
