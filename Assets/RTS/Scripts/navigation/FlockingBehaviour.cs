using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockingBehaviour : MonoBehaviour
{

	private NavmeshPathfinding nvmPathfinding;
	public Vector3 target;
	private NavMeshAgent navmeshAgent;

	public FlockingAsset flockingAsset;
	public List<GameObject> targets;

	public List<GameObject> Targets { get => targets; set => targets = value; }
	public Vector3 desiredDirection = new Vector3();
	// Start is called before the first frame update
	void Start()
	{
		nvmPathfinding = GetComponent<NavmeshPathfinding>();
		targets = new List<GameObject>();
		navmeshAgent = GetComponent<NavMeshAgent>();

	}


	void FixedUpdate()
	{

		desiredDirection = BoidCohesion();
		desiredDirection += BoidSeparation();
		desiredDirection += Arrive();
		//desiredDirection += FlockingBehaviour.Align()  ;


		//navmeshAgent.velocity += desiredDirection;
		navmeshAgent.velocity += desiredDirection;
		navmeshAgent.velocity = Vector3.ClampMagnitude(navmeshAgent.velocity, nvmPathfinding.maxSpeed);
	}


	// Start is called before the first frame update


	// Update is called once per frame

	public Vector3 BoidCohesion()
	{

		Vector3 steering = new Vector3();
		int count = 0;

		foreach (GameObject other in targets) //iterate through the group of objects
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
		foreach (GameObject other in targets)
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


		if (target == null) return steering;

		Vector3 direction = target - transform.position;
		float distance = direction.magnitude;
		float targetSpeed;

		if (distance < flockingAsset.targetRadius) //if we are far away, do nothing
		{
			return steering;
		}

		if (distance > flockingAsset.slowRadius)
		{
			targetSpeed = nvmPathfinding.maxSpeed; //proceed at maxspeed
		}
		else
		{
			targetSpeed = nvmPathfinding.maxSpeed * distance / flockingAsset.slowRadius;
		}

		Vector3 desiredVelocity = direction;
		desiredVelocity.Normalize();
		desiredVelocity *= targetSpeed;
		steering = desiredVelocity - nvmPathfinding.velocity;
		steering /= flockingAsset.timeToTarget;

		if (steering.magnitude > nvmPathfinding.maxAccel)
		{
			steering.Normalize();
			steering *= nvmPathfinding.maxAccel;
		}
		return steering * flockingAsset.arrive;
	}

	public Vector3 Align()
	{

		Vector3 steering = new Vector3();
		//float targetOrientation = target.GetComponent<Agent>().orientation;
		//float rotation = targetOrientation - agent.orientation;
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
