namespace CardGame
{
    public abstract partial class Character
    {
        public void DealDamage(CombatContext context, Character target, int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            int finalDamage = amount;
            foreach (var status in this.StatusEffects)
            {
                finalDamage = status.ModifyDamageDealt(finalDamage);
            }
            foreach (var status in target.StatusEffects)
            {
                finalDamage = status.ModifyDamageTaken(finalDamage);
            }
            int damageAfterBlock = Math.Max(0, finalDamage - target.Block);
            target.Block = Math.Max(0, target.Block - finalDamage);
            target.TakeDamageDirect(context, damageAfterBlock);
        }
        public void TakeDamageDirect(CombatContext context, int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            HP -= amount;
            HP = Math.Max(0, HP);
            context.TriggerRelics(this, TriggerEvent.DamageTaken, context);
            context.NotifyCharacterDamaged(this);
        }
        public void GainBlock(CombatContext context, int amount)
        {
            GainBlockRaw(amount);
            context.TriggerRelics(this, TriggerEvent.BlockGained, context);
        }
        public void GainBlockRaw(int amount)
        {
            Block += amount;
        }
        public void Heal(CombatContext context, int amount)
        {
            HealRaw(amount);
            context.TriggerRelics(this, TriggerEvent.Healed, context);
        }
        public void HealRaw(int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            HP = Math.Min(MaxHP, HP + amount);
        }
        public void GainEnergy(int amount)
        {
            if (amount <= 0)
            {
                return;
            }
            Energy += amount;
        }
        public bool SpendEnergy(int amount)
        {
            if (amount <= 0)
            {
                return false;
            }
            if (Energy < amount)
            {
                return false;
            }
            Energy -= amount;
            return true;
        }
        public bool LoseMaxHP(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            }
            if (MaxHP - amount <= 0)
            {
                return false;
            }
            MaxHP -= amount;
            if (HP > MaxHP)
            {
                HP = MaxHP;
            }
            return true;
        }
    }
}