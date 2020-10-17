using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SorrowServer
{
    public class Config
    {

        public List<int> BlockedNPCs = new List<int>();

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