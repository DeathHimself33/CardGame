#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame;

public sealed class CombatContext
{
    private readonly Action<Character>? _onCharacterDamaged;

    private readonly Character _playerCharacter;
    private readonly IReadOnlyList<Character> _enemyCharacters;

    public CombatContext(Character player, IReadOnlyList<Character> enemies, Action<Character>? onCharacterDamaged = null)
    {
        _playerCharacter = player;
        _enemyCharacters = enemies;
        _onCharacterDamaged = onCharacterDamaged;
    }

    public void TriggerRelics(Character owner, TriggerEvent trigger, CombatContext context, object? eventData = null)
    {
        foreach (var relic in owner.Relics)
            relic.OnEvent(trigger, owner, context, eventData);
    }

    public void NotifyCharacterDamaged(Character damagedCharacter)
        => _onCharacterDamaged?.Invoke(damagedCharacter);

    public IReadOnlyList<Character> GetEnemiesOf(Character character)
    {
        if (ReferenceEquals(character, _playerCharacter))
            return _enemyCharacters;

        if (_enemyCharacters.Contains(character))
            return new[] { _playerCharacter };

        throw new ArgumentException("Character not part of this combat context.", nameof(character));
    }
    
    //                  LOGS

    public sealed record LogEntry(DateTime Time, string Message);

    private readonly List<LogEntry> _log = new();
    public IReadOnlyList<LogEntry> Log => _log;

    public void AddLog(string message) => _log.Add(new LogEntry(DateTime.UtcNow, message));
    public void ClearLog() => _log.Clear();
}
