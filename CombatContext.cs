namespace CardGame;

public sealed class CombatContext
{
    private readonly Action<Character>? _onCharacterDamaged;

    private readonly Character _playerCharacter;
    private readonly Character _enemyCharacter;

    //Cached Lists so GetEnemiesOf doesn't create new lists each call
    private readonly IReadOnlyList<Character> _playerEnemies;
    private readonly IReadOnlyList<Character> _enemyEnemies;   
    public CombatContext(Character player, Character enemy, Action<Character>? onCharacterDamaged = null)
    {
        _playerCharacter = player;
        _enemyCharacter = enemy;
        _onCharacterDamaged = onCharacterDamaged;

        _playerEnemies = new[] { _enemyCharacter };
        _enemyEnemies = new[] { _playerCharacter };
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
        if(ReferenceEquals(character, _playerCharacter))
        {
            return _playerEnemies;
        }
        else if(ReferenceEquals(character, _enemyCharacter))
        {
            return _enemyEnemies;
        }
        else
        {
            throw new ArgumentException("Character not part of this combat context.", nameof(character));
        }
    }
}
