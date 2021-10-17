 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
 
namespace Assets.RTS.Scripts.Navigation.Movement
{
	public class CarAIControllerHybrid : MonoBehaviour
	{

		[SerializeField] private Transform targetPositionTranform;

		private CarController m_CarController;
 		private PathFindingBehaviour pathfinder;
		public float reachedTargetDistance = 15f;
		public float forwardAmount;
		public float turnAmount;
		public float brake;



		private void Awake()
		{
			m_CarController = GetComponent<CarController>();
			pathfinder = GetComponent<PathFindingBehaviour>();
		}

		private void Update()
		{
			targetPositionTranform = pathfinder.target?.transform;

			if (targetPositionTranform == null)
			{
				forwardAmount = 0f;
				turnAmount = 0f;
				brake = 1f;
				//m_CarController.Move(0, 0, -1f, 1f);

			} else
			{
				float distanceToTarget = Vector3.Distance(transform.position, targetPositionTranform.position);
				brake = 0f;

				if (distanceToTarget > reachedTargetDistance)
				{
					// Still too far, keep going
					Vector3 dirToMovePosition = (targetPositionTranform.position - transform.position).normalized;
					float dot = Vector3.Dot(transform.forward, dirToMovePosition);

					if (dot > 0)
					{
						// Target in front
						forwardAmount = 1f;

						float stoppingDistance = 30f;
						float stoppingSpeed = 40f;
						if (distanceToTarget < stoppingDistance && m_CarController.CurrentSpeed > stoppingSpeed)
						{
							// Within stopping distance and moving forward too fast
							forwardAmount = -1f;
						}
					}
					else
					{
						// Target behind
						float reverseDistance = 25f;
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

					float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);

					if (angleToDir > 0)
					{
						turnAmount = 1f;
					}
					else
					{
						turnAmount = -1f;
					}
				}
				else
				{
					// Reached target
					if (m_CarController.CurrentSpeed > 15f)
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


			m_CarController.Move(turnAmount, forwardAmount, forwardAmount, 0);

			//m_CarController.Move(-1, 1, 1, 0);

			//carDriver.SetInputs(forwardAmount, turnAmount);
		}

		public void SetTargetPosition(Transform targetPosition)
		{
			this.targetPositionTranform = targetPosition;
		}

	}

}
