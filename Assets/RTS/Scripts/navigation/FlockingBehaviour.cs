using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockingBehaviour : MonoBehaviour
{

	public Group group;


	private NavMeshAgent navmeshAgent;

	public FlockingAsset flockingAsset;


	public Vector3 desiredDirection = new Vector3();

	private UnitController uController;
	public bool targetReached = false;
	public bool nearLeader;

	// Start is called before the first frame update
	void Start()
	{

		navmeshAgent = GetComponent<NavMeshAgent>();
		uController = GetComponent<UnitController>();
		StartCoroutine(FlockingUpdate());
	}


	IEnumerator FlockingUpdate()
	{
		while (true)
		{
			if (group != null)
			{
 				desiredDirection = BoidCohesion();
				desiredDirection += BoidSeparation();
				desiredDirection += Arrive();

				desiredDirection = desiredDirection.normalized * flockingAsset.multiplier;




				if (TargetReached())
				{
					if (uController.isGroupLeader)
					{
						group.TargetReached = true;
					}
					if (group.TargetReached)
					{
						navmeshAgent.SetDestination(transform.position);
					}
				}



				if (uController.IsGroupLeader)
				{
					navmeshAgent.nextPosition += desiredDirection * flockingAsset.leaderFlocking;

				}
				else
				{
					navmeshAgent.nextPosition += desiredDirection * flockingAsset.flocking;

				}


				// Use for Debugging
				targetReached = TargetReached();
				nearLeader = IsNearLeader();



			}
			yield return new WaitForSeconds(flockingAsset.updateRate);

		}



	}

	private void Rotate()
	{

		// Determine which direction to rotate towards

		// The step size is equal to speed times frame time.
		float singleStep = uController.unitStats.rotationSpeed * Time.deltaTime;

		// Rotate the forward vector towards the target direction by one step
		Vector3 newDirection = Vector3.RotateTowards(transform.forward, group.orientation, singleStep, 0.0f);

		// Draw a ray pointing at our target in
		Debug.DrawRay(transform.position, newDirection, Color.red);

		// Calculate a rotation a step closer to the target and applies rotation to this object
		transform.rotation = Quaternion.LookRotation(newDirection);

	}

	private bool IsNearLeader()
	{
		return (group.leader.transform.position - transform.position).magnitude < flockingAsset.maxFlockingRadius;

	}

	private IEnumerator DelayTargetNotification()
	{

		yield return new WaitForSeconds(flockingAsset.targetReachedNotifyTime);
		targetReached = true;
		navmeshAgent.SetDestination(transform.position);
	}

	private bool TargetReached()
	{
		return (group.target - transform.position).magnitude < flockingAsset.targetRadius;
	}


	// Start is called before the first frame update


	// Update is called once per frame

	public Vector3 BoidCohesion()
	{

		Vector3 steering = new Vector3();
		int count = 0;

		foreach (GameObject other in group.members) //iterate through the group of objects
		{

			if (other != null)
			{

				float d = (transform.position - other.transform.position).magnitude;
				if ((d > 0) && (d < flockingAsset.neighbordist))
				{
					steering += other.transform.position; // Add position
					count++;
				}

			}
		}

		if (count > 0) //average the positions of all the objects
		{
			steering /= count;
			steering = steering - transform.position;
		}

		return steering * flockingAsset.cohesion;

	}



	public Vector3 BoidSeparation()
	{


		Vector3 steering = new Vector3();
		int count = 0;

		// For every boid in the system, check if it's too close
		foreach (GameObject other in group.members)
		{

			if (other != null)
			{

				float d = (transform.position - other.transform.position).magnitude;

				// If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
				if ((d > 0) && (d < flockingAsset.desiredseparation))
				{
					// Calculate vector pointing away from neighbor
					Vector3 diff = transform.position - other.transform.position;
					diff.Normalize();
					diff /= d;        // Weight by distance
					steering += diff;
					count++;            // Keep track of how many
				}

			}
		}

		// Average -- divide by how many
		if (count > 0)
		{
			//steering.linear /= (float)count;
		}

		return steering * flockingAsset.separation;
	}



	public Vector3 Arrive()
	{


		Vector3 steering = new Vector3();
		Vector3 direction = group.target - transform.position;
		float distance = direction.magnitude;
		float targetSpeed;

		if (distance < flockingAsset.targetRadius) //if we are far away, do nothing
		{
			return steering;
		}

		if (distance > flockingAsset.slowRadius)
		{
			targetSpeed = uController.unitStats.maxSpeed; //proceed at maxspeed
		}
		else
		{
			targetSpeed = uController.unitStats.maxSpeed * distance / flockingAsset.slowRadius;
		}

		Vector3 desiredVelocity = direction;
		desiredVelocity.Normalize();
		desiredVelocity *= targetSpeed;
		steering = desiredVelocity - uController.Velocity;
		steering /= flockingAsset.timeToTarget;

		if (steering.magnitude > uController.unitStats.maxAccel)
		{
			steering.Normalize();
			steering *= uController.unitStats.maxAccel;
		}
		return steering * flockingAsset.arrive;
	}

	public Vector3 Align()
	{

		Vector3 steering = new Vector3();

		if (!TargetReached()) return steering;

		Vector3 direction = group.target - transform.position;

		//create the rotation we need to be in to look at the target
		Quaternion lookRotation = Quaternion.LookRotation(direction);

		//rotate us over time according to speed until we are in the required rotation
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * uController.unitStats.rotationSpeed);


		float targetOrientation = 0f;
		//float targetOrientation = 0f;//target.GetComponent<Agent>().orientation;
		//float rotation = targetOrientation - navmeshAgent.orientation;
		//rotation = MapToRange(rotation);
		//float rotationSize = Mathf.Abs(rotation);

		//if (rotationSize < targetRadius)
		//{
		//	return steering;
		//}

		//float targetRotation;

		//if (rotationSize > slowRadius)
		//{
		//	targetRotation = agent.maxRotation;
		//}
		//else
		//{
		//	targetRotation = agent.maxRotation * rotationSize / slowRadius;
		//}

		//targetRotation *= rotation / rotationSize;
		//steering.angular = targetRotation - agent.rotation;
		//steering.angular /= timeToTarget;
		//float angularAccel = Mathf.Abs(steering.angular);

		//if (angularAccel > agent.maxAngularAccel)
		//{
		//	steering.angular /= angularAccel;
		//	steering.angular *= agent.maxAngularAccel;
		//}
		return steering * flockingAsset.alignement;
	}
}




