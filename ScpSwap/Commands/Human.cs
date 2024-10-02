using System;
using CommandSystem;
using Exiled.API.Features;
using MEC;
using PlayerRoles;

namespace ScpSwap.Commands
{
    public class Human : ICommand
    {
        public string Command { get; } = "human";
        public string[] Aliases { get; } = ["no", "h"];
        public string Description { get; } = "Allow to swap off of being an SCP";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = Plugin.Instance.Translation.WrongUsageMessage;
                return false;
            }

            if (Plugin.Instance.HasReplacementCutoffPassed())
            {
                response = Plugin.Instance.Translation.TooLateMessage;
                return false;
            }

            // Remove non-digits from input so they can type e.g. "SCP-079" and have it still work
            var requestedScp = arguments.FirstElement().ScpNumber();

            // Look in our list of SCPs awaiting replacement and see if any matches
            foreach (var role in Plugin.Instance.ScpsAwaitingReplacement)
            {
                if (role.Name == requestedScp && Player.Get(sender) is Player player)
                {
                    if (player.IsScp && player.Role != RoleTypeId.Scp0492)
                    {
                        response = "SCPs cannot use this command.";
                        return false;
                    }

                    if (Plugin.Instance.ScpsAwaitingReplacement.Any(s => s.Volunteers.Contains(player)))
                    {
                        response = "You cannot volunteer more than once at a time";
                        return false;
                    }

                    role.Volunteers.Add(player);

                    if (role.LotteryTimeout == null)
                    {
                        role.LotteryTimeout = Timing.CallDelayed(Plugin.Instance.Config.LotteryPeriodSeconds, () =>
                        {
                            role.Replace();
                        });
                    }

                    response = $"You have entered the lottery to become SCP {role.Name}.";
                    player.Broadcast(5,
                        Plugin.Instance.Translation.BroadcastHeader +
                        Plugin.Instance.Translation.EnteredLotteryBroadcast.Replace("%NUMBER%", requestedScp),
                        Broadcast.BroadcastFlags.Normal,
                        true
                    );
                    // replacement successful
                    return true;
                }
            }

            // SCP was not in our list of SCPs awaiting replacement
            if (Plugin.Instance.ScpsAwaitingReplacement.IsEmpty())
            {
                response = Plugin.Instance.Translation.NoEligibleSCPsError;
            }
            else
            {
                response = Plugin.Instance.Translation.InvalidSCPError
                     + string.Join(", ", Plugin.Instance.ScpsAwaitingReplacement); // display available SCP numbers
            }
            return false;
        }
    }
}