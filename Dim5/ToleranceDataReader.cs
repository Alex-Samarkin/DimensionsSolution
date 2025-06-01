using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dim5
{
    public static class ToleranceDataReader
    {
        private static ToleranceDataJson _data;

        public static async Task LoadDataAsync(string filePath = "ISO286_Tolerances.json")
        {
            if (_data != null) return;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл {filePath} не найден.");

            string json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
            _data = JsonSerializer.Deserialize<ToleranceDataJson>(json, options);
        }

        public static Dictionary<string, double> GetHoleDeviations(string intervalKey)
        {
            return _data.Intervals.Find(i => i.Range == intervalKey)?.HoleDeviations
                   ?? throw new KeyNotFoundException($"Интервал {intervalKey} не найден");
        }

        public static Dictionary<string, double> GetShaftDeviations(string intervalKey)
        {
            return _data.Intervals.Find(i => i.Range == intervalKey)?.ShaftDeviations
                   ?? throw new KeyNotFoundException($"Интервал {intervalKey} не найден");
        }
    }
}
