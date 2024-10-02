// -----------------------------------------------------------------------
// <copyright file="SwapData.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features.Roles;

namespace ScpSwap.Models
{
    using Exiled.API.Features;
    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// A container to swap data between players.
    /// </summary>
    public class SwapData
    {
        private readonly CustomSwap customSwap;
        private readonly RoleTypeId role;
        private readonly Vector3 position;
        private readonly float health;
        private int level;
        private int exp;
        private object scp079Role;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapData"/> class.
        /// </summary>
        /// <param name="player">The player to generate the data from.</param>
        public SwapData(Player player)
        {
            role = player.Role;
            position = player.Position;
            health = player.Health;
            if (Config.Preserve079XpOnSwap)
            {
                if (player.Role.Is(out Scp079Role scp079Role))
                {
                    level = scp079Role.Level;
                    exp = scp079Role.Experience;
                }
            }
            customSwap = ValidSwaps.GetCustom(player);
        }

        /// <summary>
        /// Spawns a player with the contained swap data.
        /// </summary>
        /// <param name="player">The player to swap.</param>
        public void Swap(Player player)
        {
            if (customSwap == null)
                player.Role.Set(role);
            else
                customSwap.SpawnMethod(player);

            player.Position = position;
            player.Health = health;
            if (Config.Preserve079XpOnSwap)
            {
                if (player.Role.Is(out Scp079Role scp079Role))
                {
                    scp079Role.Level = level;
                    scp079Role.Experience = exp;
                }
            }
        }
    }
}