using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneControllerTests
{
    [UnityTest]
    public IEnumerator SceneController_HasSceneControllerComponent()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindFirstObjectByType<SceneController>();
        Assert.IsNotNull(sc);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_StartScreenVisible()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var startScreen = Object.FindFirstObjectByType<StartScreen>();
        Assert.IsNotNull(startScreen);
        Assert.IsTrue(startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_GameScreenHidden()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var gameScreen = Object.FindFirstObjectByType<GameScreen>();
        Assert.IsNotNull(gameScreen);
        Assert.IsFalse(gameScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_GameOverScreenHidden()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var gameOverScreen = Object.FindFirstObjectByType<GameOverScreen>();
        Assert.IsNotNull(gameOverScreen);
        Assert.IsFalse(gameOverScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_ShowsGameScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindFirstObjectByType<SceneController>();
        var gameScreen = Object.FindFirstObjectByType<GameScreen>();
        sc.StartGame();
        yield return null;
        Assert.IsTrue(gameScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_HidesStartScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindFirstObjectByType<SceneController>();
        var startScreen = Object.FindFirstObjectByType<StartScreen>();
        sc.StartGame();
        yield return null;
        Assert.IsFalse(startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToStart_ShowsStartScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindFirstObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToStart();
        yield return null;
        var startScreen = Object.FindFirstObjectByType<StartScreen>();
        Assert.IsTrue(startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToGameOver_ShowsGameOverScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindFirstObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToGameOver();
        yield return null;
        var gameOverScreen = Object.FindFirstObjectByType<GameOverScreen>();
        Assert.IsTrue(gameOverScreen.IsVisible);
    }
}
