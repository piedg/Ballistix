using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
    private int _score = 15;
    public int Score => _score;

    public void UpdateScore(int amount)
    {
        _score -= amount;
    }
}