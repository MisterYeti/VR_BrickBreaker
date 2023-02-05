using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform chil in t) GameObject.Destroy(chil.gameObject);
    }
}
