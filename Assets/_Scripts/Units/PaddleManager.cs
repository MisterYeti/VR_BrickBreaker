using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PaddleManager : Singleton<PaddleManager>
{
    private Ball _ball;
    [SerializeField] private float x_maxPosition = 0.68f;
    [SerializeField] private float y_maxPosition = 0.2f;
    [SerializeField] private GameObject _lineIndicator;
    [SerializeField] private float _lineIndicatorMultiplicator = 0.5f;
    private float _distanceToScreen;
    private Vector3 _posMove;
    private PaddleState _state = PaddleState.Idle;
    private Vector3 throwingVector;
    private bool xrInputDown = false;
    private bool xrInputUp = false;

    [SerializeField] XRCustomGrabInteractable _customGrabInteractable;
    UnityEvent OnGrab, OnDrop; 
    public Ball Ball { get => _ball; set => _ball = value; }
    public float X_maxPosition { get => x_maxPosition; }

    private void Start()
    {
        if (_customGrabInteractable != null)
        {
            _customGrabInteractable.OnGrab += () => XRInputDown();
            _customGrabInteractable.OnDrop += () => XRInputUp();
        }
    }

    public void ChangeState(PaddleState newState)
    {
        _state = newState;
        switch (_state)
        {
            case PaddleState.Idle:
                Time.timeScale = 1.0f;
                break;
            case PaddleState.Waiting:
                Time.timeScale = 1.0f;
                ShowingIndicator(true);
                break;
            case PaddleState.ThrowBall:
                ShowingIndicator(false);
                _ball.ThrowBall(throwingVector);
                _ball.ChangeState(BallState.Moving);
                ChangeState(PaddleState.Active);
                break;
            case PaddleState.Active:
                Time.timeScale = 1.0f;
                break;
            case PaddleState.SoftPause:
                Time.timeScale = 0.25f;
                break;
            case PaddleState.Stop:
                Time.timeScale = 0.0f;
                break;
            default:
                break;
        }

        Debug.Log($"New Paddle state: {newState}");
    }


    void LateUpdate()
    {
        switch (_state)
        {
            case PaddleState.Stop:
                break;
            case PaddleState.Idle:
                Moving();
                break;
            case PaddleState.Waiting:
                throwingVector = new Vector3(transform.position.x * 3f, 0.5f, 0).normalized * _lineIndicatorMultiplicator - transform.position;
                Moving();
                WaitingRelease();
                break;
            case PaddleState.Active:
                Moving();
                WaitingRelease();
                break;
            case PaddleState.SoftPause:
                Moving();
                WaitingUnrelease();
                break;
            default:
                break;
        }
        ResetXRInputs();

    }

    private void ShowingIndicator(bool on)
    {
        _lineIndicator.SetActive(on);
    }

    private void Moving()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            _distanceToScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

            _posMove = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                        Input.mousePosition.y, _distanceToScreen));
            _posMove.x = Mathf.Clamp(_posMove.x, -x_maxPosition, x_maxPosition);

            if (isInScreen(_posMove))
            {
                transform.position = new Vector3(_posMove.x, transform.position.y, 0);
            }

        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                //float distanceToScreenAndroid = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
                //// get the touch position from the screen touch to world point
                //Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, distanceToScreenAndroid));
                //// lerp and set the position of the current object to that of the touch, but smoothly over time.
                //transform.position = Vector3.Lerp(transform.position, new Vector3(touchedPos.x, 0), Time.deltaTime * 10);

                _distanceToScreen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
                _posMove = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x,
                                                        touch.position.y, _distanceToScreen));
                                                        _posMove.x = Mathf.Clamp(_posMove.x, -x_maxPosition, x_maxPosition);

                if (isInScreen(_posMove))
                {
                    transform.position = new Vector3(_posMove.x, transform.position.y, 0);
                }

            }
        }

#endif

    }

    private void WaitingRelease()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            switch (_state)
            {
                case PaddleState.Waiting:
                    ChangeState(PaddleState.ThrowBall);
                    break;
                case PaddleState.Active:
                    ChangeState(PaddleState.SoftPause);                    
                    break;
                default:
                    break;
            }          
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
      if (Input.touchCount >= 1)
      {
         Touch t = Input.GetTouch(0);
         if (t.phase == TouchPhase.Ended)
         {
            switch (_state)
            {
                case PaddleState.Waiting:
                    ChangeState(PaddleState.ThrowBall);
                    break;
                case PaddleState.Active:
                    ChangeState(PaddleState.SoftPause);                    
                    break;
                default:
                    break;
            }
        }
         
      }
#endif
        if (xrInputUp)
        {
            switch (_state)
            {
                case PaddleState.Waiting:
                    ChangeState(PaddleState.ThrowBall);
                    break;
                case PaddleState.Active:
                    ChangeState(PaddleState.SoftPause);
                    break;
                default:
                    break;
            }
        }
    }

    private void WaitingUnrelease()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && isInScreen(_posMove))
        {
            ChangeState(PaddleState.Active);
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
      if (Input.touchCount > 0 && isInScreen(_posMove))
      {
        ChangeState(PaddleState.Active);       
      }
#endif
        if (xrInputDown)
        {
            ChangeState(PaddleState.Active);
        }
    }

    public bool isInScreen(Vector2 inputPosition)
    {
        return ((inputPosition.x <= x_maxPosition && inputPosition.x >= -x_maxPosition) && inputPosition.y < y_maxPosition) ? true : false;
    }

    private void XRInputDown()
    {
        xrInputDown = true;
    }
    private void XRInputUp()
    {
        xrInputUp = true;
    }
    private void ResetXRInputs()
    {
        xrInputDown = false;
        xrInputUp = false;
    }

}



public enum PaddleState
{
     Idle,
     Waiting,
     ThrowBall,
     Active,
     SoftPause,
     Stop
}
