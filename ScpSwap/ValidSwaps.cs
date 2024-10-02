// -----------------------------------------------------------------------
// <copyright file="ValidSwaps.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpSwap
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScpSwap.Models;

    /// <summary>
    /// Handles queries that expose the names of swappable classes and config blacklists.
    /// </summary>
    public static class ValidSwaps
    {
        private static readonly List<string> NamesValue = new List<string>();
        private static readonly List<CustomSwap> CustomSwapsValue = new List<CustomSwap>();
        private static readonly List<RoleTypeId> DefaultSwapsValue = new List<RoleTypeId>();
        private static readonly Dictionary<string, RoleTypeId> TranslatableSwapsValue = new Dictionary<string, RoleTypeId>();

        /// <summary>
        /// Gets a collection of all available swap names.
        /// </summary>
        public static ReadOnlyCollection<string> Names => NamesValue.AsReadOnly();

        /// <summary>
        /// Gets a collection of all available custom swaps.
        /// </summary>
        public static ReadOnlyCollection<CustomSwap> CustomSwaps => CustomSwapsValue.AsReadOnly();

        /// <summary>
        /// Gets a collection of all default swaps.
        /// </summary>
        public static ReadOnlyCollection<RoleTypeId> DefaultSwaps => DefaultSwapsValue.AsReadOnly();

        /// <summary>
        /// Gets a collection of all translatable swaps.
        /// </summary>
        public static ReadOnlyDictionary<string, RoleTypeId> TranslatableSwaps => new ReadOnlyDictionary<string, RoleTypeId>(TranslatableSwapsValue);

        /// <summary>
        /// Attempts to get a <see cref="RoleTypeId"/> from <see cref="Translation.TranslatableSwaps"/> or from directly parsing a request.
        /// </summary>
        /// <param name="request">The query to get the <see cref="RoleTypeId"/>.</param>
        /// <returns>The found <see cref="RoleTypeId"/>.</returns>
        public static RoleTypeId Get(string request)
        {
            if (TranslatableSwaps.TryGetValue(request, out RoleTypeId roleType))
                return roleType;

            if (Enum.TryParse(request, true, out roleType) && DefaultSwaps.Contains(roleType))
                return roleType;

            return RoleTypeId.None;
        }

        /// <summary>
        /// Returns a <see cref="CustomSwap"/> with a matching name to the provided one.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The found <see cref="CustomSwap"/> or null if one is not found.</returns>
        public static CustomSwap GetCustom(string name)
        {
            foreach (CustomSwap customSwap in CustomSwaps)
            {
                if (string.Equals(name, customSwap.Name, StringComparison.OrdinalIgnoreCase))
                    return customSwap;
            }

            return null;
        }

        /// <summary>
        /// Checks a <see cref="Player"/> to see if they have a custom class.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns>The found <see cref="CustomSwap"/> or null if one is not found.</returns>
        public static CustomSwap GetCustom(Player player)
        {
            foreach (CustomSwap customSwap in CustomSwaps)
            {
                if (customSwap.VerificationMethod(player))
                    return customSwap;
            }

            return null;
        }

        /// <summary>
        /// Clears and adds all available names of roles to swap to the cache.
        /// </summary>
        public static void Refresh()
        {
            NamesValue.Clear();

            RefreshCustomSwaps();
            RefreshTranslatableSwaps();
            RefreshDefaultSwaps();
        }

        private static void RefreshCustomSwaps()
        {
            CustomSwapsValue.Clear();
            foreach (CustomSwap customSwap in CustomSwap.Registered)
            {
                if (Plugin.Instance.Config.BlacklistedNames != null &&
                    Plugin.Instance.Config.BlacklistedNames.Contains(customSwap.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                CustomSwapsValue.Add(customSwap);
                NamesValue.Add(customSwap.Name);
            }
        }

        private static void RefreshTranslatableSwaps()
        {
            TranslatableSwapsValue.Clear();
            if (Plugin.Instance.Translation.TranslatableSwaps == null)
                return;

            foreach (KeyValuePair<string, RoleTypeId> kvp in Plugin.Instance.Translation.TranslatableSwaps)
            {
                if ((Plugin.Instance.Config.BlacklistedSwapToScps != null && Plugin.Instance.Config.BlacklistedSwapToScps.Contains(kvp.Value))
                    || kvp.Value.GetTeam() != Team.SCPs)
                    continue;

                if (NamesValue.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Debug($"Failed to add a translation that was a duplicate of another swap with the name of {kvp.Key}.");
                    continue;
                }

                TranslatableSwapsValue.Add(kvp.Key, kvp.Value);
                NamesValue.Add(kvp.Key);
            }
        }

        private static void RefreshDefaultSwaps()
        {
            DefaultSwapsValue.Clear();
            foreach (RoleTypeId role in Enum.GetValues(typeof(RoleTypeId)))
            {
                if ((Plugin.Instance.Config.BlacklistedSwapToScps != null && Plugin.Instance.Config.BlacklistedSwapToScps.Contains(role))
                    || role.GetTeam() != Team.SCPs)
                    continue;

                string roleText = role.ToString();
                if (NamesValue.Contains(roleText, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Debug($"Failed to add a translation that was a duplicate of another swap with the name of {roleText}.");
                    continue;
                }

                DefaultSwapsValue.Add(role);
                NamesValue.Add(roleText);
            }
        }
    }
}