using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAITargetMouse : MonoBehaviour
{

	[SerializeField] private Transform targetTransform;

	private bool isFollowing = false;
	RaycastHit hit;
	public LayerMask ground;
	private void Update()
	{
		if (isFollowing)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //raycast from previous mouse pointer position

			if (Physics.Raycast(ray, out hit, 50000.0f, ground)) //if we hit a unit
			{
				targetTransform.position = hit.point;
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			isFollowing = !isFollowing;
		}
	}

}
