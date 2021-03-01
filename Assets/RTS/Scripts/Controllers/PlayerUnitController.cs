using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerUnitController : UnitController
{
	private InputManager inputManager;
	private GameObject targetObj;

	override protected void Start()
	{
		base.Start();
		inputManager = FindObjectOfType<InputManager>();
		targetObj = Instantiate(navMeshBehaviour.waypointPrefab, Vector3.zero, Quaternion.identity, null);
		targetObj.GetComponent<Renderer>().material.color = Color.green;
		targetObj.SetActive(false);
	}

	override protected void Update()
	{
		base.Update();

		if (Input.GetKeyUp(KeyCode.X))
		{
			CancelOrder();
		}

		handleInput();
	}


	private void FixedUpdate()
	{
		if (isGroupLeader)
		{
			bool active = !navMeshBehaviour.TargetReached();
			targetObj.SetActive(active);
			targetObj.transform.position = navMeshBehaviour.agent.destination;
		}
	}

	private void handleInput()
	{


		if (isSelected && isGroupLeader)
		{
			if (inputManager.doubleE.DoubleClickLongPressedCheak())
			{
				flockingBehaviour.SpreadUnit(-flockingBehaviour.flockingStats.spreadAmount);

			}
			else if (inputManager.doubleE.SingleClickLongPressedCheck())
			{
				flockingBehaviour.SpreadUnit(flockingBehaviour.flockingStats.spreadAmount);
			}
		}
	}

	public void CancelOrder()
	{
		combatBehaviour.Cancel();

	}


	override public void SetSelected(bool isSelected)
	{
		base.SetSelected(isSelected);
		if (!isSelected && IsGroupLeader)
		{

		}
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
}


