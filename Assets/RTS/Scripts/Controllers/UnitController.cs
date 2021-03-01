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



		virtual protected void Start()
		{
			selectUI = transform.Find("Highlight").gameObject;
			navMeshBehaviour = GetComponent<NavMeshBehaviour>();
			flockingBehaviour = GetComponent<FlockingBehaviour>();
			combatBehaviour = GetComponent<CombatBehaviour>();
			animator = GetComponentInChildren<Animator>();
			colorRenderer.material.color = unitStats.color;
			groupManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GroupManager>();

		}

		virtual protected void Update()
		{
			//animator.SetFloat("speed", navMeshBehaviour.agent.velocity.magnitude); 
		}



		virtual public void SetSelected(bool isSelected)
		{
			this.isSelected = isSelected;
			selectUI.SetActive(isSelected);
		}

		public void UpdateTarget(Transform enemy)
		{
			combatBehaviour.SetNewTarget(enemy);
			navMeshBehaviour.SetDestination(enemy.position);

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
					colorRenderer.material.color = unitStats.color;

				}

				isGroupLeader = value;
			}
		} 
	}
}