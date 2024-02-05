using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodBot.Servise
{
    public static class CRUD
    {
        public static async Task<List<T>> GetAllInfo<T>(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return await Deserialize<T>(await sr.ReadToEndAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting info from {path}: {ex.Message}");
                return new List<T>();
            }
        }

        public static async Task UpdateDB(string path, string value)
        {
            try
            {
                await File.WriteAllTextAsync(path, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating DB at {path}: {ex.Message}");
            }
        }
        public static async Task<string> Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static async Task<List<T>> Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }
    }
}
