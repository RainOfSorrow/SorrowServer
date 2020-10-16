using System;
using System.IO;
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

        public SorrowServer(Main game) : base(game)
        {
            Order = 1;
            
        }

        public override void Initialize()
        {
            TShockAPI.GetDataHandlers.ChestOpen.Register(ChestOpen);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData, -10);
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
                    
                    if ()
                    
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