using Assets.RTS.Scripts.Controllers;
using Assets.RTS.Scripts.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SelectionGraph : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject graphNodePrefab;
	private List<GraphNode> graph = new List<GraphNode>();
	public int rand = 3;
	public float distance = 1f;
	public float spring = 10f;
	public float radius = 0.5f;
	public float mass = 0.5f;
	public float gravity;
	public float repulsion;
	public float repulsionDistance;

	public float resistance;
	public float startSpread = 2f;
	public float dampingRatio = 0.15f;

	bool reset = false;
	public LayerMask ground;

	public int testNode = 11;
	private SphereCollider sphereCollider;
	public float sphereRadius = 0f;
	public List<GraphNode> Graph { get => graph; set => graph = value; }

	void Start()
	{
		sphereCollider = GetComponent<SphereCollider>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			//ClearGraph();
			//NewGraph();
		}
	}

	// Update is called once per frame
	void FixedUpdate()
	{


		//Center();
		sphereCollider.radius = sphereRadius;

	}



	private void Center()
	{

		foreach (GraphNode obj in Graph)
		{

			Vector3 direction = obj.transform.position - transform.position;
			var distance = direction.magnitude;
			var gravityForce = gravity / (distance * distance);
			var repulsionForce = repulsion / (distance * distance);

			Debug.Log(gravityForce);
			//Vector2 resistanceForce = direction.normalized * resistance * -1f;
			if(gravityForce != Mathf.Infinity && gravityForce != Mathf.NegativeInfinity)	
				obj.GetComponent<Rigidbody>().AddForce(direction * gravityForce);
			if (repulsionForce != Mathf.Infinity && repulsionForce != Mathf.NegativeInfinity)
				obj.GetComponent<Rigidbody>().AddForce(direction * repulsionForce);
		}
	}

	public void Reset(List<UnitController> lists)
	{

		if (!reset)
			StartCoroutine(ResetGraph(lists));
	}


	IEnumerator ResetGraph(List<UnitController> lists)
	{
		reset = true;
		int count = Graph.Count;
		int n = lists.Count;

		if (n == count)
		{
			reset = false;
			yield break;
		}

		bool add = n > count;



		if (add)
		{
			for (int i = count; i < n; i++)
			{
				AddNode();
			}
		}
		else
		{
			for (int i = count; i > n; i--)
			{
				RemoveNode();
			}
		}


		for (int i = 0; i < graph.Count; i++)
		{
			UnitController unitController = lists[i];
			GraphNode node = graph[i];
			node.SetUnitController(unitController);
			node.SetRadius(unitController.Radius);
		}


		reset = false;
		yield return new WaitForSeconds(0.2f);
	}


	private void RemoveNode()
	{
		GraphNode node = Graph[0];
		if (node != null)
		{
			Graph.Remove(node);
			DestroyImmediate(node.gameObject);

		}
		//if (Graph.Count > 0)
		//	Graph[0].FreezePosition();

	}


	private void AddNode()
	{
		GameObject obj = Instantiate(graphNodePrefab, transform);
		GraphNode node = obj.GetComponent<GraphNode>();
		node.Build(this);

		//if (Graph.Count == 0)
		//{
		//	node.FreezePosition();

		//}

		Graph.Add(node);
		GenerateEdges(node);
	}


	private void GenerateEdges(GraphNode node)
	{
		int j = 0;
		int n = Graph.Count;
		//Debug.Log(n + " " + j);
		while (j < rand && j < n - 1)
		{
			GraphNode node2 = Graph[Random.Range(0, n)];
			//Debug.Log(j + " " + n + " "  + (j < n - 1));

			if (node == node2 || node.IsConnectedTo(node2)) continue;

			node.ConnectTo(node2, this);

			j++;
		}
	}

	public void Clear()
	{
		foreach (GraphNode obj in Graph)
		{
			Destroy(obj.gameObject);
		}
		Graph.Clear();

	}


	public void GenerateTest()
	{
		Clear();
		var radius1 = 1.5f;
		var radius2 = 3f;
		var percent = 0.8;

		for (int i = 0; i < testNode; i++)
		{
			AddNode();
		}

		foreach (var node in graph)
		{
			if (Random.value < percent)
				node.SetRadius(radius1);
			else
				node.SetRadius(radius2);
		}
	}

}
