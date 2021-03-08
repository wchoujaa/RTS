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

	private List<GraphNode> graph = new List<GraphNode>();
	public int rand = 3;
	public float distance = 1f;
	public float spring = 10f;
	public float radius = 0.5f;
	public float mass = 0.5f;
	public float gravity;
	public float resistance;
	public float startSpread = 2f;
	public float dampingRatio = 0.15f;
	public Vector3 center;
	bool reset = false;
	public LayerMask ground;

	public List<GraphNode> Graph { get => graph; set => graph = value; }

	void Start()
	{

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


		Center();


	}

 

	private void Center()
	{
		foreach (GraphNode obj in Graph)
		{
			Vector2 direction = obj.transform.position - transform.position;
			direction *= Time.deltaTime * gravity * 10000f;
			//Vector2 resistanceForce = direction.normalized * resistance * -1f;

			obj.GetComponent<Rigidbody>().AddForce(direction);// += direction;
															  //obj.transform.position += direction;

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
				UnitController unitController = lists[i];
				AddNode(unitController);
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
		if(Graph.Count > 0)
			Graph[0].FreezePosition();

	}


	private void AddNode(UnitController uController)
	{
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		GraphNode node = obj.AddComponent<GraphNode>();
		node.Build(this, uController);
		if (Graph.Count == 0)
			node.FreezePosition();
		Graph.Add(node);
		

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

 
}
