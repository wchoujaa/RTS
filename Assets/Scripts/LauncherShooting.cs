using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherShooting : MonoBehaviour {
	private PlayerUnitController unitController;
	public UnitStats unitStats;
	public AAPod AAlauncher;
	public bool isLaunching = false;
	private void Start()
	{
		unitController = GetComponent<PlayerUnitController>();
	}

	private void Update()
	{
		if (unitController.targetAcquired)
		{
			var currentTarget = unitController.Target;
			var distance = (transform.position - currentTarget.position).magnitude;
			// if is In range launch
			if (distance <= unitStats.attackRange)
			{
				if (!isLaunching)
				{
					StartCoroutine(launchMissile());
					
				}
			}
		}
	}

	IEnumerator launchMissile()
	{
		isLaunching = true;
		Transform AAlauncherTransform = AAlauncher.GetComponent<Transform>();
		Transform currentTarget = unitController.Target;
		//AAlauncherTransform.rotation = Quaternion.LookRotation(currentTarget.position - transform.position, Vector3.up);
		AAlauncher.Launch(unitController.target);
		yield return new WaitForSeconds(2.5f);
		isLaunching = false;
	}
}
