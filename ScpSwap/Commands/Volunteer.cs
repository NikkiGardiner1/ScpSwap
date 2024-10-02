using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;

namespace ScpSwap.Commands
{
    public class Volunteer : ICommand
    {
        public string Command { get; } = "Volunteer";
        public string[] Aliases { get; } = ["v"];
        public string Description { get; } = "Volunteer to become an SCP that left at the start of the round";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            throw new NotImplementedException();
        }
    }
}