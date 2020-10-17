using System;
using System.IO;
using System.Linq;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace SorrowServer
{
    public class SorrowServer : TerrariaPlugin
    {
        public override string Name => "SorrowServer";
        public override string Description => "Not important plugin at all";
        public override string Author => "Xedlefix";
        public override Version Version => new Version("1.0.0");

        
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

            config = Config.Read("config.json");
            config.Write("config.json");
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
                            if (config.BlockedNPCs.Contains(id))
                                
                            
                            return;
                        }
                        
                        
                        
                        
                        return;
                    }
                    case "deny":
                    {
                        
                        return;
                    }
                }

            args.Player.SendInfoMessage("NpcBlocker:");
            args.Player.SendSuccessMessage("- /nb allow <name/id>");
            args.Player.SendSuccessMessage("- /nb deny <name/id>");
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