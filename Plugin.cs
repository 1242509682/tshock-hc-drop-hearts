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
            var plr = TShock.Players[args.Player.Index];
            int drop_amount = (plr.TPlayer.statLifeMax - 100) / 20;

            if (plr.Difficulty != 2 || drop_amount == 0) return;

            int itemIndex = Item.NewItem(null, (int)plr.X, (int)plr.Y, plr.TPlayer.width, plr.TPlayer.height, 29, drop_amount, false, 0, true);
            // Item.NewItem appears to send packet from server by default
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
