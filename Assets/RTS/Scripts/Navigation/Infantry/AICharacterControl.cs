using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Assets.RTS.Scripts.Navigation.Infantry
{
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class AICharacterControl : MonoBehaviour
	{
		private ThirdPersonCharacter Character;// the character we are controlling
		public Transform target;                                    // target to aim for
		private PathFindingBehaviour pathfinder;
		public float stoppingDistance = 3f;
		public Vector3 direction;
		private void Start()
		{
			// get the components on the object we need ( should not be null due to require component so no need to check )
			Character = GetComponent<ThirdPersonCharacter>();
			pathfinder = GetComponent<PathFindingBehaviour>();


		}


		private void Update()
		{
			target = pathfinder.target?.transform;


			direction = target != null ? target.position - transform.position : Vector3.zero;

			Character.Move(direction, false, false);

		}


		public void SetTarget(Transform target)
		{
			this.target = target;
		}
	}
}
