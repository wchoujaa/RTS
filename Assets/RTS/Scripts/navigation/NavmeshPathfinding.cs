using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavmeshPathfinding : MonoBehaviour
{


	public Vector3 destination = Vector3.negativeInfinity;
	[HideInInspector]
	public NavMeshAgent agent;
	private UnitController unitController;
	public float speed; 
	private UnitController uController; 
		public Group group;


	// Start is called before the first frame update
	void Start()
	{

		agent = GetComponent<NavMeshAgent>();
		unitController = GetComponent<UnitController>();
		speed = unitController.unitStats.maxSpeed;
		 agent.updatePosition = false;
		//agent.updateRotation = false;
	}

	// Update is called once per frame
	void Update()
	{


	}

	void FixedUpdate()
	{


		//Debug.Log(" desired " + desiredDirection + ", " +navmeshAgent.velocity);
	}



	public Vector3[] GetPath()
	{
		return agent.path.corners;
	}

	public void SetDestination(Vector3 target)
	{
		this.destination = target;

		agent.speed = speed;
		agent.acceleration = unitController.unitStats.maxAccel;
		agent.angularSpeed = unitController.unitStats.maxAngularSpeed;
		agent.stoppingDistance = unitController.unitStats.stoppingDistance;
		agent.SetDestination(target);
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



}
