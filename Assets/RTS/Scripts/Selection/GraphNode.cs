
using Assets.RTS.Scripts.Controllers;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.RTS.Scripts.Selection
{
	public class GraphNode : MonoBehaviour
	{
		Rigidbody rb;
		SphereCollider sphereCollider;
		List<SpringJoint> joints = new List<SpringJoint>();
		public UnitController unitController;
		private int ground;
		public float Radius { get => sphereCollider.radius; }

		private void Awake()
		{
			DestroyImmediate(GetComponent<SphereCollider>());
			rb = gameObject.AddComponent<Rigidbody>();
			rb.constraints = RigidbodyConstraints.FreezePositionY;
			sphereCollider = gameObject.AddComponent<SphereCollider>();

		}

		public void Build(SelectionGraph selectionGraph, UnitController uController)
		{
			ground = selectionGraph.ground;
			float startSpread = selectionGraph.startSpread;
			rb.mass = selectionGraph.mass;
			gameObject.layer = selectionGraph.transform.gameObject.layer;
			gameObject.transform.parent = selectionGraph.transform;
			gameObject.transform.position = selectionGraph.transform.position + Vector3.right * Random.Range(-startSpread, startSpread) + Vector3.forward * Random.Range(-startSpread, startSpread);
			sphereCollider.radius = 1.0f;

		}


		public void SetUnitController(UnitController uController)
		{
			this.unitController = uController;

		}

		public void SetRadius(float radius)
		{
			transform.localScale = Vector3.one * radius;
		}



		private void FixedUpdate()
		{

			RaycastHit hit;
			Vector3 pos = transform.position + Vector3.up * 2.3f;


			if (Physics.Raycast(pos, Vector3.down, out hit, 50f, ground))
			{
				//use below code if your pivot point is in the middle
				transform.position = hit.point;

			}
			else if (Physics.Raycast(pos, Vector3.up, out hit, 50f, ground))
			{
				//use below code if your pivot point is in the middle
				transform.position = hit.point;
			}

			//transform.localPosition = pos;
		}




		public bool IsConnectedTo(GraphNode obj2)
		{
			bool value = false;

			for (int i = 0; i < joints.Count; i++)
			{
				Joint joint = joints[i];
				if (joint.connectedBody == obj2)
				{
					value = true;
					break;
				}
			}
			return value; //gameObject.GetComponent<SpringJoint>() != null;//&& gameObject.GetComponent<SpringJoint>().connectedBody == obj2;
		}

		public void ConnectTo(GraphNode obj2, SelectionGraph selectionGraph)
		{

			SpringJoint joint = gameObject.AddComponent<SpringJoint>();
			joint.connectedBody = obj2.GetComponent<Rigidbody>();
			joint.autoConfigureConnectedAnchor = false;
			joint.anchor = Vector3.zero;
			joint.connectedAnchor = Vector3.zero;
			//joint.distance = distance;
			//joint.dampingRatio = dampingRatio; 
			//joint.minDistance = distance;
			joint.enableCollision = true;
			joint.damper = selectionGraph.dampingRatio;
			joints.Add(joint);
		}

		public void FreezePosition()
		{
			transform.position = transform.parent.position;
			rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		}
	}
}
