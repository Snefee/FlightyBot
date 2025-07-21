using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightyBot.config
{
    internal class JSONReader
    {
        public string token { get; set; }
        public string aeroDataBoxApiKey { get; set; }
        public string mapBoxApiKey { get; set; }
        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.token = data.token;
                this.aeroDataBoxApiKey = data.aeroDataBoxApiKey;
                this.mapBoxApiKey = data.mapBoxApiKey;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string token { get; set; }
        public string aeroDataBoxApiKey { get; set; }
        public string mapBoxApiKey { get; set; }
    }
}
