using UnityEngine;
using UnityEngine.SceneManagement;

public enum eScene
{
    MainMenu,
    Game
}

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(eScene scene)
    {
        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }

    public void LoadMainMenu()
    {
        LoadScene(eScene.MainMenu);
    }

    public void LoadGame()
    {
        LoadScene(eScene.Game);
    }

    public void ExitApplication()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}