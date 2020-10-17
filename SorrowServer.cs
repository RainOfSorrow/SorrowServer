using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using OTAPI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using Utils = Terraria.Utils;

namespace SorrowServer
{
    [ApiVersion(2, 1)]
    public class SorrowServer : TerrariaPlugin
    {
        public override string Name => "SorrowServer";
        public override string Description => "Not important plugin at all";
        public override string Author => "Xedlefix";
        public override Version Version => new Version(1, 0, 0,0);


        public Random PluginRandom;
        
        public Config config = new Config();
        
        public SorrowServer(Main game) : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("sorrow.npcblocker", NpcBlockerCommand, "npcblocker", "nb"));
            
            TShockAPI.GetDataHandlers.ChestOpen.Register(ChestOpen);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData, -10);
            OTAPI.Hooks.Npc.PostUpdate += NpcPostUpdate;
            ServerApi.Hooks.NpcKilled.Register(this, NpcKilled);
            ServerApi.Hooks.NetGreetPlayer.Register(this, GreetPlayer);

            config = Config.Read("config.json");
            config.Write("config.json");

            PluginRandom = new Random();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TShockAPI.GetDataHandlers.ChestOpen.UnRegister(ChestOpen);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                OTAPI.Hooks.Npc.PostUpdate -= NpcPostUpdate;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, GreetPlayer);
            }
            base.Dispose(disposing);
        }

        private void NpcKilled(NpcKilledEventArgs npc)
        {

            int relic = 0;
            switch (npc.npc.type)
            {
                case NPCID.KingSlime:
                    relic = 4929;
                    break;
                case NPCID.EyeofCthulhu:
                    relic = 4924;
                    break;
                case NPCID.EaterofWorldsHead:
                case NPCID.EaterofWorldsTail:
                    relic = 4925;
                    break;
                case NPCID.BrainofCthulhu:
                    relic = 4926;
                    break;
                case NPCID.QueenBee:
                    relic = 4928;
                    break;
                case NPCID.SkeletronHead:
                    relic = 4927;
                    break;
                case NPCID.WallofFlesh:
                    relic = 4930;
                    break;
                case NPCID.QueenSlimeBoss:
                    relic = 4950;
                    break;
                case NPCID.TheDestroyer:
                    relic = 4932;
                    break;
                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    relic = 4931;
                    break;
                case NPCID.SkeletronPrime:
                    relic = 4933;
                    break;
                case NPCID.Plantera:
                    relic = 4934;
                    break;
                case NPCID.HallowBoss:
                    relic = 4949;
                    break;
                case NPCID.Golem:
                    relic = 4935;
                    break;
                case NPCID.DukeFishron:
                    relic = 4936;
                    break;
                case NPCID.CultistBoss:
                    relic = 4937;
                    break;
                case NPCID.MoonLordCore:
                    relic = 4938;
                    break;
                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                    relic = 4946;
                    break;
                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                    relic = 4947;
                    break;
                case NPCID.DD2Betsy:
                    relic = 4948;
                    break;
                case NPCID.MartianSaucerCore:
                    relic = 4939;
                    break;
                case NPCID.MourningWood:
                    relic = 4941;
                    break;
                case NPCID.Pumpking:
                    relic = 4942;
                    break;
                case NPCID.Everscream:
                    relic = 4944;
                    break;
                case NPCID.IceQueen:
                    relic = 4943;
                    break;
                case NPCID.SantaNK1:
                    relic = 4945;
                    break;
                case NPCID.PirateShipCannon:
                    relic = 4940;
                    break;
            }

            Item.NewItem(npc.npc.position, Vector2.One, relic, 1);

        }

        private async void GreetPlayer(GreetPlayerEventArgs args)
        {
            var plr = TShock.Players[args.Who];

            await Task.Delay(500).ContinueWith(l =>
            {
                Projectile.NewProjectile(plr.TPlayer.position.X, plr.TPlayer.position.Y - 32, 0f, -8f, PluginRandom.Next(168, 170),
                    0, 0);
                plr.SendData(PacketTypes.CreateCombatTextExtended, "Witamy na serwerze! ;)",
                    (int) Colors.RarityPurple.PackedValue, plr.X, plr.Y);
            });
        }


        private void NpcPostUpdate(NPC npc, int i)
        {
            if (npc == null || i > Main.npc.Length - 1 || i < 0  || !npc.active)
                return;
            
            if (config.BlockedNPCs.Contains(npc.type))
            {
                Main.npc[i] = new NPC();
                NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, i);
            }
        }


        private void NpcBlockerCommand(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
                switch (args.Parameters[0])
                {
                    case "allow":
                    {
                        int id = 0;

                        if (int.TryParse(args.Parameters[1], out id))
                        {
                            if (!config.BlockedNPCs.Contains(id))
                            {
                                args.Player.SendErrorMessage($"Lista nie zawiera {id}" +  (id < 0 ? "." : $" czyli {TShock.Utils.GetNPCById(id).FullName}."));
                                return;
                            }
                            
                            config.BlockedNPCs.Remove(id);
                            config.Write("config.json");
                            args.Player.SendSuccessMessage($"Pomyslnie zezwolono {id}" +  (id < 0 ? "." : $" czyli {TShock.Utils.GetNPCById(id).FullName}."));
                            
                            return;
                        }

                        List<NPC> npcs = TShock.Utils.GetNPCByName(args.Parameters[1]);

                        if (npcs.Count > 1)
                        {
                            args.Player.SendErrorMessage($"Znaleziono wiecej niz 1 wynik dla \"{args.Parameters[1]}\"");
                            args.Player.SendMultipleMatchError(npcs);
                            return;
                        }
                        
                        if (npcs.Count < 1)
                        {
                            args.Player.SendErrorMessage($"Nie zaleziono nic dla \"{args.Parameters[1]}\"");
                            return;
                        }
                        
                        if (!config.BlockedNPCs.Contains(npcs[0].type))
                        {
                            args.Player.SendErrorMessage($"Lista nie zawiera {npcs[0].type}" +  (id < 0 ? "." : $" czyli {npcs[0].FullName}."));
                            return;
                        }
                        
                        config.BlockedNPCs.Remove(npcs[0].type);
                        config.Write("config.json");
                        args.Player.SendSuccessMessage($"Pomyslnie zezwolono {npcs[0].type}" +  (id < 0 ? "." : $" czyli {npcs[0].FullName}."));
                        return;
                    }
                    case "deny":
                    {
                        int id = 0;

                        if (int.TryParse(args.Parameters[1], out id))
                        {
                            if (config.BlockedNPCs.Contains(id))
                            {
                                args.Player.SendErrorMessage($"Lista juz zawiera {id}" +  (id < 0 ? "." : $" czyli {TShock.Utils.GetNPCById(id).FullName}."));
                                return;
                            }
                            
                            config.BlockedNPCs.Add(id);
                            config.Write("config.json");
                            args.Player.SendSuccessMessage($"Pomyslnie zabroniono {id}" +  (id < 0 ? "." : $" czyli {TShock.Utils.GetNPCById(id).FullName}."));
                            
                            return;
                        }

                        List<NPC> npcs = TShock.Utils.GetNPCByName(args.Parameters[1]);

                        if (npcs.Count > 1)
                        {
                            args.Player.SendErrorMessage($"Znaleziono wiecej niz 1 wynik dla \"{args.Parameters[1]}\"");
                            args.Player.SendMultipleMatchError(npcs);
                            return;
                        }
                        
                        if (npcs.Count < 1)
                        {
                            args.Player.SendErrorMessage($"Nie zaleziono nic dla \"{args.Parameters[1]}\"");
                            return;
                        }
                        
                        if (config.BlockedNPCs.Contains(npcs[0].type))
                        {
                            args.Player.SendErrorMessage($"Lista juz zawiera {id}" +  (id < 0 ? "." : $" czyli {NPC.getNewNPCName(id)}."));
                            return;
                        }
                        
                        config.BlockedNPCs.Add(npcs[0].type);
                        config.Write("config.json");
                        args.Player.SendSuccessMessage($"Pomyslnie zabroniono {npcs[0].type}" +  (id < 0 ? "." : $" czyli {npcs[0].FullName}."));
                        
                        return;
                    }
                    case "reload":
                    {
                        config = Config.Read("config.json");
                        args.Player.SendSuccessMessage("Pomyslnie przeladowano config.json");
                        return;
                    }
                }

            args.Player.SendSuccessMessage("NpcBlocker:");
            args.Player.SendInfoMessage("- /nb allow <name/id>");
            args.Player.SendInfoMessage("- /nb deny <name/id>");
            args.Player.SendInfoMessage("- /nb reload");
        }


        private void OnGetData(GetDataEventArgs args)
        {
            if (args.Handled)
                return;

            if (args.MsgID == PacketTypes.SpawnBossorInvasion)
            {
                using (BinaryReader reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                {
                    var tSPlayer = TShock.Players[reader.ReadInt16()];
                    var npcid = reader.ReadInt16();

                    if (config.BlockedNPCs.Contains(npcid))
                    {
                        args.Handled = true;
                        tSPlayer.SendErrorMessage("Ten mob jest aktualnie niedostepny.");
                    }
                    
                }
            }
        }
        

        //Chest protection on protected regions
        private void ChestOpen(object sender, GetDataHandlers.ChestOpenEventArgs e)
        {
            if (e.Handled)
                return;

            e.Handled = !e.Player.HasBuildPermission(e.X, e.Y);
        }
    }
    
}