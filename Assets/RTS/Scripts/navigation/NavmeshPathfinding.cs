using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavmeshPathfinding : MonoBehaviour
{


	public Vector3 target = Vector3.negativeInfinity;

	private NavMeshAgent navmeshAgent;
	private UnitController unitController;






	// Start is called before the first frame update
	void Start()
	{

		navmeshAgent = GetComponent<NavMeshAgent>();
		unitController = GetComponent<UnitController>();
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
		this.target = target;
		navmeshAgent.speed = unitController.unitStats.maxSpeed;
		navmeshAgent.acceleration = unitController.unitStats.maxAccel;
		navmeshAgent.angularSpeed = unitController.unitStats.maxAngularSpeed;
		navmeshAgent.SetDestination(target);
	}
}
