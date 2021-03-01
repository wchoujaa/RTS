using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.RTS.Scripts.navigation
{
	public class FlockingBehaviour : MonoBehaviour
	{

		public FlockingStats flockingStats;

		public Vector3 desiredDirection = new Vector3();

		private UnitController uController;

		// Start is called before the first frame update
		void Start()
		{

			uController = GetComponent<UnitController>();
			StartCoroutine(FlockingUpdate());
		}

		void FixedUpdate()
		{
			if (uController.group == null)
			{
				desiredDirection = Vector3.zero;
			} 
		}


		IEnumerator FlockingUpdate()
		{
			while (true)
			{
				if (uController.group != null)
				{
					desiredDirection = BoidCohesion();
					desiredDirection += BoidSeparation();
					desiredDirection *= flockingStats.multiplier * Time.deltaTime;
				}

				yield return new WaitForSeconds(flockingStats.updateRate);

			}
		}

		public Vector3 BoidCohesion()
		{

			Vector3 steering = new Vector3();
			int count = 0;

			foreach (GameObject other in uController.group.members) //iterate through the group of objects
			{

				if (other != null)
				{

					float d = (transform.position - other.transform.position).magnitude;
					if (d > 0 && d < flockingStats.neighbordist)
					{
						steering += other.transform.position; // Add position
						count++;
					}

				}
			}

			if (count > 0) //average the positions of all the objects
			{
				steering /= count;
				steering = steering - transform.position;
			}

			return steering * flockingStats.cohesion;

		}



		public Vector3 BoidSeparation()
		{


			Vector3 steering = new Vector3();
			int count = 0;

			// For every boid in the system, check if it's too close
			foreach (GameObject other in uController.group.members)
			{

				if (other != null)
				{

					float d = (transform.position - other.transform.position).magnitude;

					// If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
					if (d > 0 && d < flockingStats.desiredseparation)
					{
						// Calculate vector pointing away from neighbor
						Vector3 diff = transform.position - other.transform.position;
						diff.Normalize();
						diff /= d;        // Weight by distance
						steering += diff;
						count++;            // Keep track of how many
					}

				}
			}

			// Average -- divide by how many
			if (count > 0)
			{
				//steering.linear /= (float)count;
			}

			return steering * uController.group.separationValue;
		}


		public void SpreadUnit(float amount)
		{
			Group group = uController.group;
			group.separationValue = Mathf.Clamp(group.separationValue + amount, 2f, 100);
			group.leaderRadius = Mathf.Clamp(group.leaderRadius + amount, 10f, 110f);
		}


		private void OnDrawGizmos()
		{
			if (uController && uController.IsGroupLeader)
			{
				Gizmos.DrawSphere(transform.position, uController.group.leaderRadius);
			}
		}
	}
}