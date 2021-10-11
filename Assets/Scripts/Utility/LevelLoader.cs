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
    private TextMeshProUGUI loadingText;
    [SerializeField]
    private Image loadingImage;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float transitionTime;
    private bool isFadingOut = false;
    private bool isLoading = false;

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
    }

    /// <summary>
    /// This function is the interface through which other scripts can interact with the LevelLoader.
    /// </summary>
    /// <param name="level"> is the new level corresponding to the scene you want to load. If there is no LevelMetaData with the same name as the scene you want to load, you need to create one first.</param>
    public static void LoadLevel(LevelMetaData level)
    {
        Initialize();
        instance._LoadLevel(level);
    }

    private void _LoadLevel(LevelMetaData level)
    {
        StartCoroutine(FadeOut());
        StartCoroutine(WaitToStartUnloading(SceneManager.GetActiveScene()));
        StartCoroutine(UpdateLoadingProgress(SceneManager.LoadSceneAsync(level.name, LoadSceneMode.Additive), level));
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
    }

    private IEnumerator WaitToStartUnloading(Scene scene)
    {
        while (isFadingOut || isLoading)
            yield return null;

        SceneManager.UnloadSceneAsync(scene);
    }

    private IEnumerator UpdateLoadingProgress(AsyncOperation loadingOperation, LevelMetaData levelMetaData)
    {
        isLoading = true;
        loadingOperation.allowSceneActivation = false;

        while (loadingOperation.progress <= 0.9f)
        {
            loadingText.text = levelMetaData.loadingText+ " " + (loadingOperation.progress / 0.9f) * 100 + "%.";

            if (!isFadingOut)
                loadingOperation.allowSceneActivation = true;
            
            yield return null;
        }

        while (isFadingOut || loadingOperation.progress < 1.0f)
            yield return null;

        animator.speed = 1 / transitionTime;
        animator.SetTrigger("FadeIn");

        isLoading = false;
    }
}
