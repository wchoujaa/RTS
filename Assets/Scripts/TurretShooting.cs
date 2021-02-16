using GNB;
using System.Collections;
using System.Collections.Generic;
using Turrets;
using UnityEngine;
using UnityEngine.AI;

public class TurretShooting : MonoBehaviour
{

	private PlayerUnitController unitController;
	public UnitStats unitStats;
	public TurretRotation turretRotation;
	public List<Gun> guns;
	public float velocity;
	public bool isFiring = false;
	public float Rate;

	private void Start()
	{
		unitController = GetComponentInParent<PlayerUnitController>();
	}

	private void Update()
	{
		if (unitController.targetAcquired)
		{

			turretRotation.SetIdle(false);

			var currentTarget = unitController.Target;
			var distance = (transform.position - currentTarget.position).magnitude;

			if (distance <= unitStats.attackRange)
			{
				turretRotation.SetAimpoint(currentTarget.position);

				if (turretRotation.IsLineOfSight() && !isFiring)
				{
					StartCoroutine(FireBullets());
				}

			}
		}
		else
		{
			turretRotation.SetIdle(true);
		}
	}


	IEnumerator FireBullets()
	{
		isFiring = true;

		foreach(Gun gun in guns)
		{
			if (gun.ReadyToFire)
			{
				gun.Fire(unitController.transform.forward * velocity);
				break;
			}
		}

		yield return new WaitForSeconds(1/Rate);

		isFiring = false;
		yield return null;
	}
}