//// if within the group at a small distance
//if (IsNearLeader() && !uController.IsGroupLeader)
//{
//	// if one member of the group is at target all stops
//	if (TargetReached() || group.TargetReached)
//	{
//		group.TargetReached = true;
//		//navmeshAgent.SetDestination(transform.position);
//		//navmeshAgent.updateRotation = false;
//		//navmeshAgent.updatePosition = false;
//		Rotate();
//		navmeshAgent.velocity = desiredDirection * flockingAsset.flocking;
//	}
//	else
//	{
//		//navmeshAgent.updateRotation = true;
//		navmeshAgent.velocity = navmeshAgent.velocity + desiredDirection * flockingAsset.flocking * flockingAsset.flocking;

//	}
//}
//// leader navigation logic
//if (uController.IsGroupLeader)
//{
//	if (TargetReached())
//	{
//		group.TargetReached = true;
//		navmeshAgent.SetDestination(transform.position);
//		//navmeshAgent.updateRotation = false;
//		Rotate();
//	}
//	else
//	{
//		navmeshAgent.velocity = navmeshAgent.velocity + desiredDirection * flockingAsset.leaderFlocking;
//		DrawArrow.ForDebug(group.target, group.orientation);
//		//navmeshAgent.updateRotation = true;
//	}
//}
//// if not witih group reach 
//if (!IsNearLeader() && !uController.isGroupLeader)
//{
//	//navmeshAgent.updateRotation = true;
//	//navmeshAgent.updatePosition = true;
//	navmeshAgent.velocity = navmeshAgent.velocity + desiredDirection * (1 - flockingAsset.flocking);
//}


