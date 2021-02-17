using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingBehaviour : MonoBehaviour
{


	[Header("Cohesion")]

	public float neighbordist = 20;
	public List<GameObject> targets;
	
	[Header("Separation")]
	public float desiredseparation = 15.0f;

	private PlayerUnitController pController;
	private NavmeshPathfinding agent;

	[Header("Arrive")]
	public float targetRadius = 5.0f;
	public float slowRadius = 15.0f;
	public float timeToTarget = 0.1f;

	// Start is called before the first frame update
	void Start()
	{
		pController = GetComponent<PlayerUnitController>();
	}

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
				if ((d > 0) && (d < neighbordist))
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

		return steering;

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
				if ((d > 0) && (d < desiredseparation))
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

		return steering;
	}



	public Vector3 Arrive()
	{
		GameObject target = agent.target;
		Vector3 steering = new Vector3();
		Vector3 direction = target.transform.position - transform.position;
		float distance = direction.magnitude;
		float targetSpeed;

		if (distance < targetRadius) //if we are far away, do nothing
		{
			return steering;
		}

		if (distance > slowRadius)
		{
			targetSpeed = agent.maxSpeed; //proceed at maxspeed
		}
		else
		{
			targetSpeed = agent.maxSpeed * distance / slowRadius;
		}

		Vector3 desiredVelocity = direction;
		desiredVelocity.Normalize();
		desiredVelocity *= targetSpeed;
		steering = desiredVelocity - agent.velocity;
		steering /= timeToTarget;

		if (steering.magnitude > agent.maxAccel)
		{
			steering.Normalize();
			steering *= agent.maxAccel;
		}
		return steering;
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
		return steering;
	}
}
