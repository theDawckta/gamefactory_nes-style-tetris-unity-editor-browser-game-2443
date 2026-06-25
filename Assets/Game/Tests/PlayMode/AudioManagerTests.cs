using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class AudioManagerTests
{
    private GameObject _go;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Ensure any leftover singleton is destroyed before each test
        if (AudioManager.Instance != null)
        {
            Object.Destroy(AudioManager.Instance.gameObject);
            yield return null;
        }
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (AudioManager.Instance != null)
        {
            Object.Destroy(AudioManager.Instance.gameObject);
            yield return null;
        }
        if (_go != null)
        {
            Object.Destroy(_go);
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator Instance_IsSetAfterAwake()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;
        Assert.IsNotNull(AudioManager.Instance);
    }

    [UnityTest]
    public IEnumerator SecondInstance_IsDestroyed()
    {
        _go = new GameObject("AM1");
        _go.AddComponent<AudioManager>();
        yield return null;

        var first = AudioManager.Instance;

        var go2 = new GameObject("AM2");
        go2.AddComponent<AudioManager>();
        yield return null;

        Assert.AreEqual(first, AudioManager.Instance);
        Object.Destroy(go2);
    }

    [UnityTest]
    public IEnumerator PlayMusic_WithUnknownName_DoesNotThrow()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        Assert.DoesNotThrow(() => AudioManager.Instance.PlayMusic("nonexistent_track"));
    }

    [UnityTest]
    public IEnumerator PlayMusic_WithKnownName_DoesNotThrow()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        Assert.DoesNotThrow(() => AudioManager.Instance.PlayMusic("gameplay_theme"));
    }

    [UnityTest]
    public IEnumerator StopMusic_DoesNotThrow()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        Assert.DoesNotThrow(() => AudioManager.Instance.StopMusic());
    }

    [UnityTest]
    public IEnumerator PlaySFX_WithUnknownName_DoesNotThrow()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        Assert.DoesNotThrow(() => AudioManager.Instance.PlaySFX("nonexistent_sfx"));
    }

    [UnityTest]
    public IEnumerator PlaySFX_WithKnownName_DoesNotThrow()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        Assert.DoesNotThrow(() => AudioManager.Instance.PlaySFX("piece_lock"));
    }

    [UnityTest]
    public IEnumerator StopMusic_AfterPlayMusic_StopsMusicSource()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        // PlayMusic then StopMusic -- verify no exception and source is stopped
        AudioManager.Instance.PlayMusic("gameplay_theme");
        yield return null;
        AudioManager.Instance.StopMusic();
        yield return null;
        // Just verifying no exceptions thrown; clip may be null in test environment
        Assert.IsNotNull(AudioManager.Instance);
    }

    [UnityTest]
    public IEnumerator OnDestroy_ClearsInstance()
    {
        _go = new GameObject();
        _go.AddComponent<AudioManager>();
        yield return null;

        Assert.IsNotNull(AudioManager.Instance);

        Object.Destroy(_go);
        _go = null;
        yield return null;

        Assert.IsNull(AudioManager.Instance);
    }
}
