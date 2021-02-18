using GNB;
using System.Collections;
using System.Collections.Generic;
using Turrets;
 
using UnityEngine;
public class TurretShooting : MonoBehaviour
{

 
	public UnitStats unitStats;
	public TurretRotation turretRotation;
	public List<Gun> guns;
	public float velocity;
	public bool isFiring = false;
	[Range(1, 1000)]
	public float Rate;
	private PlayerUnitController unitController;
	private void Start()
	{
		 
		unitController = GetComponentInParent<PlayerUnitController>();
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
