using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Brick", menuName = "ScriptableObjects/BrickScriptableObject", order = 2)]
public class ScriptableBrick : ScriptableObject
{
    [SerializeField] GameObject prefabObject = null;
    [SerializeField] private Material _material = null;
    [SerializeField] private float _score = 50.0f;
    [SerializeField] private GameObject _prefabFX = null;
    [SerializeField] private BrickEffect _brickEffect = BrickEffect.Basic;
    [SerializeField] private float _initScaleDuration = 0.1f;

    public Material Material { get => _material;}
    public GameObject PrefabVisualEffect { get => _prefabFX; }
    public GameObject PrefabObject { get => prefabObject;}
    public BrickEffect BrickEffect { get => _brickEffect; set => _brickEffect = value; }
    public float Score { get => _score; set => _score = value; }
    public float InitScaleDuration { get => _initScaleDuration; set => _initScaleDuration = value; }
}
