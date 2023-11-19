using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig", order = 1)]
public class PlayerConfig : ScriptableObject
{
    [BoxGroup("Player")] public int jumpSpeedX = 10;
    [BoxGroup("Player")] public int jumpSpeedY = 17;
    [BoxGroup("Player")] public float gravity = 3;

    [BoxGroup("Triangle")] public int multipleScoreChangeBackgrounds = 20;
    [BoxGroup("Triangle")] public bool inManualAtStart = true;
    [BoxGroup("Triangle")] public float offsetAtStart = 3f;
    //[BoxGroup("Triangle")] public List<Sprite> backgroundSprites;
    [BoxGroup("Triangle")] public List<BackgroundData> backgroundKeys;
    [BoxGroup("Triangle")] public float offsetRight = -0.7f;
    [BoxGroup("Triangle")] public float offsetLeft = 0f;
    
    [BoxGroup("Triangle")] [Space] [Range(0, 15)]
    public int numberOfTrianglesStart = 3;

    [BoxGroup("Triangle")] [Range(1, 15)] public int numberOfTrianglesMax = 15;
    [BoxGroup("Triangle")] [Range(1, 10)] public int triangleCountUpScore = 3;
    
    [BoxGroup("Skins features")] public List<SkinData> skins = new List<SkinData>();
}

[Serializable]
public struct BackgroundData
{
    public string key;
    public int score;
    public Sprite sprite;
}