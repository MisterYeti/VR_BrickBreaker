using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRCustomGrabInteractable : XRGrabInteractable
{
    public Action OnGrab;
    public Action OnDrop;
    [SerializeField] private PaddleManager _paddle;

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        transform.localPosition = new Vector3(transform.localPosition.x, 0.2f, 0);
        if (transform.localPosition.x >= _paddle.X_maxPosition)
        {
            transform.localPosition = new Vector3(_paddle.X_maxPosition, 0.2f, 0);
        }
        if (transform.localPosition.x <= -_paddle.X_maxPosition)
        {
            transform.localPosition = new Vector3(-_paddle.X_maxPosition, 0.2f, 0);
        }
        transform.localEulerAngles = Vector3.zero;
    }

    protected override void Grab()
    {
        Debug.Log("Grab");
        OnGrab.Invoke();
        base.Grab();
    }

    protected override void Drop()
    {
        Debug.Log("Drop");
        OnDrop.Invoke();
        base.Drop();
    }
}
