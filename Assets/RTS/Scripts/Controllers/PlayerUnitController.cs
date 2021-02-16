using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerUnitController : UnitController
{

	private void Update()
	{
		if (Input.GetKey(KeyCode.X))
		{
			CancelOrder();
		}
	}
}


