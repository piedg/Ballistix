
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private AIController enemy1;
    [SerializeField] private AIController enemy2;
    [SerializeField] private AIController enemy3;

    public PlayerController Player => playerController;
    public AIController Enemy1 => enemy1;
    public AIController Enemy2 => enemy2;
    public AIController Enemy3 => enemy3;

    private bool _isGamePaused = false;
    
    public UnityEvent onGameStart;
    public UnityEvent onPauseGame;
    public UnityEvent onResumeGame;
    public UnityEvent onFinishGame;

    private void Awake()
    {
        Player.OnLivesChanged += WinCheck;
        Enemy1.OnLivesChanged += WinCheck;
        Enemy2.OnLivesChanged += WinCheck;
        Enemy3.OnLivesChanged += WinCheck;
        
        inputManager.OnPause += TogglePauseGame;
    }

    private void OnDestroy()
    {
        Player.OnLivesChanged -= WinCheck;
        Enemy1.OnLivesChanged -= WinCheck;
        Enemy2.OnLivesChanged -= WinCheck;
        Enemy3.OnLivesChanged -= WinCheck;
        
        inputManager.OnPause -= TogglePauseGame;
    }

    private void Start()
    {
        onGameStart?.Invoke();
    }

    private void WinCheck(int i)
    {
        if (playerController.Lives > 0 && enemy1.Lives <= 0 && enemy2.Lives <= 0 && enemy3.Lives <= 0)
        {
            Debug.Log("Player Wins!");
        }
        else if (playerController.Lives <= 0 && enemy1.Lives > 0 && enemy2.Lives <= 0 && enemy3.Lives <= 0)
        {
            Debug.Log("Enemy 1 Wins!");
        }
        else if (playerController.Lives <= 0 && enemy1.Lives <= 0 && enemy2.Lives > 0 && enemy3.Lives <= 0)
        {
            Debug.Log("Enemy 2 Wins!");
        }
        else if (playerController.Lives <= 0 && enemy1.Lives <= 0 && enemy2.Lives <= 0 && enemy3.Lives > 0)
        {
            Debug.Log("Enemy 3 Wins!");
        }
    }

    private void TogglePauseGame()
    {
        _isGamePaused = !_isGamePaused;

        if (!_isGamePaused)
        {
            onPauseGame?.Invoke();
        }
        else 
        {
            onResumeGame?.Invoke();
        }
    }

    public void Invoke_OnResumeGame()
    {
        onResumeGame?.Invoke();
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1;
    }
}