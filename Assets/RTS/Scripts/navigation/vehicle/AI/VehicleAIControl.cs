using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleController = Assets.RTS.Scripts.Navigation.GamePlusJam.VehicleController;
namespace Assets.RTS.Scripts.Navigation
{
	public class VehicleAIControl : MonoBehaviour
	{

 
		private VehicleController carController;
		private Vector3 targetPosition;
		private PathFindingBehaviour pathfinder;
		public float reachedTargetDistance = 15f;
		private float forwardAmount;
		private float turnAmount;
		[SerializeField]
		private float reverseDistance = 25f;
		[SerializeField]
		private float stoppingDistance = 30f;
		[SerializeField]
		private float stoppingSpeed = 40f;

		private bool isMoving = false;
		[SerializeField]

		private float m_SteerSensitivity = 0.05f;

		private void Awake()
		{
			carController = GetComponent<VehicleController>();
			pathfinder = GetComponent<PathFindingBehaviour>();
		}

		private void Update()
		{

			if (pathfinder.Target == null)
			{
 				forwardAmount = 0f;
				turnAmount = 0f;
			} else
			{
				SetTargetPosition(pathfinder.Target.transform.position);

				float distanceToTarget = Vector3.Distance(transform.position, targetPosition);


				if (distanceToTarget > reachedTargetDistance)
				{
					// Still too far, keep going
					Vector3 dirToMovePosition = (targetPosition - transform.position).normalized;
					float dot = Vector3.Dot(transform.forward, dirToMovePosition);

					if (dot > 0)
					{
						// Target in front
						forwardAmount = 1f;


						if (distanceToTarget < stoppingDistance && carController.Speed.magnitude > stoppingSpeed)
						{
							// Within stopping distance and moving forward too fast
							forwardAmount = -1f;
						}
					}
					else
					{
						// Target behind
						if (distanceToTarget > reverseDistance)
						{
							// Too far to reverse
							forwardAmount = 1f;
						}
						else
						{
							forwardAmount = -1f;
						}
					}

					var forward = (forwardAmount > 0) ? transform.forward : transform.forward * -1f;

					float angleToDir = Vector3.SignedAngle(forward, dirToMovePosition, Vector3.up);
					turnAmount = Mathf.Clamp(angleToDir * m_SteerSensitivity, -1, 1);
 

 
				}
				else
				{
					// Reached target
					if (carController.Speed.magnitude > 15f)
					{
						forwardAmount = -1f;
					}
					else
					{
						forwardAmount = 0f;
					}
					turnAmount = 0f;
				}
			}


			

			carController.Move(forwardAmount, turnAmount, 0, 0);
		}

		public void SetTargetPosition(Vector3 targetPosition)
		{
			this.targetPosition = targetPosition;
		}

	}
}