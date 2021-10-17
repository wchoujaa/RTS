using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.RTS.Scripts.Navigation.Movement
{
	public class Infantry : MonoBehaviour
	{

		public AgentStats agentStats;

		public NavMeshAgent agent;
		private UnitController UnitController;
		private void Start()
		{
			agent = GetComponent<NavMeshAgent>();

		}

		private void Update()
		{
			agent.speed = agentStats.maxSpeed;
			agent.acceleration = agentStats.maxAccel;
			agent.angularSpeed = agentStats.maxAngularSpeed;
			agent.stoppingDistance = agentStats.stoppingDistance;
			//agent.avoidancePriority = uController.IsGroupLeader ? agentStats.leaderPriority : agentStats.priority;
			transform.position = agent.nextPosition; 
		}


		public void Move(Vector3 target)
		{
			agent.SetDestination(target); 
		}


		private void Rotate()
		{
			Group group = UnitController.group;

			// Determine which direction to rotate towards

			// The step size is equal to speed times frame time.
			float singleStep = agentStats.maxAngularSpeed * Time.deltaTime;

			// Rotate the forward vector towards the target direction by one step
			Vector3 newDirection = Vector3.RotateTowards(transform.forward, group.orientation, singleStep, 0.0f);

			// Draw a ray pointing at our target in
			Debug.DrawRay(transform.position, newDirection, Color.red);

			// Calculate a rotation a step closer to the target and applies rotation to this object
			transform.rotation = Quaternion.LookRotation(newDirection);

		}
	}
}
