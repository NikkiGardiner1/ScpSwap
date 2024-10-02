// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.CustomRoles.API;
using SCPReplacer;

namespace ScpSwap
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using PlayerRoles;
    using ScpSwap.Models;

    /// <summary>
    /// Handles events derived from <see cref="Exiled.Events.Handlers"/>.
    /// </summary>
    public class EventHandlers
    {
        private readonly Plugin plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="plugin">An instance of the <see cref="Plugin"/> class.</param>
        public EventHandlers(Plugin plugin) => this.plugin = plugin;
        
        public List<ScpReplace> ScpsAwaitingReplacement { get; } = new List<ScpReplace>();

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRoundStarted"/>
        public void OnSpawned(SpawnedEventArgs ev)
        {
            if ((ev.Player.IsScp || ValidSwaps.GetCustom(ev.Player) != null) && ev.OldRole.Type.GetTeam() != Team.SCPs && Round.ElapsedTime.TotalSeconds < plugin.Config.SwapTimeout)
                ev.Player.Broadcast(plugin.Translation.StartMessage);
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnReloadedConfigs"/>
        public void OnReloadedConfigs()
        {
            ValidSwaps.Refresh();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnRestartingRound"/>
        public void OnRestartingRound()
        {
            Swap.Clear();
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Server.OnWaitingForPlayers"/>
        public void OnWaitingForPlayers()
        {
            ValidSwaps.Refresh();
        }

        public void OnRoundStart()
        {
            /*if (!Config.IndependentNotificationForScps)
                return;
            foreach (var scp in Player.List.Where(p => p.IsScp))
            {
                scp.Broadcast(8, "Use <color=#ff4eac>.human</color> in the <color=#b8bd00>~</color> console if you want to be a human class instead");
            }*/

            ScpsAwaitingReplacement.Clear();
        }
        public void OnPlayerLeave(LeftEventArgs ev)
        {
            ScpLeft(ev.Player);
        } 
        public void ScpLeft(Player scpPlayer)
        {
            if (scpPlayer.IsScp && scpPlayer.Role != RoleTypeId.Scp0492)
            {
                var elapsedSeconds = Round.ElapsedTime.TotalSeconds;
                // Minimum required health (configurable percentage) of the SCP
                // when they quit to be eligible for replacement
                var requiredHealth = (int)(Config.RequiredHealthPercentage / 100.0 * scpPlayer.MaxHealth);
                var customRole = scpPlayer.GetCustomRoles().FirstOrDefault();
                var scpNumber = customRole?.Name.ScpNumber() ?? scpPlayer.Role.ScpNumber();
                Log.Debug($"{scpPlayer.Nickname} left {elapsedSeconds} seconds into the round, was SCP-{scpNumber} with {scpPlayer.Health}/{scpPlayer.MaxHealth} HP ({requiredHealth} required for replacement)");
                if (elapsedSeconds > Config.QuitCutoff)
                {
                    Log.Debug("This SCP will not be replaced because the quit cutoff has already passsed");
                    return;
                }

                if (scpPlayer.Health < requiredHealth)
                {
                    Log.Debug("This SCP will not be replaced because they have lost too much health");
                    return;
                }

                // Let all non-SCPs (including spectators) know of the opportunity to become SCP
                // SCPs are not told of this because then they would also have to be replaced after swapping 
                foreach (var p in Player.List.Where(x => !x.IsScp))
                {
                    var message = Translation.ReplaceBroadcast.Replace("%NUMBER%", scpNumber);
                    // Longer broadcast time since beta test revealed users were having trouble reading it all in time
                    p.Broadcast(16, Translation.BroadcastHeader + message, global::Broadcast.BroadcastFlags.Normal, true);
                    // Also send console message in case they miss the broadcast
                    p.SendConsoleMessage(message, "yellow");
                }

                // Add the SCP to our list so that a user can claim it with the .volunteer command

                ScpsAwaitingReplacement.Add(new ScpReplace
                {
                    Name = scpNumber,
                    Role = scpPlayer.Role,
                    CustomRole = customRole
                });
            }
        }

        /// <summary>
        ///     Whether the replacement cutoff (i.e. the max time in seconds after the round
        ///     that a player can still use .volunteer) has passed
        ///     We put this function here so that we can conveniently access the Config without
        ///     needing to implement the Singleton pattern in Config
        /// </summary>
        /// <returns>whether the replacement period cutoff has passed (true if passed)</returns>
        public bool HasReplacementCutoffPassed()
        {
            return Round.ElapsedTime.TotalSeconds > Config.ReplaceCutoff;
        }
    }
}