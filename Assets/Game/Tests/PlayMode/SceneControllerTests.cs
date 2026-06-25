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
        var sc = Object.FindAnyObjectByType<SceneController>();
        Assert.IsNotNull(sc);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_StartScreenVisible()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var startScreen = Object.FindAnyObjectByType<StartScreen>();
        Assert.IsNotNull(startScreen);
        Assert.IsTrue(startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_GameScreenHidden()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var gameScreen = Object.FindAnyObjectByType<GameScreen>();
        Assert.IsNotNull(gameScreen);
        Assert.IsFalse(gameScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_GameOverScreenHidden()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var gameOverScreen = Object.FindAnyObjectByType<GameOverScreen>();
        Assert.IsNotNull(gameOverScreen);
        Assert.IsFalse(gameOverScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_ShowsGameScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        var gameScreen = Object.FindAnyObjectByType<GameScreen>();
        sc.StartGame();
        yield return null;
        Assert.IsTrue(gameScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_HidesStartScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        var startScreen = Object.FindAnyObjectByType<StartScreen>();
        sc.StartGame();
        yield return null;
        Assert.IsFalse(startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToStart_ShowsStartScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToStart();
        yield return null;
        var startScreen = Object.FindAnyObjectByType<StartScreen>();
        Assert.IsTrue(startScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToGameOver_ShowsGameOverScreen()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToGameOver();
        yield return null;
        var gameOverScreen = Object.FindAnyObjectByType<GameOverScreen>();
        Assert.IsTrue(gameOverScreen.IsVisible);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_PlayfieldRendererInactive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var renderer = Object.FindAnyObjectByType<PlayfieldRenderer>(FindObjectsInactive.Include);
        Assert.IsNotNull(renderer);
        Assert.IsFalse(renderer.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_InitialState_PieceControllerInactive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var piece = Object.FindAnyObjectByType<PieceController>(FindObjectsInactive.Include);
        Assert.IsNotNull(piece);
        Assert.IsFalse(piece.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_PlayfieldRendererActive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        var renderer = Object.FindAnyObjectByType<PlayfieldRenderer>();
        Assert.IsNotNull(renderer);
        Assert.IsTrue(renderer.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_StartGame_PieceControllerActive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        var piece = Object.FindAnyObjectByType<PieceController>();
        Assert.IsNotNull(piece);
        Assert.IsTrue(piece.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToGameOver_PlayfieldRendererInactive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToGameOver();
        yield return null;
        var renderer = Object.FindAnyObjectByType<PlayfieldRenderer>(FindObjectsInactive.Include);
        Assert.IsNotNull(renderer);
        Assert.IsFalse(renderer.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToGameOver_PieceControllerInactive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToGameOver();
        yield return null;
        var piece = Object.FindAnyObjectByType<PieceController>(FindObjectsInactive.Include);
        Assert.IsNotNull(piece);
        Assert.IsFalse(piece.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToStart_PlayfieldRendererInactive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToStart();
        yield return null;
        var renderer = Object.FindAnyObjectByType<PlayfieldRenderer>(FindObjectsInactive.Include);
        Assert.IsNotNull(renderer);
        Assert.IsFalse(renderer.gameObject.activeSelf);
    }

    [UnityTest]
    public IEnumerator SceneController_GoToStart_PieceControllerInactive()
    {
        SceneManager.LoadScene("Main");
        yield return null;
        var sc = Object.FindAnyObjectByType<SceneController>();
        sc.StartGame();
        yield return null;
        sc.GoToStart();
        yield return null;
        var piece = Object.FindAnyObjectByType<PieceController>(FindObjectsInactive.Include);
        Assert.IsNotNull(piece);
        Assert.IsFalse(piece.gameObject.activeSelf);
    }
}
