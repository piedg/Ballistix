namespace Gameplay
{
    public interface IPlayer
    {
        public const int InitialLives = 15;
        public int Lives { get; set; }
        public void DecreaseLives(int amount);
        public void Die();
    }
}