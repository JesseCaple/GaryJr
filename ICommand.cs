﻿using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GaryJr
{
    interface ICommand
    {
        bool HasPermission(SocketUser user);
        Task RunAsync(SocketMessage message);
        string Description { get; }
    }
}
