using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class DeleteSnapshotCommandHandler : ICommandHandler<DeleteSnapshotCommand>
    {
        IConfigurationSnapshotRepository repo;

        public DeleteSnapshotCommandHandler(IConfigurationSnapshotRepository repo)
        {
            this.repo = repo;
        }

        public async Task<CommandResult> Handle(DeleteSnapshotCommand command)
        {
            await repo.DeleteSnapshot(command.SnapshotId);
            return CommandResult.Success();
        }
    }
}
