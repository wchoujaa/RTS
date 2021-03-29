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
	public GameObject target;
	public float groundWaypointOffset = 0.1f;
	private NavMeshPath path;
	public GameObject Target { get => target; set => target = value; }

	// Start is called before the first frame update
	void Start()
	{
		uController = GetComponent<UnitController>();
		flockingBehaviour = GetComponent<FlockingBehaviour>();
		path = new NavMeshPath();
	}

	void Update()
	{

	}

	private void FixedUpdate()
	{


		// Debug.Log(currentWP + " " + waypoints.Count);
		

		if (TargetReached() || (IsNearLeader() && !uController.isGroupLeader))
		{
			NextWaypoint();
		}

		updateWaypointDisplay();




		if (uController.isGroupLeader && TargetReached())
		{
			GetGroup().TargetReached = true;
		}
 


		//DebugPath();


	}

	private void updateWaypointDisplay()
	{
 		 
		if (uController.isGroupLeader && target != null)
		{

			target.GetComponent<LineRenderer>().SetPosition(0, transform.position + Vector3.up * groundWaypointOffset);
		} 
	}

	private void DebugPath()
	{
		if (path.corners.Length == 1)
			Debug.DrawLine(transform.position, path.corners[0], Color.red);

		for (int i = 0; i < path.corners.Length - 1; i++)
			Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
	}


	private void NextWaypoint()
	{
		//Debug.Log(currentWP);
		if (waypoints.Count == 0 | currentWP >= waypoints.Count)
		{
			ClearWaypoints();
			return;
		}
		//Debug.Log(currentWP);
		GameObject nextWaypont = waypoints[currentWP];
		Target = nextWaypont;

		if (uController.IsGroupLeader) // update waypoint display
		{
			GameObject nextWaypointObj = waypoints[currentWP];
			nextWaypointObj.GetComponent<LineRenderer>().SetPosition(1, nextWaypointObj.transform.position);
		}

		if (currentWP > 0 && waypoints.Count > 1) // destroy previous
		{
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

		target = null;

		waypoints.Clear();

		currentWP = 0; // reset waypoint counter
	}

	public Vector3[] GetPath(Vector3 dest)
	{
		NavMeshHit myNavHit;
		Vector3[] corners = new Vector3[0];
		Vector3 source = (waypoints.Count > 0) ? waypoints[waypoints.Count - 1].transform.position : transform.position;
		if (NavMesh.SamplePosition(dest, out myNavHit, 100, -1))
		{
			if (NavMesh.CalculatePath(source, myNavHit.position, NavMesh.AllAreas, path))
			{
				corners = path.corners;
			}
		}
		return corners;
	}


	public void AddWaypoint(Vector3 point)
	{
		// waypoint diplay
		Vector3[] corners = GetPath(point);
		GameObject waypoint;

		for (int i = 0; i < corners.Length; i++)
		{

			Vector3 target = corners[i];
			waypoint = InstantiateWaypoint(target);

			if (waypoints.Count == 0) Target = waypoint;

			waypoints.Add(waypoint);
		}
	}

	private GameObject InstantiateWaypoint(Vector3 position)
	{
		GameObject waypoint;
		position += Vector3.up * groundWaypointOffset;
		waypoint = Instantiate(waypointPrefab, position, Quaternion.identity, null);
		waypoint.name = "waypoint";

		if (uController.isGroupLeader) // if is Group Leader, add to group waypoint list and display it
		{

			LineRenderer lineRenderer = waypoint.GetComponent<LineRenderer>();

			if (waypoints.Count > 0) // if there is one or more waypoint
			{
				Vector3 last = waypoints[waypoints.Count - 1].transform.position;
				lineRenderer.SetPosition(1, last);
			} else
			{
				lineRenderer.SetPosition(1, transform.position); 
			}

			lineRenderer.SetPosition(0, waypoint.transform.position);

		}
		return waypoint;
	}

	public bool TargetReached()
	{
		if (target == null) return true;
		return (target.transform.position - transform.position).magnitude < uController.unitStats.stoppingDistance;
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
