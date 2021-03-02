using Assets.RTS.Scripts.Combat;
using Assets.RTS.Scripts.navigation;
using Assets.RTS.Scripts.ScriptableObjects;
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
		protected NavMeshBehaviour navMeshBehaviour;
		protected FlockingBehaviour flockingBehaviour;
		protected CombatBehaviour combatBehaviour;
		protected Vector3 previousDestination = Vector3.negativeInfinity;
		protected bool isSelected = false;
		protected GroupManager groupManager;


		public GameObject selectUI;
		public UnitStats unitStats;
		public bool isGroupLeader = false;
		public Group group;
		public Renderer colorRenderer;
		private Color baseColor;


		virtual protected void Start()
		{
			selectUI = transform.Find("Highlight").gameObject;
			navMeshBehaviour = GetComponent<NavMeshBehaviour>();
			flockingBehaviour = GetComponent<FlockingBehaviour>();
			combatBehaviour = GetComponent<CombatBehaviour>();
			animator = GetComponentInChildren<Animator>();
			baseColor = colorRenderer.material.color;
			groupManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GroupManager>();

		}

		virtual protected void Update()
		{
			//animator.SetFloat("speed", navMeshBehaviour.agent.velocity.magnitude); 
		}



		virtual public void SetSelected(bool isSelected)
		{
			this.isSelected = isSelected;
			//selectUI.SetActive(isSelected);

			Color color = colorRenderer.material.GetColor("_OutlineColor");
			color.a = (isSelected) ? 1f: 0f;
			colorRenderer.material.SetColor("_OutlineColor", color);
			//Debug.Log(colorRenderer.material.GetColor("_OutlineColor"));
		}

		public void UpdateTarget(Transform enemy)
		{
			combatBehaviour.SetNewTarget(enemy);
			navMeshBehaviour.SetDestination(enemy.position);

		}



		public void AddWaypoint(Vector3 point)
		{

			if (group == null)
			{
				MoveUnit(point);
			}
			if (IsGroupLeader)
			{
				navMeshBehaviour.AddWaypoint(point);
			}
			if (navMeshBehaviour.TargetReached())
			{
				navMeshBehaviour.SetDestination(point);
			}
		}



		public void MoveUnit(Vector3 dest)
		{

			navMeshBehaviour.ClearWaypoints();
			SetGroup(dest);
			navMeshBehaviour.SetDestination(dest);
			navMeshBehaviour.AddWaypoint(dest);
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
				group.leaderRadius = flockingBehaviour.flockingStats.leaderRadius;
				group.separationValue = flockingBehaviour.flockingStats.separation;
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
	}
}