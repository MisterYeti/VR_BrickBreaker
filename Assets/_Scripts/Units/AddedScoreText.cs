using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddedScoreText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtScore;
    [SerializeField] float _animDuration;
    [SerializeField] float _ySpacing;

    public void PlayAnimation(string strScore, Color color)
    {
        _txtScore.text = "+ " + strScore;
        _txtScore.color = color;
        StartCoroutine(AnimationCoroutine());
    }

    private IEnumerator AnimationCoroutine()
    {
        float currentTime = 0.0f;
        float posX = transform.localPosition.x;
        float initPosY = transform.localPosition.y;
        float endPostY = transform.localPosition.y + _ySpacing;

        while (currentTime < _animDuration)
        {
            currentTime += Time.deltaTime;
            transform.localPosition = new Vector3(posX, Mathf.Lerp(initPosY, endPostY, currentTime / _animDuration));
            yield return null;
        }

        Destroy(gameObject);
    }

}
