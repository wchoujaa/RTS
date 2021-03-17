using Assets.RTS.Scripts.Combat;
using Assets.RTS.Scripts.navigation;
using Assets.RTS.Scripts.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.RTS.Scripts.Controllers
{
	public class UnitController : MonoBehaviour
	{

		public Vector3 destination = Vector3.negativeInfinity;

		protected Animator animator;
		protected PathFindingBehaviour pathfinder;
		protected FlockingBehaviour flocking;
		protected CombatBehaviour combat;
		protected Vector3 previousDestination = Vector3.negativeInfinity;
		private bool isSelected = false;
		protected GroupManager groupManager;


		public UnitStats unitStats;
		public bool isGroupLeader = false;
		public Group group;
		public Renderer colorRenderer;
		private Color baseColor;
		public UnitType unitType;
		private GameObject targetObj;
		private float radius;

		virtual protected void Start()
		{
			//selectUI = transform.Find("Highlight").gameObject;
			pathfinder = GetComponent<PathFindingBehaviour>();
			flocking = GetComponent<FlockingBehaviour>();
			combat = GetComponent<CombatBehaviour>();
			animator = GetComponentInChildren<Animator>();
			baseColor = colorRenderer.material.color;
			groupManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GroupManager>();
			unitType = unitStats.unitType;
			targetObj = Instantiate(pathfinder.waypointPrefab, Vector3.zero, Quaternion.identity, null);
			targetObj.GetComponent<Renderer>().material.color = Color.green;
			targetObj.SetActive(false);
			radius = unitStats.radius;
		}

		virtual protected void Update()
		{
			//animator.SetFloat("speed", navMeshBehaviour.agent.velocity.magnitude); 
		}

		private void FixedUpdate()
		{
			if (isGroupLeader)
			{
				bool active = !pathfinder.TargetReached();
				targetObj.SetActive(active);
				targetObj.transform.position = pathfinder.Target;
			}
		}



		virtual public void SetSelected(bool isSelected)
		{
			this.IsSelected = isSelected;
			//selectUI.SetActive(isSelected);

			Color color = colorRenderer.material.GetColor("_OutlineColor");
			color.a = (isSelected) ? 1f : 0f;
			colorRenderer.material.SetColor("_OutlineColor", color);
			//Debug.Log(colorRenderer.material.GetColor("_OutlineColor"));
		}

		public void UpdateTarget(Transform enemy)
		{
			combat.SetNewTarget(enemy);
			pathfinder.SetDestination(enemy.position);

		}



		//public void AddWaypoint(Vector3 dest, Vector3 position)
		//{

		//	if (group == null)
		//	{
		//		MoveUnit(dest, position);
		//	}

		//	pathfinder.AddWaypoint(position);

		//	if (pathfinder.TargetReached())
		//	{
		//		pathfinder.SetDestination(position);
		//	}
		//}



		public void MoveUnit(Vector3 dest, Vector3 position, bool isWaypoint=false)
		{
			if (!isWaypoint)
				pathfinder.ClearWaypoints();
			SetGroup(dest);
			pathfinder.AddWaypoint(position);
			previousDestination = dest;
		}


		private void SetGroup(Vector3 dest)
		{
			Group previousGroup = groupManager.removeFromGroup(previousDestination, gameObject);

			if (previousGroup != null)
			{
				group = groupManager.addToGroup(dest, gameObject);
				group.leaderRadius = previousGroup.leaderRadius;
				group.separationValue = previousGroup.separationValue;

			}
			else
			{
				group = groupManager.addToGroup(dest, gameObject);
				group.leaderRadius = flocking.flockingStats.leaderRadius;
				group.separationValue = flocking.flockingStats.separation;
			}
		}


		public bool IsGroupLeader
		{
			get => isGroupLeader;
			set
			{
				if (value == true)
				{
					colorRenderer.material.color = Color.red;

				}
				else
				{
					colorRenderer.material.color = baseColor;

				}

				isGroupLeader = value;
			}
		}

		public float Radius { get => radius; set => radius = value; }
		public bool IsSelected { get => isSelected; set => isSelected = value; }
	}
}