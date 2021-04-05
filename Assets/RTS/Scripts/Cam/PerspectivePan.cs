using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectivePan : MonoBehaviour
{
    public Vector3 touchStart;
    public Camera cam;
    [Range(0,2)]
    public int mousebutton;
    RaycastHit hit;
    public LayerMask ground;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mousebutton))
        {
            touchStart = GetWorldPosition();
        }
        if (Input.GetMouseButton(mousebutton))
        {
            Vector3 direction = touchStart - GetWorldPosition();
            //Debug.Log(direction);

            transform.position += direction;
        }
    }
    private Vector3 GetWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position
        Vector3 target = Vector3.zero;
        if (Physics.Raycast(ray, out hit, 50000.0f, ground)) //if we hit a unit
        {
            //Debug.Log(target);
            target = hit.point;
        }
        target.y = 0f;
        return target;
    }
}