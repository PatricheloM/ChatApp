using System.IO;
using Newtonsoft.Json;

namespace ChatApp.Json
{
    class JsonHandling<T>
    {
        public T JsonObject { get; private set; }

        protected JsonHandling(string path)
        {
            string json;
            using (StreamReader r = new StreamReader(path))
            {
                json = r.ReadToEnd();
            }
            JsonObject = JsonConvert.DeserializeObject<T>(json);
        }
    }
}
