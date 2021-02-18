using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupManager : MonoBehaviour
{

	//foreign objects should only access the removeFromGroup() and addToGroup() functions
	//these two functions should handle all cases

	//dictionary links target gameobject, to a list of movement group objects 
	Dictionary<Vector3, List<Group>> groupTable = new Dictionary<Vector3, List<Group>>();

	float lookdistance = 50.0f;

	//add a gameObject to the group specified
	public void addToGroup(Vector3 target, GameObject member)
	{
		if (containsGroup(target))//check if there is a list with the specified target
		{
			//Debug.Log("A group with target: " + target + " exists!");

			//perform a spherecast on all nearby units to see if any of them are already in a group in this list
			Collider[] hitColliders = Physics.OverlapSphere(member.transform.position, lookdistance, (1 << 9));
			foreach (Collider c in hitColliders)
			{

				//Debug.Log("Colliders hit!");

				if (insertIfMemberHasTarget(target, c.gameObject, member) == true)//check if any of our neighbors have the same target
				{
					return;
				}
			}
			addGroup(target, member); //add a new group with self as the leader
		}
		else
		{
			addGroup(target, member); // add a new group with self as the leader
		}
	}

	//remove gameobject from movement group
	public void removeFromGroup(Vector3 target, GameObject member)
	{
		if (groupTable.ContainsKey(target)) //check if a list for our target exists
		{
			foreach (Group mg in groupTable[target]) //loop through all movement groups in the list
			{
				if (mg.containsMember(member)) //if we are in this group
				{
					mg.removeMember(member); //remove us and return

					if (mg.isEmpty()) //if this movement group is now empty
					{
						groupTable[target].Remove(mg); //remove it from the list

						if (groupTable[target].Count == 0) //if the list is now empty
						{
							groupTable.Remove(target); //remove the list altogether
						}

					}

					return;
				}
			}
		}
	}


	//find parent ground
	public Group getParentGroup(Vector3 target, GameObject member)
	{
		if (groupTable.ContainsKey(target)) //check if a list for our target exists
		{
			foreach (Group mg in groupTable[target]) //loop through all movement groups in the list
			{
				if (mg.containsMember(member)) //if we are in this group
				{
					return mg;
				}
			}
		}

		return new Group();
	}


	//========================================================================================================================//


	//add a new group for the specified target
	void addGroup(Vector3 target, GameObject leader)
	{
		if (groupTable.ContainsKey(target)) //if we have a list for the target
		{
			Group new_group = new Group(); //add a new movement group to the list
			new_group.addMember(leader);
			new_group.leader = leader;
			groupTable[target].Add(new_group);
		}
		else
		{
			List<Group> mg_list = new List<Group>(); //create a new list and add a movement group to it
			Group new_group = new Group();
			new_group.addMember(leader);
			new_group.leader = leader;
			mg_list.Add(new_group);
			groupTable.Add(target, mg_list); //put the list into the table
		}
	}

	//look for a specific gameobject with a specified target
	bool insertIfMemberHasTarget(Vector3 target, GameObject member, GameObject new_member)
	{
		if (groupTable.ContainsKey(target)) //see if there is a movement group list for this target
		{
			foreach (Group mg in groupTable[target]) //iterate through the list
			{
				foreach (GameObject unit in mg.group) //look through every unit in the group
				{
					if (unit == member) //check if the member is in the group
					{
						mg.addMember(new_member);

						//new_member.GetComponent<seekTarget>().mg = mg; //set the object's movement group;
						return true; //return true
					}
				}
			}
		}

		return false;
	}

	//tell us if a movement group exists for this target
	bool containsGroup(Vector3 target)
	{
		return groupTable.ContainsKey(target);
	}

}
