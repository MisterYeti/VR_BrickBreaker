using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private Renderer _renderer;
    private Vector3 _initialScaleFactor;
    private float _scaleDuration;
    private float _scaleDeathDuration = 0.2f;
    private GameObject _fxPrefab = null;
    private Collider _collider = null;
    private BrickEffect _brickEffect;
    private float _score;


    public IEnumerator Setup(ScriptableBrick brickData)
    {         
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
        _initialScaleFactor = transform.localScale;
        transform.localScale = Vector3.zero;
        _renderer.material = brickData.Material;
        _fxPrefab = brickData.PrefabVisualEffect;
        _brickEffect = brickData.BrickEffect;
        _score = brickData.Score;
        _scaleDuration = brickData.InitScaleDuration;
        yield return Scale(_scaleDuration, true);
    }


    private void OnCollisionEnter(Collision collision)
    {
        switch (_brickEffect)
        {
            case BrickEffect.Basic:
                ScoreManager.Instance.AddScore(_score,_renderer.material.color);
                StartCoroutine(Destroy(_scaleDeathDuration));
                break;
            case BrickEffect.Explode:
                break;
            case BrickEffect.Teleport:
                break;
            case BrickEffect.Invincible:
                PlayFX();
                break;
            default:
                throw new System.Exception();
        }

    }

    private void PlayFX()
    {
        GameObject fx = Instantiate(_fxPrefab);
        fx.transform.localPosition = transform.position;
    }

    public IEnumerator Destroy(float destroyDuration, bool checkEndGame = true)
    {
        if (checkEndGame)
        {
            LevelManager.Instance.AddDestroyedBrick();
        }
        PlayFX();
        yield return 0.2f;
        _collider.enabled = false;
        yield return Scale(destroyDuration, false);
        Destroy(gameObject);
    }


    private IEnumerator Scale(float duration, bool up)
    {
        Transform transform = this.transform;
        var currentTime = 0f;

        float startValueX, startValueY, startValueZ;
        float endValueX, endValueY, endValueZ;
        if (up)
        {
            startValueX = 0.0f; endValueX = _initialScaleFactor.x;
            startValueY = 0.0f; endValueY = _initialScaleFactor.y;
            startValueZ = 0.0f; endValueZ = _initialScaleFactor.z;
        }
        else
        {
            startValueX = _initialScaleFactor.x; endValueX = 0.0f;
            startValueY = _initialScaleFactor.y; endValueY = 0.0f;
            startValueZ = _initialScaleFactor.z; endValueZ = 0.0f;
        }

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = new Vector3(
                Mathf.Lerp(startValueX, endValueX, currentTime / duration),
                Mathf.Lerp(startValueY, endValueY, currentTime / duration),
                Mathf.Lerp(startValueZ, endValueZ, currentTime / duration));
            yield return null;
        }


    }

}
