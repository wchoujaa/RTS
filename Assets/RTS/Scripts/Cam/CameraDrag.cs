using UnityEngine;

public class CameraDrag : MonoBehaviour
{
	// Original code sourced from LostConjugate & Matti Jokihaara via
	// https://www.youtube.com/watch?v=mJCbEL5J5fg

	public float dragSpeed = 15f; // Found this by trial and error - applied below in Update

	float scale = 1f;
	float normalizedScale = 250f; // Found this by trial and error - applied below in Update
	[Range(0, 2)]
	public int mousebutton;
	void Update()
	{
		Vector3 pos = transform.position;

		if (Camera.main.orthographic)
			scale = Camera.main.orthographicSize;  // Use this to capture the zoom level

		if (Input.GetMouseButtonDown(mousebutton))
		{
			pos.x -= Input.GetAxis("Mouse X") * dragSpeed * scale / normalizedScale;
			pos.z -= Input.GetAxis("Mouse Y") * dragSpeed * scale / normalizedScale;
		}

		transform.position = pos;
	}
}