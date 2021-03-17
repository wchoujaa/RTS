using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.navigation;
using Assets.RTS.Scripts.ScriptableObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PathFindingBehaviour : MonoBehaviour
{
	private FlockingBehaviour flockingBehaviour;
	private UnitController uController;
	public GameObject waypointPrefab;
	public int currentWP = 0;
	private Vector3 direction;
	public List<GameObject> waypoints = new List<GameObject>();
	public Vector3 target;

	private NavMeshPath path;
	public Vector3 Target { get => target; set => target = value; }

	// Start is called before the first frame update
	void Start()
	{
		uController = GetComponent<UnitController>();
		flockingBehaviour = GetComponent<FlockingBehaviour>();
		path = new NavMeshPath();
		target = transform.position;
	}

	void Update()
	{

	}

	private void FixedUpdate()
	{


		//Debug.Log(currentWP + " " + waypoints.Count);
		if (TargetReached() || (IsNearLeader() && !uController.isGroupLeader))
		{
			NextWaypoint();

		}


		if (uController.isGroupLeader && TargetReached())
		{
			GetGroup().TargetReached = true;
		}


		switch (uController.unitType)
		{
			case UnitType.Tank:
				//if (Vector3.Angle(agent.velocity, transform.forward) > 3f)
				//{
				//	transform.position = agent.nextPosition;
				//}
				break;
			case UnitType.Vehicle:
			case UnitType.Infantry:
			case UnitType.Jet:
			case UnitType.Heli:
				break;
			default:
				break;
		}


		for (int i = 0; i < path.corners.Length - 1; i++)
			Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
	}



	public void SetDestination(Vector3 dest)
	{
		NavMeshHit myNavHit;

		if (NavMesh.SamplePosition(dest, out myNavHit, 100, -1))
		{
			if (NavMesh.CalculatePath(transform.position, myNavHit.position, NavMesh.AllAreas, path))
			{
				Target = path.corners[0];

				if (uController.isGroupLeader)
				{
					GetGroup().target = Target;
					GetGroup().TargetReached = false;
				}
			}
		}
	}

	private void NextWaypoint()
	{
		Debug.Log(currentWP);
		if (waypoints.Count == 0 | currentWP >= waypoints.Count) return;
		GameObject nextWaypont = waypoints[currentWP];
		SetDestination(nextWaypont.transform.position);

		if (uController.IsGroupLeader && currentWP > 0 && waypoints.Count > 1) // update waypoint display
		{
			GameObject nextWaypointObj = waypoints[currentWP];
			nextWaypointObj.GetComponent<LineRenderer>().SetPosition(1, nextWaypointObj.transform.position);
			GameObject previous = waypoints[currentWP - 1];
			Destroy(previous);
		}

		currentWP++;

	}



	public void ClearWaypoints()
	{

		foreach (GameObject obj in waypoints)
		{
			Destroy(obj);
		}

		target = transform.position;

		waypoints.Clear();

		currentWP = 0; // reset waypoint counter
	}

	public void AddWaypoint(Vector3 point)
	{
		GameObject waypoint;
		// waypoint diplay
		if (uController.isGroupLeader) // if is Group Leader, add to group waypoint list and display it
		{
			waypoint = Instantiate(waypointPrefab, point, Quaternion.identity, null);
			LineRenderer lineRenderer = waypoint.GetComponent<LineRenderer>();

			if (waypoints.Count > 0) // if there is one or more waypoint
			{
				Vector3 last = waypoints[waypoints.Count - 1].transform.position;
				lineRenderer.SetPosition(1, last);
				lineRenderer.SetPosition(0, waypoint.transform.position);
			}
		}
		else
		{
			waypoint = new GameObject();
			waypoint.transform.position = point;
		}

		if (waypoints.Count == 0) Target = waypoint.transform.position;

		waypoints.Add(waypoint); 
	}

	public bool TargetReached()
	{
 		return (Target - transform.position).magnitude < uController.unitStats.stoppingDistance;
	}

	public bool IsNearLeader()
	{
		if (GetGroup() == null) return false;
		if (uController.isGroupLeader) return true;
		return !uController.IsGroupLeader && (GetGroup().leader.transform.position - transform.position).magnitude < GetGroup().leaderRadius;
	}

	private Group GetGroup()
	{
		return uController.group;
	}
}
