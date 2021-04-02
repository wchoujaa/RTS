using Assets.RTS.Scripts.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Group
{
	public List<GameObject> members = new List<GameObject>(); //list of units in the group
															  //Dictionary<int, bool> isLeader = new Dictionary<int, bool>(); //maps ID's to whether or not the unit is a leader
	public GameObject leader;

	public float maxSpeed = float.MaxValue; //

	public Vector3 target;
	public Vector3 orientation;
	public float leaderRadius;
	public float separationValue;
	//public List<Vector3> waypoints = new List<Vector3>();
 	public bool TargetReached { get; internal set; }

	//add a new member to the group



	public Group(Vector3 target)
	{
		this.target = target;


	}

 

	public void addMember(GameObject member)
	{

		if (leader == null) //if there is no leader
		{
			leader = member;
			//set leader flag
			member.GetComponent<UnitController>().IsGroupLeader = true;
			orientation = target - member.transform.position;
			orientation.y = 0f;
		}

		if (member.GetComponent<UnitController>().unitStats.maxSpeed < maxSpeed) //check who the slowest member of the movement group is
		{
			maxSpeed = member.GetComponent<UnitController>().unitStats.maxSpeed; //set everyone to match the slowest member's pace
		}

		members.Add(member);
		TargetReached = false;
	}

	//remove a unit from the group and assign a new leader if the previous leader is removed
	public void removeMember(GameObject member)
	{
		if (member == leader) //if the group leader is the object we are removing
		{
			members.Remove(member); //remove the leader from the list of units
			member.GetComponent<UnitController>().IsGroupLeader = false; //set unit leader flag to FALSE

			if (members.Count > 0)
			{
				if (members[0] != null)
				{
					leader = members[0]; //re-add the new top member as the leader
										 //update the leader's behavior
					leader.GetComponent<UnitController>().IsGroupLeader = true; //set unit leader flag to TRUE
				}
			}
			else
			{
				leader = null;
			}
		}
		else
		{
			members.Remove(member);
		}
	}

	public bool containsMember(GameObject member)
	{
		return members.Contains(member);
	}

	public bool isEmpty()
	{
		return (!(members.Count > 0));
	}
}
