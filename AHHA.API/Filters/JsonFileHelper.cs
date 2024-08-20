using Newtonsoft.Json;

namespace AHHA.API.Filters
{
    public static class JsonFileHelper
    {
        private static readonly string JsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "people.json");

        public static List<T> ReadFromJsonFile<T>()
        {
            using StreamReader file = File.OpenText(JsonFilePath);
            JsonSerializer serializer = new JsonSerializer();
            return (List<T>)serializer.Deserialize(file, typeof(List<T>));
        }

        public static void WriteToJsonFile<T>(List<T> data)
        {
            using StreamWriter file = File.CreateText(JsonFilePath);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, data);
        }
    }
}