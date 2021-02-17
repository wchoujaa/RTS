using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class NavmeshPathfinding : MonoBehaviour
{


	public Vector3 target;
	[Header("Agent Settings")]
	public float maxSpeed;
	public float maxAccel;
	public float maxAngularSpeed;

	private NavMeshAgent navmeshAgent;
	private FlockingBehaviour flockingBehaviour;
	Vector3 desiredDirection = new Vector3();

	public Vector3 velocity { get => navmeshAgent.velocity; set => navmeshAgent.velocity = value; }




	// Start is called before the first frame update
	void Start()
	{

		navmeshAgent = GetComponent<NavMeshAgent>();
		flockingBehaviour = GetComponent<FlockingBehaviour>();
	}

	// Update is called once per frame
	void Update()
	{


	}

	void FixedUpdate()
	{


		//Debug.Log(" desired " + desiredDirection + ", " +navmeshAgent.velocity);
	}


 

	public void SetDestination(Vector3 target, List<GameObject> selectedUnits)
	{
		this.target = target;
		flockingBehaviour.Targets = selectedUnits;
		flockingBehaviour.target = target;
		navmeshAgent.speed = maxSpeed;
		navmeshAgent.acceleration = maxAccel;
		navmeshAgent.angularSpeed = maxAngularSpeed;
		navmeshAgent.SetDestination(target);
	}
}
