using Assets.RTS.Scripts.Combat;
using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using Turrets;
using UnityEngine;

public class LauncherShooting : MonoBehaviour {
	private UnitController unitController;
	private CombatBehaviour combatBehaviour;

	public AAPod aapod;
	public bool isLaunching = false;
	public TurretRotation turretRotation;

	private void Start()
	{
		unitController = GetComponent<UnitController>();
		turretRotation = GetComponentInChildren<TurretRotation>();
	}

	private void Update()
	{
		if (combatBehaviour.targetAcquired)
		{

			turretRotation.SetIdle(false);

			var currentTarget = combatBehaviour.target;
			var distance = (transform.position - currentTarget.position).magnitude;

			if (distance <= combatBehaviour.combatStats.attackRange)
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
		Transform currentTarget = combatBehaviour.target;

 
		aapod.Launch(currentTarget);
		yield return new WaitForSeconds(2.5f);
		isLaunching = false;
	}
}
