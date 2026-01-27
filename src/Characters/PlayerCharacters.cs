namespace CardGame
{
    public class Player : Character
    {
        public int Gold { get; set; }
        //Player-specific logic here :)
        public Player(int maxHP)
        {
            MaxHP = maxHP;
            HP = maxHP;
        }
    }
}

