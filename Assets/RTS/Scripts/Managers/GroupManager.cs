using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{

	//foreign objects should only access the removeFromGroup() and addToGroup() functions
	//these two functions should handle all cases

	//dictionary links target gameobject, to a list of movement group objects 
	public Dictionary<Vector3, Group> groupTable = new Dictionary<Vector3, Group>();
	float lookdistance = 50.0f;
	public int groupCount = 0;
	private void Update()
	{
		groupCount = groupTable.Count;
	}

	public Group AddToGroup(Vector3 target, List<GameObject> members)
	{
		Group group = null;

		foreach (GameObject member in members)
		{
			group = addToGroup(target, member);
		}

		return group;

	}

	public Group addToGroup(Vector3 target, GameObject member)
	{
		return addGroup(target, member);
	}


	//remove gameobject from movement groups
	public void removeFromGroup(Vector3 target, GameObject member)
	{
		if (groupTable.ContainsKey(target)) //check if a list for our target exists
		{
			Group group = groupTable[target];


			if (group.containsMember(member)) //if we are in this group
			{
				member.GetComponent<UnitController>().IsGroupLeader = false;

				group.removeMember(member); //remove us and return

				if (group.isEmpty()) //if this movement group is now empty
				{
					groupTable.Remove(target);

				}

				return;

			}
		}
	}


	//find parent ground
	public Group getParentGroup(Vector3 target, GameObject member)
	{
		if (groupTable.ContainsKey(target)) //check if a list for our target exists
		{
			Group group = groupTable[target];


			if (group.containsMember(member)) //if we are in this group
			{
				return group;
			}

		}

		return new Group(target);
	}


	//========================================================================================================================//


	//add a new group for the specified target
	Group addGroup(Vector3 target, GameObject leader)
	{
		Group group = null;

		if (groupTable.ContainsKey(target)) //if we have a list for the target
		{
			group = groupTable[target];
		}
		else
		{
			group = new Group(target);
			groupTable.Add(target, group); //put the list into the table
		}

		group.addMember(leader);
		group.leader = leader;
		group.targetReached = false;

		return group;
	}

	//look for a specific gameobject with a specified target
	Group insertIfMemberHasTarget(Vector3 target, GameObject member, GameObject new_member)
	{
		if (groupTable.ContainsKey(target)) //see if there is a movement group list for this target
		{
			Group mg = groupTable[target];

			foreach (GameObject unit in mg.members) //look through every unit in the group
			{
				if (unit == member) //check if the member is in the group
				{
					mg.addMember(new_member);

					//new_member.GetComponent<seekTarget>().mg = mg; //set the object's movement group;
					return mg; //return true
				}

			}
		}

		return null;
	}

	//tell us if a movement group exists for this target
	bool containsGroup(Vector3 target)
	{
		return groupTable.ContainsKey(target);
	}



	////add a gameObject to the group specified
	//public Group addToGroup(Vector3 target, GameObject member)
	//{

	//	Group group = null;

	//	if (containsGroup(target))//check if there is a list with the specified target
	//	{
	//		//Debug.Log("A group with target: " + target + " exists!");

	//		//perform a spherecast on all nearby units to see if any of them are already in a group in this list
	//		Collider[] hitColliders = Physics.OverlapSphere(member.transform.position, lookdistance);

	//		foreach (Collider c in hitColliders)
	//		{

	//			//Debug.Log("Colliders hit!");
	//			group = insertIfMemberHasTarget(target, c.gameObject, member);

	//			if (group != null)//check if any of our neighbors have the same target
	//			{
	//				break;
	//			}
	//		}

	//		if (group == null)
	//		{
	//			group = addGroup(target, member); //add a new group with self as the leader

	//		}
	//	}
	//	else
	//	{
	//		group = addGroup(target, member); // add a new group with self as the leader
	//	}
	//	return group;
	//}


}
