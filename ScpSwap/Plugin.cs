// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpSwap
{
    using System;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using RemoteAdmin;
    using PlayerHandlers = Exiled.Events.Handlers.Player;
    using ServerHandlers = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config, Translation>
    {
        private EventHandlers eventHandlers;

        /// <summary>
        /// Gets the only existing instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Author => "Vicious Vikki";

        /// <inheritdoc />
        public override string Name => "ScpSwap";

        /// <inheritdoc />
        public override string Prefix => "ScpSwap";

        /// <inheritdoc />
        public override Version RequiredExiledVersion { get; } = new Version(8, 7, 2);

        /// <inheritdoc />
        public override Version Version { get; } = new Version(1, 1, 8);

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Instance = this;

            eventHandlers = new EventHandlers(this);
            PlayerHandlers.Spawned += eventHandlers.OnSpawned;
            ServerHandlers.ReloadedConfigs += eventHandlers.OnReloadedConfigs;
            ServerHandlers.RestartingRound += eventHandlers.OnRestartingRound;
            ServerHandlers.WaitingForPlayers += eventHandlers.OnWaitingForPlayers;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            PlayerHandlers.Spawned -= eventHandlers.OnSpawned;
            ServerHandlers.ReloadedConfigs -= eventHandlers.OnReloadedConfigs;
            ServerHandlers.RestartingRound -= eventHandlers.OnRestartingRound;
            ServerHandlers.WaitingForPlayers -= eventHandlers.OnWaitingForPlayers;
            eventHandlers = null;

            Instance = null;

            base.OnDisabled();
        }
    }
}