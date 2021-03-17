using Assets.RTS.Scripts.Combat;
using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.ScriptableObjects;
using GNB;
using System.Collections;
using System.Collections.Generic;
using Turrets;

using UnityEngine;
public class TurretShooting : MonoBehaviour
{


	public CombatStats combatStat;
	public TurretRotation turretRotation;
	public List<Gun> guns;
	public float velocity;
	public bool isFiring = false;
	[Range(1, 1000)]
	public float Rate;
	private UnitController unitController;
	private CombatBehaviour combatBehaviour;


	private void Start()
	{
		combatBehaviour = GetComponent<CombatBehaviour>();
		unitController = GetComponentInParent<UnitController>();
	}

	private void Update()
	{
		if (combatBehaviour.targetAcquired)
		{

			turretRotation.SetIdle(false);

			var currentTarget = combatBehaviour.target;
			var distance = (transform.position - currentTarget.position).magnitude;

			if (distance <= combatStat.attackRange)
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

		foreach (Gun gun in guns)
		{
			if (gun.ReadyToFire)
			{
				gun.Fire(unitController.transform.forward * velocity);
				break;
			}
		}

		yield return new WaitForSeconds(1 / Rate);

		isFiring = false;
		yield return null;
	}
}
