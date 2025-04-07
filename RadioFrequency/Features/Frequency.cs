using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using YamlDotNet.Serialization;

namespace RadioFrequency.Features
{
    public class Frequency
    {
        internal static readonly List<Frequency> Frequencies = new();

        internal static readonly Dictionary<ushort, Frequency> RadioFrequency = new();

        internal static readonly Dictionary<RoleTypeId, List<Frequency>> FrequenciesByRole = new();

        internal static readonly Dictionary<Player, Frequency> PlayerFrequency = new();

        public Frequency(string name, HashSet<RoleTypeId> authorizedRoles, bool canBePickedUp)
        {
            Name = name;
            AuthorizedRoles = authorizedRoles;
            CanBePickedUp = canBePickedUp;
        }

        public Frequency()
        {
        }

        [Description("Name of the frequency.")]
        public string Name { get; set; }

        [Description("Roles that can access this frequency. If empty, then everyone can access this frequency.")]
        public HashSet<RoleTypeId> AuthorizedRoles { get; set; }

        [Description("Whenever a radio is dropped, its current frequency is saved.\n" +
                     "If this config is true, then a player with an unauthorized role will be able to talk on this frequency if he picks up the radio.\n" +
                     "This config is ignored if there is no authorized roles")]
        public bool CanBePickedUp { get; set; }

        [YamlIgnore]
        public HashSet<Player> Players { get; } = new();

        public static void SetRadioFrequency(ushort serial, Frequency frequency) => RadioFrequency[serial] = frequency;

        public static bool TryGetRadioFrequency(ushort serial, out Frequency frequency) => RadioFrequency.TryGetValue(serial, out frequency);

        public static bool TryGetFrequenciesByRole(RoleTypeId role, out List<Frequency> frequencies) =>
            FrequenciesByRole.TryGetValue(role, out frequencies);

        public static bool TryGetPlayerFrequency(Player player, out Frequency frequency) => PlayerFrequency.TryGetValue(player, out frequency);

        public static Frequency GetNextFrequency(RoleTypeId role, Frequency currentFrequency)
        {
            if (!TryGetFrequenciesByRole(role, out List<Frequency> frequencies))
                return null;

            int index = frequencies.IndexOf(currentFrequency);

            return frequencies[index == -1 ? 0 : (index + 1) % frequencies.Count];
        }
        
        public static string GetFrenquencyName(Player player)
        {
            return TryGetPlayerFrequency(player, out var frequency) ? frequency.Name : Plugin.Singleton.Config.NoneFrequency;
        }

        public void Init()
        {
            Frequencies.Add(this);

            if (AuthorizedRoles.Count <= 0)
            {
                AuthorizedRoles = EnumUtils<RoleTypeId>.Values.ToHashSet();
            }

            foreach (RoleTypeId role in AuthorizedRoles)
            {
                if (!FrequenciesByRole.TryGetValue(role, out List<Frequency> frequencies))
                {
                    frequencies = new List<Frequency>();
                    FrequenciesByRole[role] = frequencies;
                }

                frequencies.Add(this);
            }
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);

            if (PlayerFrequency.ContainsKey(player))
                return;

            PlayerFrequency.Add(player, this);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            PlayerFrequency.Remove(player);
        }
    }
}