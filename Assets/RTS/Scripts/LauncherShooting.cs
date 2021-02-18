using System;
using System.Collections;
using System.Collections.Generic;
using Turrets;
using UnityEngine;

public class LauncherShooting : MonoBehaviour {
	private PlayerUnitController unitController;
	public UnitStats unitStats;
	public AAPod aapod;
	public bool isLaunching = false;
	public TurretRotation turretRotation;

	private void Start()
	{
		unitController = GetComponent<PlayerUnitController>();
		turretRotation = GetComponentInChildren<TurretRotation>();
	}

	private void Update()
	{
		if (unitController.targetAcquired)
		{

			turretRotation.SetIdle(false);

			var currentTarget = unitController.target;
			var distance = (transform.position - currentTarget.position).magnitude;

			if (distance <= unitStats.attackRange)
			{
				turretRotation.SetAimpoint(currentTarget.position);

				if (turretRotation.IsLineOfSight() && !isLaunching)
				{
					StartCoroutine(launchMissile());
				}

			}
		}
		else
		{
			turretRotation.SetIdle(true);
		}
		 
 
	}

	IEnumerator launchMissile()
	{
		isLaunching = true; 
		Transform currentTarget = unitController.target;

 
		aapod.Launch(currentTarget);
		yield return new WaitForSeconds(2.5f);
		isLaunching = false;
	}
}
