using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    private static bool isShuttingDown;
    private static bool isLoadingScene;

    static Singleton()
    {
        SceneManager.activeSceneChanged += (_, __) => { isLoadingScene = true; };
        SceneManager.sceneLoaded += (_, __) => { isLoadingScene = false; };
    }

    public static T Instance
    {
        get
        {
            if (isShuttingDown || isLoadingScene) return null; // don’t spam during teardown/load
            if (instance == null)
                instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (instance == null)
                Debug.LogError($"[Singleton] No instance of {typeof(T).Name} in active scene '{SceneManager.GetActiveScene().name}'.");
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this as T;
    }

    protected virtual void OnApplicationQuit() => isShuttingDown = true;

    protected virtual void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}