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
                ValidateBase(def,path);
                ResolveUpgrade(def);
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

    private static void ResolveUpgrade(CardDef baseDef)
    {
        if(baseDef.Upgraded == null)
            return;

        var up = baseDef.Upgraded;

        up.ID = baseDef.ID;
        up.TargetType = baseDef.TargetType;
        up.Rarity = baseDef.Rarity;
        up.Type = baseDef.Type;

        // Defaults
        up.Name = string.IsNullOrWhiteSpace(up.Name)
            ? baseDef.Name + "+"
            : up.Name;

        up.Description ??= baseDef.Description;
        up.Cost = up.Cost == 0 ? baseDef.Cost : up.Cost;

        if (up.Ops == null || up.Ops.Count == 0)
            throw new Exception($"Upgraded card '{baseDef.ID}' has no ops");
    }

    // VALIDATION (Everyone loves a bit of validation fr fr)
    private static void ValidateBase(CardDef def, string path)
    {
        if(def.Ops == null || def.Ops.Count == 0)
            throw new Exception($"Card '{def.ID}' has no ops (path: {path})");
    }
}
