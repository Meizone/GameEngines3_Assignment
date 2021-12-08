using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFunctionality : MonoBehaviour
{
    public void UnloadScenes(LevelMetaData scene)
    {
        LevelLoader.LoadLevel(scene);
    }
}
