// Credit to Brackeys from YouTube for the idea for this script.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Level Load by Kiera Bacon
/// Version 1.0 created  October 6th, 2021
/// 
/// Makes for smooth loading of levels.
/// </summary>
public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    private LevelMetaData _defaultLevel;
    private static LevelMetaData defaultLevel;
    [SerializeField]
    private TextMeshProUGUI loadingText;
    [SerializeField]
    private Image loadingImage;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float transitionTime;
    private bool isFadingOut = false;
    private bool isLoading = false;
    public static LevelMetaData previousLevel { get; private set; }
    public static LevelMetaData currentLevel { get; private set; }
    public static LevelMetaData nextLevel { get; private set; }

    private Coroutine fadeCoroutine = null;
    private Coroutine unloadCoroutine = null;
    private Coroutine loadCoroutine = null;
    private LinkedList<LevelMetaData> levelsToLoad = new LinkedList<LevelMetaData>();
    private LinkedList<LevelMetaData> levelsToUnLoad = new LinkedList<LevelMetaData>();

    private static LevelLoader instance;
    public static LevelLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load<LevelLoader>("LevelLoader"));
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    private static void Initialize()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<LevelLoader>("LevelLoader"));
            DontDestroyOnLoad(instance);
            Debug.Log("LevelLoader loaded at runtime: " + (instance != null).ToString());
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("dest!");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        defaultLevel = _defaultLevel;
    }

    private void Update()
    {
        if (levelsToLoad.Count > 0 && loadCoroutine == null && unloadCoroutine == null)
        {
            _LoadLevel(levelsToUnLoad.First.Value, levelsToLoad.First.Value);
            levelsToUnLoad.RemoveFirst();
            levelsToLoad.RemoveFirst();
        }
    }


    /// <summary>
    /// This function is the interface through which other scripts can interact with the LevelLoader.
    /// </summary>
    /// <param name="level"> is the new level corresponding to the scene you want to load. If there is no LevelMetaData with the same name as the scene you want to load, you need to create one first.</param>
    public static void LoadLevel(LevelMetaData level)
    {
        Initialize();
        if (level == null)
            level = defaultLevel;
        
        instance._LoadLevel(currentLevel, level);
    }

    private void _LoadLevel(LevelMetaData unload, LevelMetaData load)
    {
        if (unloadCoroutine != null || loadCoroutine != null)
        {
            levelsToLoad.AddLast(load);
            if (levelsToLoad.Last.Previous != null)
                levelsToUnLoad.AddLast(levelsToLoad.Last.Previous);
            else
                levelsToUnLoad.AddLast(nextLevel);
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOut());
        unloadCoroutine = StartCoroutine(WaitToStartUnloading(unload));
        loadCoroutine = StartCoroutine(UpdateLoadingProgress(SceneManager.LoadSceneAsync(load.name, LoadSceneMode.Additive), load));
    }

    private IEnumerator FadeOut()
    {
        if (!isFadingOut)
        {
            isFadingOut = true;
            animator.speed = 1 / transitionTime;
            animator.SetTrigger("FadeOut");

            yield return new WaitForSeconds(transitionTime);

            isFadingOut = false;
        }
        else
            yield return null;

        fadeCoroutine = null;
    }

    private IEnumerator WaitToStartUnloading(LevelMetaData levelMetaData)
    {
        Scene unload;
        if (levelMetaData == null)
            unload = SceneManager.GetActiveScene();
        else
            unload = SceneManager.GetSceneByName(levelMetaData.name);

        while (isFadingOut || isLoading)
            yield return null;

        SceneManager.UnloadSceneAsync(unload);
        unloadCoroutine = null;
    }

    private IEnumerator UpdateLoadingProgress(AsyncOperation loadingOperation, LevelMetaData levelMetaData)
    {
        isLoading = true;
        loadingOperation.allowSceneActivation = false;
        LevelMetaData nextPreviousLevel = currentLevel;
        nextLevel = levelMetaData;

        while (loadingOperation.progress <= 0.9f)
        {
            loadingText.text = levelMetaData.loadingText+ " " + (loadingOperation.progress / 0.9f) * 100 + "%.";

            if (!isFadingOut)
                loadingOperation.allowSceneActivation = true;
            
            yield return null;
        }

        while (isFadingOut || loadingOperation.progress < 1.0f)
            yield return null;

        previousLevel = nextPreviousLevel;
        currentLevel = levelMetaData;
        animator.speed = 1 / transitionTime;
        animator.SetTrigger("FadeIn");
        nextLevel = null;
        isLoading = false;
        loadCoroutine = null;
    }
}
