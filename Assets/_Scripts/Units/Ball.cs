using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody rb;
    private float _speed = 0.9f;

    [SerializeField] GameObject _prefabImpactFX;
    [SerializeField] GameObject _prefabImpactBubbleFX;
    [SerializeField] float _ySpacingFromPaddle = 1.4f;
    [SerializeField] float _scaleUpDuration = 0.2f;
    [SerializeField] float _settingBallSpeedFactor = 1.0f;
    private PaddleManager _paddle;
    private BallState _ballState = BallState.Idle;
    private Vector3 _initialScaleFactor;

    private void Start()
    {
        _paddle = PaddleManager.Instance;
        ChangeState(BallState.Idle);
        rb = GetComponent<Rigidbody>();
    }

    public void SetSpeed(float speed)
    {
        _speed = speed * _settingBallSpeedFactor;
    }

    public void ThrowBall(Vector3 direction)
    {        
        rb.AddForce(direction);
    }

    public void ChangeState(BallState newState)
    {
        _ballState = newState;
        switch (_ballState)
        {
            case BallState.Idle:
                ResetToPaddle();
                break;
            case BallState.Moving:
                UnlinkToPaddle();
                break;
            default:
                break;
        }


        Debug.Log($"New Ball state: {newState}");
    }

    private void ResetToPaddle()
    {
        transform.parent = _paddle.transform;
        transform.localPosition = new Vector3(0, _ySpacingFromPaddle, 0);
    }

    private void UnlinkToPaddle()
    {
        transform.parent = null;
    }

    private void FixedUpdate()
    {
       rb.velocity = rb.velocity.normalized * _speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Waterfall")
        {
            GameObject fx = Instantiate(_prefabImpactBubbleFX);
            fx.transform.localPosition = transform.position;
            Debug.Log("Unscale");
            StartCoroutine(EndGame());
            
        }
        else
        {
            GameObject fx = Instantiate(_prefabImpactFX);
            fx.transform.localPosition = transform.position;
        }
    }

    private IEnumerator EndGame()
    {
        yield return Scale(0.2f, false);
        Destroy(gameObject);
        GameManager.Instance.ChangeState(GameState.Lose);
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

public enum BallState
{
    Idle,
    Moving
}
