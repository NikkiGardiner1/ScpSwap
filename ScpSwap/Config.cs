// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpSwap
{
    using System;
    using System.ComponentModel;
    using Exiled.API.Interfaces;
    using PlayerRoles;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be shown.
        /// </summary>
        [Description("Indicates whether debug messages should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the duration, in seconds, before a swap request gets automatically deleted.
        /// </summary>
        [Description("The duration, in seconds, before a swap request gets automatically deleted.")]
        public float RequestTimeout { get; set; } = 20f;

        /// <summary>
        /// Gets or sets the duration, in seconds, after the round starts that swap requests can be sent.
        /// </summary>
        [Description("The duration, in seconds, after the round starts that swap requests can be sent.")]
        public float SwapTimeout { get; set; } = 60f;

        /// <summary>
        /// Gets or sets a value indicating whether a player can switch to a class if there is nobody playing as it.
        /// </summary>
        [Description("Indicates whether a player can switch to a class if there is nobody playing as it.")]
        public bool AllowNewScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a collection of roles blacklisted from being swapped to.
        /// </summary>
        [Description("A collection of roles blacklisted from being swapped to.")]
        public RoleTypeId[] BlacklistedSwapToScps { get; set; } =
        {
            RoleTypeId.Scp0492,
        };

        /// <summary>
        /// Gets or sets a collection of the names of custom scps blacklisted from being swapped to. This must match the name the developer integrated the SCP into this plugin's API with.
        /// </summary>
        [Description("A collection of the names of custom scps blacklisted from being swapped to. This must match the name the developer integrated the SCP into this plugin's API with.")]
        public string[] BlacklistedNames { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets roles of SCPs who cannot be swapped off from.
        /// </summary>
        [Description("A collection of roles blacklisted from swapping off from.")]
        public RoleTypeId[] BlacklistedSwapFromScps { get; set; } =
        {
            RoleTypeId.Scp0492,
        };

        /// <summary>
        /// Enables/Disables SCPs/Humans swapping off or swapping onto SCPs
        /// </summary>
        [Description("Allow for human and volunteering (going off and coming on as SCPs")]
        public bool AllowVolunteerAndHuman { get; set; } = true;

        /// <summary>
        /// Enables/Disables Humans that becomes SCPs to swapping to other SCPs
        /// </summary>
        [Description("Should players that swap onto SCPs be able to swap?")]
        public bool VolunteersCanScpSwapToOtherScps { get; set; } = true;
        
        /// <summary>
        /// Enables/Disables the SCP Swap plugin based off of permissions. Permission is "scpswap.allowed"
        /// </summary>
        [Description(
            "Enables/Disables the plugin for users based off of a permission they have, the permission is scpswap.allowed (Covers both Swapping & Volunteering")]
        public bool AllowUserSwapByPermission { get; set; } = false;

        [Description("Preserve 079 XP on swap?")] 
        public static bool Preserve079XpOnSwap { get; set; } = true;
        
        [Description("The maximum time after the round start, in seconds, that a quitting SCP can cause the volunteer opportunity announcement (defaults to 60)")]
        public static int QuitCutoff { get; set; } = 60;

        [Description("The maximum time after the round start, in seconds, that a player can use the .volunteer command (defaults to 90)")]
        public static int ReplaceCutoff { get; set; } = 90;

        [Description("The required percentage of health (0-100) the SCP must have had to be eligible for replacement. Defaults to 95 (no percent sign)")]
        public static int RequiredHealthPercentage { get; set; } = 95;

        [Description("How long (in seconds) after the first player volunteers should the SCP be replaced")]
        public int LotteryPeriodSeconds { get; set; } = 10;
    }
}