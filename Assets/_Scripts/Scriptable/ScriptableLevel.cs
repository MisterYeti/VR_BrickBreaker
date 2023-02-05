using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelScriptableObject", order = 1)]
public class ScriptableLevel : ScriptableObject
{
    [SerializeField] LevelLayout bricks = null;
    [SerializeField] LevelDifficulty levelDifficulty;

    public LevelLayout Bricks { get => bricks; }
    public LevelDifficulty LevelDifficulty { get => levelDifficulty; set => levelDifficulty = value; }
}

[Serializable]
public struct LevelDifficulty
{
    public int level;
    public float speed;
}