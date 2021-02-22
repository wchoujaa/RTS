using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerUnitController : UnitController
{

	override public void Update()
	{
		base.Update();

		if (Input.GetKey(KeyCode.X))
		{
			CancelOrder();
		}
	}
}


