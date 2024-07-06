// -----------------------------------------------------------------------
// <copyright file="ScpSwapParent.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpSwap.Commands
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScpSwap.Configs;
    using ScpSwap.Models;

    /// <summary>
    /// The base command for ScpSwapParent.
    /// </summary>
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ScpSwapParent : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScpSwapParent"/> class.
        /// </summary>
        public ScpSwapParent() => LoadGeneratedCommands();

        /// <inheritdoc />
        public override string Command => "scpswap";

        /// <inheritdoc />
        public override string[] Aliases { get; } = { "swap" };

        /// <inheritdoc />
        public override string Description => "Base command for ScpSwap.";

        /// <inheritdoc />
        public sealed override void LoadGeneratedCommands()
        {
            CommandTranslations commandTranslations = Plugin.Instance.Translation.CommandTranslations;

            RegisterCommand(commandTranslations.Accept);
            RegisterCommand(commandTranslations.Cancel);
            RegisterCommand(commandTranslations.Decline);
            RegisterCommand(commandTranslations.List);
        }

        /// <inheritdoc />
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player playerSender = Player.Get(sender);
            if (playerSender == null)
            {
                response = "This command must be executed at the game level.";
                return false;
            }

            if (playerSender.CheckPermission("scpswap.command"))
            {
                response = "You do not have permission to execute this command."
                return false;
            }

            if (!Round.IsStarted)
            {
                response = "The round has not yet started.";
                return false;
            }

            if (Round.ElapsedTime.TotalSeconds > Plugin.Instance.Config.SwapTimeout)
            {
                response = "The swap period has ended.";
                return false;
            }

            if (arguments.IsEmpty())
            {
                response = $"Usage: .{Command} ScpNumber";
                return false;
            }

            if (!playerSender.IsScp && ValidSwaps.GetCustom(playerSender) == null)
            {
                response = "You must be an Scp to use this command.";
                return false;
            }

            if (Swap.FromSender(playerSender) != null)
            {
                response = "You already have a pending swap request!";
                return false;
            }

            Player receiver = GetReceiver(arguments.At(0), out Action<Player> spawnMethod);
            if (playerSender == receiver)
            {
                response = "You can't swap with yourself.";
                return false;
            }

            if (Plugin.Instance.Config.BlacklistedSwapFromScps.Contains(playerSender.Role.Type))
            {
                response = "You're not allowed to swap off from this SCP.";
                return false;
            }

            if (receiver != null)
            {
                Swap.Send(playerSender, receiver);
                response = "Request sent!";
                return true;
            }

            if (spawnMethod == null)
            {
                response = "Unable to find the specified role. Please refer to the list command for available roles.";
                return false;
            }

            if (Plugin.Instance.Config.AllowNewScps)
            {
                spawnMethod(playerSender);
                response = "Swap successful.";
                return true;
            }

            response = "Unable to locate a player with the requested role.";
            return false;
        }

        private static Player GetReceiver(string request, out Action<Player> spawnMethod)
        {
            CustomSwap customSwap = ValidSwaps.GetCustom(request);
            if (customSwap != null)
            {
                spawnMethod = customSwap.SpawnMethod;
                return Player.List.FirstOrDefault(player => customSwap.VerificationMethod(player));
            }

            RoleTypeId roleSwap = ValidSwaps.Get(request);
            if (roleSwap != RoleTypeId.None)
            {
                spawnMethod = player => player.Role.Set(roleSwap);
                return Player.List.FirstOrDefault(player => player.Role == roleSwap);
            }

            spawnMethod = null;
            return null;
        }
    }
}