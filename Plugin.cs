using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using Microsoft.Xna.Framework;
using System.Text.Json;
using Terraria.GameContent.Creative;
using System.Diagnostics;
using IL.Terraria.DataStructures;
using Terraria.Net;
using IL.Terraria.Net;

namespace HCDropHearts
{
    [ApiVersion(2, 1)]
    public class HCDropHearts : TerrariaPlugin
    {

        public override string Author => "Onusai";
        public override string Description => "Hardcore characters drop hearts upon detah";
        public override string Name => "HCDropHearts";
        public override Version Version => new Version(1, 0, 0, 0);

        public class ConfigData
        {
            public bool temp { get; set; } = false;
        }

        ConfigData configData;

        public HCDropHearts(Main game) : base(game) { }

        public override void Initialize()
        {
            //configData = PluginConfig.Load("HCDropHearts");

            ServerApi.Hooks.GameInitialize.Register(this, OnGameLoad);
        }

        void OnGameLoad(EventArgs e)
        {
            TShockAPI.GetDataHandlers.KillMe += OnPlayerDeath;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnGameLoad);
                TShockAPI.GetDataHandlers.KillMe -= OnPlayerDeath;
            }
            base.Dispose(disposing);
        }

        void RegisterCommand(string name, string perm, CommandDelegate handler, string helptext)
        {
            TShockAPI.Commands.ChatCommands.Add(new Command(perm, handler, name)
            { HelpText = helptext });
        }

        void OnPlayerDeath(object sender, TShockAPI.GetDataHandlers.KillMeEventArgs args)
        {
            var player = TShock.Players[args.PlayerId];

            if (player.Difficulty != 2) return;
            
            int drop_amount = (player.TPlayer.statLifeMax - 100) / 20;

            if (drop_amount == 0) return;

            player.GiveItem(29, drop_amount);
            
        }

        void GiveItemByDrop(TSPlayer player, int type, int stack, int prefix)
        {
            //int itemIndex = Item.NewItem(null, (int)player.X, (int)player.Y, player.TPlayer.width, player.TPlayer.height, type, stack, true, prefix, true);
            //Main.item[itemIndex].playerIndexTheItemIsReservedFor = player.TPlayer.whoAmI;
            //SendData((int)PacketTypes.ItemDrop, -1, -1, null, "", itemIndex, 1);
            //NetMessage.SendData((int)PacketTypes.ItemOwner, null, itemIndex);
            //////NetMessage.SendData()
            //TShockAPI.Net.Send
        }

        public static class PluginConfig
        {
            public static string filePath;
            public static ConfigData Load(string Name)
            {
                filePath = String.Format("{0}/{1}.json", TShock.SavePath, Name);

                if (!File.Exists(filePath))
                {
                    var data = new ConfigData();
                    Save(data);
                    return data;
                }

                var jsonString = File.ReadAllText(filePath);
                var myObject = JsonSerializer.Deserialize<ConfigData>(jsonString);

                return myObject;
            }

            public static void Save(ConfigData myObject)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(myObject, options);

                File.WriteAllText(filePath, jsonString);
            }
        }

    }
}