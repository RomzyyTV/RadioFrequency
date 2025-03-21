using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using RadioFrequency.Features;
using UserSettings.ServerSpecific;

namespace RadioFrequency
{
    internal static class EventHandlers
    {
        private static int _keybindId;

        internal static void RegisterEvents()
        {
            _keybindId = Plugin.Singleton.Config.KeybindId;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.ItemAdded += OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved += OnItemRemoved;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
        }

        internal static void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Player.ItemAdded -= OnItemAdded;
            Exiled.Events.Handlers.Player.ItemRemoved -= OnItemRemoved;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
        }

        private static void OnWaitingForPlayers()
        {
            Frequency.RadioFrequency.Clear();

            foreach (Frequency frequency in Frequency.Frequencies)
            {
                frequency.Players.Clear();
            }
        }

        private static void OnLeft(LeftEventArgs ev)
        {
            Player player = ev.Player;

            if (Frequency.TryGetPlayerFrequency(player, out Frequency frequency))
                frequency.RemovePlayer(player);
        }

        private static void OnItemAdded(ItemAddedEventArgs ev)
        {
            Player player = ev.Player;
            Item item = ev.Item;

            if (item == null || item.Type != ItemType.Radio)
                return;

            if (Frequency.TryGetRadioFrequency(item.Serial, out Frequency radioFrequency) && (radioFrequency.AuthorizedRoles.Contains(player.Role.Type) || radioFrequency.CanBePickedUp))
            {
                radioFrequency.AddPlayer(player);
            }
            else
            {
                if (!Frequency.TryGetFrequenciesByRole(player.Role.Type, out List<Frequency> frequencies) || frequencies.Count <= 0)
                    return;

                Frequency frequency = frequencies.First();
                frequency.AddPlayer(player);

                Frequency.SetRadioFrequency(item.Serial, frequency);
            }
        }

        private static void OnItemRemoved(ItemRemovedEventArgs ev)
        {
            Player player = ev.Player;
            Item item = ev.Item;

            if (item == null || item.Type != ItemType.Radio || player.Items.Any(i => i.Type == ItemType.Radio))
                return;

            if (Frequency.TryGetPlayerFrequency(player, out Frequency frequency))
                frequency.RemovePlayer(player);
        }

        private static void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (settingBase is not SSKeybindSetting keybindSetting || keybindSetting.SettingId != _keybindId || !keybindSetting.SyncIsPressed)
                return;

            if (!Player.TryGet(hub, out Player player)) 
                return;

            if (player.Items.All(i => i.Type != ItemType.Radio))
            {
                player.ShowHint(Plugin.Singleton.Config.NoRadioHint);
                return;
            }

            if (!Frequency.TryGetPlayerFrequency(player, out Frequency frequency))
                return;

            Frequency nextFrequency = Frequency.GetNextFrequency(player.Role.Type, frequency);

            frequency.RemovePlayer(player);
            nextFrequency.AddPlayer(player);

            foreach (Item playerItem in player.Items)
            {
                if (playerItem.Type == ItemType.Radio)
                {
                    Frequency.SetRadioFrequency(playerItem.Serial, nextFrequency);
                }
            }

            player.ShowHint(Plugin.Singleton.Config.ChangedFrequencyHint.Replace("{radio_frequency}", nextFrequency.Name));
        }
    }
}