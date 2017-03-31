using System;
using System.Text;

namespace ConfigServer.Server
{
    internal class ConfigurationClientGroupUpdatedEvent : IEvent
    {
        public ConfigurationClientGroupUpdatedEvent(string groupId)
        {
            GroupId = groupId;
        }

        public string GroupId { get; }
    }
}
