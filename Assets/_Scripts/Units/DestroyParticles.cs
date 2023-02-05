using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DestroyParticles : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 2.0f);
    }
}
