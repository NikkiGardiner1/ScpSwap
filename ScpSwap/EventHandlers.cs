// -----------------------------------------------------------------------
// <copyright file="EventHandlers.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

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

        /*public void OnPlayerLeave(LeftEventArgs ev)
        {
            VolunteerSwap.ScpLeft(Player scpPlayer);
        }*/
    }
}