using Assets.RTS.Scripts.Controllers;
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

 		private int health;

		// Start is called before the first frame update


		void Start()
		{
 
			attackTimer = combatStats.attackRate;
			health = combatStats.health;
		}

		private void Update()
		{
			attackTimer += Time.deltaTime;

			if (target != null)
			{
				var distance = (transform.position - target.position).magnitude;

				if (distance <= combatStats.attackRange)
				{
					Attack();
				}
			}
		}

		public void StopShooting()
		{
			targetAcquired = false;
		}

		public void Attack()
		{
			if (attackTimer >= combatStats.attackRate)
			{
				CombatBehaviour targetBehaviour = target.GetComponent<CombatBehaviour>();

				targetBehaviour.TakeDamage(this);
				attackTimer = 0;
			}

		}

		public void SetNewTarget(Transform enemy)
		{

			target = enemy;
			Vector3 position = target.position;
			Vector3 aimTarget = new Vector3();
			aimTarget.Set(position.x, position.y, position.z);
			targetAcquired = true;


		}

		public void TakeDamage(CombatBehaviour enemy)
		{
			health -= enemy.combatStats.attackDamage;
			StartCoroutine(Flasher(GetComponent<Renderer>().material.color));
		}

		IEnumerator Flasher(Color defaultColor)
		{
			var renderer = GetComponent<Renderer>();
			for (int i = 0; i < 2; i++)
			{
				renderer.material.color = Color.gray;
				yield return new WaitForSeconds(.05f);
				renderer.material.color = defaultColor;
				yield return new WaitForSeconds(.05f);
			}
		}

		public void Cancel()
		{
			targetAcquired = false;
			target = null;
		}
	}
}
