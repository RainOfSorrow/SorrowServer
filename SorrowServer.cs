using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            
            GetDataHandlers.ChestOpen.Register(ChestOpen);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData, -10);
            Hooks.Npc.PostUpdate += NpcPostUpdate;
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
                GetDataHandlers.ChestOpen.UnRegister(ChestOpen);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                Hooks.Npc.PostUpdate -= NpcPostUpdate;
                ServerApi.Hooks.NpcKilled.Deregister(this, NpcKilled);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, GreetPlayer);
            }
            base.Dispose(disposing);
        }


        private void NpcKilled(NpcKilledEventArgs args)
        {

            if (!Main.expertMode)
                return;

            int relic = 0;
            int extra = 0;
            switch (args.npc.type)
            {
                case NPCID.KingSlime:
                    relic = 4929;
                    extra = 4797;
                    break;
                case NPCID.EyeofCthulhu:
                    relic = 4924;
                    extra = 4798;
                    break;
                case NPCID.EaterofWorldsTail:
                    if (args.npc.boss)
                    {
                        relic = 4925;
                        extra = 4799;
                    }

                    break;
                case NPCID.BrainofCthulhu:
                    relic = 4926;
                    extra = 4800;
                    break;
                case NPCID.QueenBee:
                    relic = 4928;
                    extra = 4802;
                    break;
                case NPCID.SkeletronHead:
                    relic = 4927;
                    extra = 4801;
                    break;
                case NPCID.WallofFlesh:
                    relic = 4930;
                    extra = 4795;
                    break;
                case NPCID.QueenSlimeBoss:
                    relic = 4950;
                    extra = 4960;
                    break;
                case NPCID.TheDestroyer: //Destroyer is the tricky one
                    relic = 4932;
                    extra = 4803;
                    

                    float num1 = 1E+08f;
                    Vector2 center = Main.player[args.npc.target].Center;
                    Vector2 position = args.npc.position;
                    for (int index = 0; index < 200; ++index)
                    {
                        if (Main.npc[index].active && (Main.npc[index].type == 134 || Main.npc[index].type == 135 || Main.npc[index].type == 136))
                        {
                            float num2 = Math.Abs(Main.npc[index].Center.X - center.X) + Math.Abs(Main.npc[index].Center.Y - center.Y);
                            if (num2 < num1)
                            {
                                num1 = num2;
                                position = Main.npc[index].position;
                            }
                        }
                    }
                    
                    if (PluginRandom.Next(0, 3) == 0)
                        Item.NewItem(position,  args.npc.Size, relic, 1);
                    
                    if (PluginRandom.Next(0, 6) == 0)
                        Item.NewItem(position,  args.npc.Size / 3, extra, 1);
                    return;
                case NPCID.Spazmatism:
                case NPCID.Retinazer:
                    if (args.npc.boss)
                    {
                        relic = 4931;
                        extra = 4804;
                    }
                    break;
                case NPCID.SkeletronPrime:
                    relic = 4933;
                    extra = 4805;
                    break;
                case NPCID.Plantera:
                    relic = 4934;
                    extra = 4806;
                    break;
                case NPCID.HallowBoss:
                    relic = 4949;
                    extra = 4811;
                    break;
                case NPCID.Golem:
                    relic = 4935;
                    extra = 4807;
                    break;
                case NPCID.DukeFishron:
                    relic = 4936;
                    extra = 4808;
                    break;
                case NPCID.CultistBoss:
                    relic = 4937;
                    extra = 4809;
                    break;
                case NPCID.MoonLordCore:
                    relic = 4938;
                    extra = 4810;
                    break;
                case NPCID.DD2DarkMageT1:
                case NPCID.DD2DarkMageT3:
                    relic = 4946;
                    extra = 4796;
                    break;
                case NPCID.DD2OgreT2:
                case NPCID.DD2OgreT3:
                    relic = 4947;
                    extra = 4816;
                    break;
                case NPCID.DD2Betsy:
                    relic = 4948;
                    extra = 4817;
                    break;
                case NPCID.MartianSaucerCore:
                    relic = 4939;
                    extra = 4815;
                    break;
                case NPCID.MourningWood:
                    relic = 4941;
                    extra = 4793;
                    break;
                case NPCID.Pumpking:
                    relic = 4942;
                    extra = 4812;
                    break;
                case NPCID.Everscream:
                    relic = 4944;
                    extra = 4813;
                    break;
                case NPCID.IceQueen:
                    relic = 4943;
                    extra = 4814;
                    break;
                case NPCID.SantaNK1:
                    relic = 4945;
                    extra = 4794;
                    break;
                case NPCID.PirateShip:
                    relic = 4940;
                    extra = 4792;
                    break;
            }

            if (PluginRandom.Next(0, 3) == 0)
                Item.NewItem(args.npc.position + args.npc.Size / 2, args.npc.Size / 3, relic, 1);
            
            if (PluginRandom.Next(0, 6) == 0)
                Item.NewItem(args.npc.position + args.npc.Size / 2,  args.npc.Size / 3, extra, 1);
        }

        private async void GreetPlayer(GreetPlayerEventArgs args)
        {
            var plr = TShock.Players[args.Who];

            await Task.Delay((plr.RPPending + 1) * 1000).ContinueWith(l =>
            {
                if (plr.ConnectionAlive)
                {
                    Projectile.NewProjectile(plr.TPlayer.position.X + 16f, plr.TPlayer.position.Y - 32, 0f, -6f,
                        PluginRandom.Next(167, 171),
                        0, 0);
                    plr.SendData(PacketTypes.CreateCombatTextExtended, "Witaj przyjacielu!",
                        (int) Color.White.PackedValue, plr.X + 16f, plr.Y + 24f);
                }
            });
        }


        private void NpcPostUpdate(NPC npc, int i)
        {
            if (npc == null || i > Main.npc.Length - 1 || i < 0 || !npc.active)
                return;
            
            
            if (config.BlockedNPCs.Contains(npc.type))
            {
                Main.npc[i] = new NPC();
                NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, i);
            }
            
        }


        private void NpcBlockerCommand(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
                switch (args.Parameters[0])
                {
                    case "allow":
                    {
                        if (args.Parameters.Count == 1)
                            break;

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
                        if (args.Parameters.Count == 1)
                            break;
                        
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