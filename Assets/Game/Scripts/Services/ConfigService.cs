using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConfigService : MonoBehaviour
{
    public static ConfigService Instance { get; private set; }

    private readonly Dictionary<string, string> _values = new();
    private bool _loaded;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;
        var go = new GameObject("[ConfigService]");
        go.AddComponent<ConfigService>();
        DontDestroyOnLoad(go);
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator EnsureLoaded()
    {
        if (_loaded) yield break;
        using var req = UnityWebRequest.Get("config.json");
        req.timeout = 3;
        yield return req.SendWebRequest();
        if (req.result == UnityWebRequest.Result.Success)
        {
            var text = req.downloadHandler.text.Trim('{', '}', ' ', '\n');
            foreach (var pair in text.Split(','))
            {
                var kv = pair.Split(':');
                if (kv.Length == 2)
                    _values[kv[0].Trim('"', ' ')] = kv[1].Trim('"', ' ');
            }
        }
        _loaded = true;
    }

    public string Get(string key, string fallback = "") =>
        _values.TryGetValue(key, out var v) ? v : fallback;
}
