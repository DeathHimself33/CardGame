using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardGame;
public static partial class GameDataLoader
{
    public static IReadOnlyDictionary<string,CardDef> LoadCards(string cardsFolder)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
        options.Converters.Add(new JsonStringEnumConverter());

        var dict = new Dictionary<string, CardDef>(StringComparer.OrdinalIgnoreCase);

        foreach(var path in Directory.EnumerateFileSystemEntries(cardsFolder, "*.json", SearchOption.AllDirectories))
        {
            try
            {
                var json = File.ReadAllText(path);
                var def = JsonSerializer.Deserialize<CardDef>(json,options)
                    ?? throw new Exception($"Failed to load {path}");
                if(string.IsNullOrWhiteSpace(def.ID))
                {
                    throw new Exception($"Card missing id: {path}");
                }
                if(!dict.TryAdd(def.ID, def))
                {
                    throw new Exception($"Duplicate card id: {def.ID} (file: {path})");
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed parsing card file: {path}", ex);
            }
        }
        return dict;
    }
}
