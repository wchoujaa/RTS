using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class PreRenderCalls : MonoBehaviour
{
    public bool UsingSRP;
    public Fog _Fog;

    void OnEnable()
    {

    }

    void OnDisable()
    {
 
    }


    void PreRenderSRP(Camera obj)
    {
        OnPreRender();
    }

    void OnPreRender()
    {
        // FOG CALL
        //_Fog.SetCookie();
    }
}
