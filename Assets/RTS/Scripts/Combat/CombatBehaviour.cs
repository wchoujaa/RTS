using Assets.RTS.Scripts.Controllers;
using LOS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.RTS.Scripts.Combat
{
	public class CombatBehaviour : MonoBehaviour
	{
		public CombatStats combatStats;
		public Transform target;
		private float attackTimer;
		public bool targetAcquired;
		private UnitController unitController;
		private int health;
		public LayerMask unit;
		private bool takingDamage = false;
		// Start is called before the first frame update
		private GameObject pointer;
		public string pointerTag = "Pointer";

		void Start()
		{

			attackTimer = combatStats.rate;
			health = combatStats.health;
			unitController = GetComponent<UnitController>();
			pointer = GameObject.FindGameObjectWithTag(pointerTag);


		}

		private void Update()
		{
			attackTimer += Time.deltaTime;

			if (target != null)
			{
				Attack();
			}
		}


		private void FixedUpdate()
		{
			if (unitController.IsSelected)
				SetNewTarget(pointer.transform);
			else 
				AcquireTarget();


		}

		private void AcquireTarget()
		{
			Cancel();

			Collider[] hitColliders = Physics.OverlapSphere(transform.position, combatStats.range, unit);
			var distance = Mathf.Infinity;
			foreach (var hitCollider in hitColliders)
			{
				var target = hitCollider.GetComponent<UnitController>();
				var collider = hitCollider.GetComponentInChildren<LOSCuller>();
				var newDist = (transform.position - target.transform.position).magnitude;
				if (target.team != unitController.team && newDist < distance && collider != null &&  collider.Visibile)
				{
					SetNewTarget(hitCollider.transform);
					distance = newDist;
				}
			}
		}




		public void StopShooting()
		{
			targetAcquired = false;
		}

		public void Attack()
		{
			//if (attackTimer >= combatStats.rate)
			//{
			//	CombatBehaviour targetBehaviour = target.GetComponent<CombatBehaviour>();

			//	targetBehaviour.TakeDamage(this);
			//	attackTimer = 0;
			//}

		}

		public void SetNewTarget(Transform target)
		{
			this.target = target;
			Vector3 position = target.position;
			Vector3 aimTarget = new Vector3();
			aimTarget.Set(position.x, position.y, position.z);
			targetAcquired = true;
		}

		public void TakeDamage(CombatBehaviour enemy)
		{
			health -= enemy.combatStats.damage;
			if (!takingDamage)
				StartCoroutine(Flasher(unitController.colorRenderer.material.color));
		}

		IEnumerator Flasher(Color defaultColor)
		{
			takingDamage = true;
			var renderer = unitController.colorRenderer;
			for (int i = 0; i < 2; i++)
			{
				renderer.material.color = Color.gray;
				yield return new WaitForSeconds(.05f);
				renderer.material.color = defaultColor;
				yield return new WaitForSeconds(.05f);
			}
			takingDamage = false;
		}

		public void Cancel()
		{
			targetAcquired = false;
			target = null;

		}

		public bool IsEnemy(CombatBehaviour unit)
		{
			return this.unitController.team != unit.unitController.team;
		}
	}
}
