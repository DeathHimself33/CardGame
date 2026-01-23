using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardGame;
public static partial class GameDataLoader
{
    public static IReadOnlyDictionary<string,RelicDef> LoadRelics(string relicsFolder)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
        options.Converters.Add(new JsonStringEnumConverter());

        var dict = new Dictionary<string, RelicDef>(StringComparer.OrdinalIgnoreCase);

        foreach(var path in Directory.EnumerateFileSystemEntries(relicsFolder, "*.json", SearchOption.AllDirectories))
        {
            try
            {
                var json = File.ReadAllText(path);
                var def = JsonSerializer.Deserialize<RelicDef>(json,options)
                    ?? throw new Exception($"Failed to load {path}");
                if(string.IsNullOrWhiteSpace(def.ID))
                {
                    throw new Exception($"Relic missing id: {path}");
                }
                if(!dict.TryAdd(def.ID, def))
                {
                    throw new Exception($"Duplicate relic id: {def.ID} (file: {path})");
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed parsing relic file: {path}", ex);
            }
        }
        return dict;
    }
}