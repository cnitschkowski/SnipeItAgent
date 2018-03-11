using System.IO;
using Newtonsoft.Json;

namespace SnipeItAgent
{
    public class JsonConfigSource : IConfigSource
    {
        public string FilePath { get; set; }
        
        public Config Read()
        {
            var serializer = new JsonSerializer();
            using (var file = File.OpenText(this.FilePath))
            {
                return serializer.Deserialize(file, typeof(Config)) as Config;
            }
        }
    }
}