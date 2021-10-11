using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class LevelMetaData : ScriptableObject
{
    public int buildIndex;
    public Sprite loadingImage;
    public string loadingText = "Loading...";
}
