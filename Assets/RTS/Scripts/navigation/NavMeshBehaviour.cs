using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.navigation;
using Assets.RTS.Scripts.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavMeshBehaviour : MonoBehaviour
{



	public NavMeshAgent agent;
	public AgentStats agentStats;

	private FlockingBehaviour flockingBehaviour;
	private UnitController uController;

	public GameObject waypointPrefab;

	private int currentWP = 0;

	// Start is called before the first frame update
	void Start()
	{

		agent = GetComponent<NavMeshAgent>();
		uController = GetComponent<UnitController>();
		flockingBehaviour = GetComponent<FlockingBehaviour>();
		agent.updatePosition = false;
		//agent.updateRotation = false;
	}



	void Update()
	{
		transform.position = agent.nextPosition;
		if (GetGroup() != null)
		{
			if (uController.isGroupLeader)
			{
				agent.speed = agentStats.maxSpeed * flockingBehaviour.flockingStats.leaderSpeedFactor / 100f;
				if (TargetReached())
					GetGroup().TargetReached = true;
			}
			else
			{
				if (IsNearLeader() && GetGroup().TargetReached)
				{

					Vector3 destination = transform.position;
					destination = Vector3.Lerp(destination, destination + flockingBehaviour.desiredDirection, Time.deltaTime * flockingBehaviour.flockingStats.lerpSpeed);
					agent.SetDestination(destination);
				}
				else
				{
					Vector3 destination = GetGroup().leader.transform.position + flockingBehaviour.desiredDirection;
					destination = Vector3.Lerp(destination, destination + flockingBehaviour.desiredDirection, Time.deltaTime * flockingBehaviour.flockingStats.lerpSpeed);
					agent.SetDestination(destination);
				}

			}
		}
	}


	private void FixedUpdate()
	{
		if (GetGroup() != null && TargetReached())
		{
			//Debug.Log(currentWP + " " + GetGroup().waypoints.Count);
			if(currentWP >= GetGroup().waypoints.Count - 1)  
			{
				ClearWaypoints();

			} else
			{
				NextWaypoint();

			}
		}
	}


	public void SetDestination(Vector3 dest)
	{
		
		agent.speed = agentStats.maxSpeed;
		agent.acceleration = agentStats.maxAccel;
		agent.angularSpeed = agentStats.maxAngularSpeed;
		agent.stoppingDistance = agentStats.stoppingDistance;
		agent.avoidancePriority = uController.IsGroupLeader ? agentStats.leaderPriority : agentStats.priority;
		agent.SetDestination(dest);
	}


	private void Rotate()
	{
		Group group = GetGroup();

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

	private void NextWaypoint()
	{
		//Debug.Log(currentWP);
		if (uController.IsGroupLeader  )
		{
			currentWP++; 
			Vector3 next = GetGroup().waypoints[currentWP];
			GameObject nextWaypoint = GetGroup().waypointsObj[currentWP];
			nextWaypoint.GetComponent<LineRenderer>().SetPosition(1, nextWaypoint.transform.position);
			 

			GameObject previous = GetGroup().waypointsObj[currentWP - 1];
			GetGroup().target = next;
			GetGroup().TargetReached = false;
			Destroy(previous);
		}

		SetDestination(GetGroup().target);

	}



	public void ClearWaypoints()
	{
		if (GetGroup() == null || !uController.IsGroupLeader) return;

		foreach (GameObject obj in GetGroup().waypointsObj)
		{
			Destroy(obj);
		}
		GetGroup().waypointsObj.Clear();
		GetGroup().waypoints.Clear();
		currentWP = 0; // reset waypoint counter
	}

	public void AddWaypoint(Vector3 point)
	{
		GameObject obj = Instantiate(waypointPrefab, point, Quaternion.identity, null);
		LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();

		Group group = GetGroup();

		group.waypoints.Add(obj.transform.position);
		group.waypointsObj.Add(obj);

		if(group.waypoints.Count > 1)
		{
			Vector3 last = group.waypoints[group.waypoints.Count - 2];
			lineRenderer.SetPosition(1, last);
			lineRenderer.SetPosition(0, obj.transform.position); 
		} 
	}


	public bool TargetReached()
	{

		return (agent.destination - transform.position).magnitude < agentStats.stoppingDistance;
	}

	public bool IsNearLeader()
	{

		return (GetGroup().leader.transform.position - transform.position).magnitude < GetGroup().leaderRadius;

	}

	private Group GetGroup()
	{
		return uController.group;
	}
}
