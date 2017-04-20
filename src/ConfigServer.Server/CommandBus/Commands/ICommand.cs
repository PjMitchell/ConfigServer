using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal interface ICommand
    {
        string CommandName { get; }
    }
}
