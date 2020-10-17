using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SorrowServer
{
    public class Config
    {

        public List<int> BlockedNPCs = new List<int>()
        {
            50, //King Slime
            4, // EOC
            13, //EOW
            14,
            15,
            266, //BOC
            267,
            222, //Bee
            35, //Skeletron
            113, //WOF
            657, //Queen Slime
            125, //Twins
            126,
            134, //Destroyer
            135,
            136,
            127, //Prime
            262, //Plantera
            246, //GOlem
            245,
            249,
            636, //EOL
            370, //Duke Fishron
            439, //Lunatic
            398 //Moon Lord
            -1,
            -2,
            -3,
            -4,
            -5,
            -6,
            -7,
            -8,
            -10
        };
        
        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path)
        {
            return File.Exists(path) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) : new Config();
        } 
    }
}