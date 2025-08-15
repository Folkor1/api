using System.Text.Json;
using Co2Calc.Engine;

namespace Co2.Api.Factors;

public sealed class JsonFactorProvider : IFactorProvider
{
    // Кастомный comparer для ключа-кортежа (case-insensitive по всем строкам)
    private sealed class KeyComparer : IEqualityComparer<(string category, string region, string unit)>
    {
        public bool Equals((string category, string region, string unit) x, (string category, string region, string unit) y) =>
            string.Equals(x.category, y.category, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.region,   y.region,   StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.unit,     y.unit,     StringComparison.OrdinalIgnoreCase);

        public int GetHashCode((string category, string region, string unit) k) =>
            HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(k.category ?? string.Empty),
                StringComparer.OrdinalIgnoreCase.GetHashCode(k.region   ?? string.Empty),
                StringComparer.OrdinalIgnoreCase.GetHashCode(k.unit     ?? string.Empty)
            );
    }

    private readonly Dictionary<(string category, string region, string unit), (decimal value, string id)> _map
        = new(new KeyComparer());

    public JsonFactorProvider(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Factors file not found: {path}");

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<FactorFile>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new FactorFile();

        foreach (var f in data.Factors)
        {
            var key = (f.Category, f.Region, f.Unit);
            _map[key] = (f.Value, f.Source);
        }

        // дублируем DEFAULT как запасной ключ
        foreach (var f in data.Factors.Where(x => x.Region.Equals("DEFAULT", StringComparison.OrdinalIgnoreCase)))
        {
            var key = (f.Category, "DEFAULT", f.Unit);
            _map.TryAdd(key, (f.Value, f.Source));
        }
    }

    public (decimal value, string id) Get(string category, string region, string unit)
    {
        if (_map.TryGetValue((category, region, unit), out var v))
            return v;
        if (_map.TryGetValue((category, "DEFAULT", unit), out v))
            return v;
        throw new KeyNotFoundException($"No factor for {category}/{region}/{unit}");
    }
}
