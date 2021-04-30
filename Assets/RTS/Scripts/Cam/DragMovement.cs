using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMovement : MonoBehaviour
{
 
	public Camera mainCamera;
    public float dragSpeed = 2;
    private Vector3 dragOrigin;
    [Range(0,2)]
    public int mouseButton;
    public bool is2d = true;
    public Collider levelBounds;

    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(mouseButton)) return;

        Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        Vector3 move;
        if (is2d)
		{
            move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        }
        else
		{
            move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed); 
        }
        var nextPost = transform.position + move;

        if (levelBounds.bounds.Contains(nextPost))
            transform.position = nextPost;
    }
}


