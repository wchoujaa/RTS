using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{

	[HideInInspector]
	public NavmeshPathfinding navmeshPathfinding;
	[HideInInspector]
	FlockingBehaviour flocking;
	public Transform target;
	private float attackTimer;

	public UnitStats unitStats;
	public bool targetAcquired;

	private GameObject selectUI;
	private bool isGroupLeader = false;
	private GroupManager groupManager;

 	public enum State
	{
		IDLE,
		MOVING,
		ATTACKING
	}

	public State state;

	private void Start()
	{
		selectUI = transform.Find("Highlight").gameObject;
		navmeshPathfinding = GetComponent<NavmeshPathfinding>();
		flocking = GetComponent<FlockingBehaviour>();
		groupManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GroupManager>();
		attackTimer = unitStats.attackSpeed;
		state = State.IDLE;
		var renderer = GetComponent<Renderer>();

		renderer.material.color = unitStats.color;

	}

	private void Update()
	{
		attackTimer += Time.deltaTime;

		if (target != null)
		{
			var distance = (transform.position - target.position).magnitude;

			if (distance <= unitStats.attackRange)
			{
				Attack();
			}
		}
	}

	public void CancelOrder()
	{
		targetAcquired = false;
		target = null;
	}

	public void StopShooting()
	{
		targetAcquired = false;
		state = State.IDLE;
	}

	void Rotate(Transform target)
	{
		Vector3 targetDir = target.position - transform.position;

		// The step size is equal to speed times frame time.
		float step = unitStats.rotationSpeed * Time.deltaTime;

		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
		Debug.DrawRay(transform.position, newDir, Color.red);

		// Move our position a step closer to the target.
		transform.rotation = Quaternion.LookRotation(newDir);
	}

	public void MoveUnit(Vector3 dest)
	{
		Vector3 previousDestination = navmeshPathfinding.destination;
		state = State.MOVING;
		navmeshPathfinding.SetDestination(dest);

		if (previousDestination != Vector3.negativeInfinity)
		{
			groupManager.removeFromGroup(previousDestination, this.gameObject);
		} 
		Group group = groupManager.addToGroup(dest, this.gameObject);
		flocking.shouldStop = false; 
		flocking.group = group;
	}



	public void SetSelected(bool isSelected)
	{
		selectUI.SetActive(isSelected);
	}

	public void SetNewTarget(Transform enemy)
	{

		target = enemy;
		Vector3 position = target.position;
		Vector3 aimTarget = new Vector3();
		aimTarget.Set(position.x, position.y, position.z);
		targetAcquired = true;

		navmeshPathfinding.SetDestination(position);

	}

	public void Attack()
	{
		state = State.ATTACKING;
		if (attackTimer >= unitStats.attackSpeed)
		{
			RTSGameManager.UnitTakeDamage(this, target.GetComponent<UnitController>());
			attackTimer = 0;
		}

	}

	public void TakeDamage(UnitController enemy, float damage)
	{
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

	public Vector3 Velocity { get => navmeshPathfinding.agent.velocity; set => navmeshPathfinding.agent.velocity = value; }
	public bool IsGroupLeader
	{
		get => isGroupLeader; set
		{
			var renderer = GetComponent<Renderer>();

			if (value == true)
			{
				renderer.material.color = Color.red;

			} else
			{
				renderer.material.color = unitStats.color;

			}

			isGroupLeader = value;
		}
	}
}
