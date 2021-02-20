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






	// Start is called before the first frame update
	void Start()
	{

		agent = GetComponent<NavMeshAgent>();
		unitController = GetComponent<UnitController>();
		speed = unitController.unitStats.maxSpeed;
	}

	// Update is called once per frame
	void Update()
	{


	}

	void FixedUpdate()
	{


		//Debug.Log(" desired " + desiredDirection + ", " +navmeshAgent.velocity);
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


 
}
