using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{

	[HideInInspector]
	public NavmeshPathfinding navmeshPathfinding;
	[HideInInspector]
	FlockingBehaviour flockingBehaviour;
	public Transform target;
	private float attackTimer;

	public UnitStats unitStats;
	public bool targetAcquired;

	private GameObject selectUI;
	public bool isGroupLeader = false;
	private GroupManager groupManager;
	public Group group;
	public bool targetReached = false;
	private Vector3 previousDestination = Vector3.negativeInfinity;
	public enum State
	{
		IDLE,
		MOVING,
		ATTACKING
	}

	public State state;

	void Start()
	{
		selectUI = transform.Find("Highlight").gameObject;
		navmeshPathfinding = GetComponent<NavmeshPathfinding>();
		flockingBehaviour = GetComponent<FlockingBehaviour>();

		groupManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GroupManager>();
		attackTimer = unitStats.attackSpeed;
		state = State.IDLE;
		var renderer = GetComponent<Renderer>();

		renderer.material.color = unitStats.color;

	}

	virtual public void Update()
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



		transform.position = navmeshPathfinding.agent.nextPosition;


	}
	private void OnDrawGizmos()
	{

		if (isGroupLeader)
		{
			Gizmos.DrawSphere(transform.position, flockingBehaviour.flockingAsset.leaderRadius);
		}
	}
	private void UpdatePosition()
	{
		//Vector3 desiredVelocity = navmeshPathfinding.agent.velocity + flockingBehaviour.desiredDirection;
		//desiredVelocity = Vector3.Lerp(navmeshPathfinding.agent.velocity, desiredVelocity, Time.deltaTime * flockingBehaviour.flockingAsset.lerpSpeed);
		//desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, unitStats.maxSpeed);

		if (group != null)
		{
			if (isGroupLeader)
			{
				if (TargetReached())
					group.TargetReached = true;
			}
			else
			{
				if (IsNearLeader() && group.TargetReached)
				{
					Vector3 destination = transform.position;
					destination = Vector3.Lerp(destination, destination + flockingBehaviour.desiredDirection, Time.deltaTime * flockingBehaviour.flockingAsset.lerpSpeed);
					navmeshPathfinding.SetDestination(destination);
				}
				else if (IsNearLeader())
				{
					Vector3 destination = group.leader.transform.position;
					destination = Vector3.Lerp(destination, destination + flockingBehaviour.desiredDirection, Time.deltaTime * flockingBehaviour.flockingAsset.lerpSpeed);
					navmeshPathfinding.SetDestination(destination);
				}
				else
				{

					Vector3 destination = group.target;
					destination = Vector3.Lerp(destination, destination + flockingBehaviour.desiredDirection, Time.deltaTime * flockingBehaviour.flockingAsset.lerpSpeed);
					navmeshPathfinding.SetDestination(destination);

				}
			}

			targetReached = group.TargetReached;
		}

	}


	public void MoveUnit(Vector3 dest)
	{


		if (previousDestination != Vector3.negativeInfinity)
		{
			
			Group g = groupManager.removeFromGroup(previousDestination, this.gameObject);
 
		}
		group = groupManager.addToGroup(dest, this.gameObject);

		state = State.MOVING;
		navmeshPathfinding.SetDestination(dest);
		flockingBehaviour.targetReached = false;
		flockingBehaviour.group = group;
		navmeshPathfinding.group = group;
		navmeshPathfinding.agent.avoidancePriority = (isGroupLeader) ? flockingBehaviour.flockingAsset.leaderPriority : flockingBehaviour.flockingAsset.priority;
		previousDestination = dest;

	}


	private void FixedUpdate()
	{

		UpdatePosition();

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

			}
			else
			{
				renderer.material.color = unitStats.color;

			}

			isGroupLeader = value;
		}
	}

	public bool TargetReached()
	{
		return (group.target - transform.position).magnitude < unitStats.stoppingDistance;
	}

	public bool IsNearLeader()
	{
		return (group.leader.transform.position - transform.position).magnitude < flockingBehaviour.flockingAsset.leaderRadius;

	}

}
