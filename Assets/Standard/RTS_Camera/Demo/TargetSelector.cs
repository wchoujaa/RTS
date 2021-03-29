using UnityEngine;
using System.Collections;
using RTS_Cam;
using Assets.RTS.Scripts.Managers;

[RequireComponent(typeof(RTS_Camera))]
public class TargetSelector : MonoBehaviour
{
    private RTS_Camera cam;
    private new Camera camera;
    public string targetsTag;
    private InputManager inputManager;
    private void Start()
    {
        cam = gameObject.GetComponent<RTS_Camera>();
        camera = gameObject.GetComponent<Camera>();
        inputManager = FindObjectOfType<InputManager>();
    }

    private void Update()
    {
        if(inputManager.DoubleClick())
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                 
                if (hit.transform.CompareTag(targetsTag))
                    cam.transform.parent = hit.transform;
                    //cam.SetTarget(hit.transform);
                else
                    cam.transform.parent = null;

                //cam.ResetTarget();
            }
        }
    }
 
}
