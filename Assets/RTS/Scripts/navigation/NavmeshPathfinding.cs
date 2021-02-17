using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshPathfinding : MonoBehaviour
{
	public GameObject target;
    public float maxSpeed;
    public Vector3 velocity;
    public float maxAccel;
    private FlockingBehaviour FlockingBehaviour;
    private NavMeshAgent navmeshAgent;

    [Header("weights")]
    public float cohesion = 1f;
    public float separation = 1f;
    public float alignement = 1f;
    public float arrive = 1f;
	// Start is called before the first frame update
	void Start()
    {
        FlockingBehaviour = GetComponent<FlockingBehaviour>();
        navmeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void FixedUpdate()
	{
        Vector3 desiredDirection = new Vector3();
        desiredDirection += FlockingBehaviour.BoidCohesion() * cohesion;
        desiredDirection += FlockingBehaviour.BoidSeparation() * separation;
        //desiredDirection += FlockingBehaviour.Arrive() * arrive;
        //desiredDirection += FlockingBehaviour.Align() * alignement;

 
        navmeshAgent.velocity += desiredDirection * Time.deltaTime;
    }

	public void SetDestination(Vector3 target, List<GameObject> selectedUnits)
	{
        FlockingBehaviour.targets = selectedUnits;
         navmeshAgent.SetDestination(target);
    }


}
