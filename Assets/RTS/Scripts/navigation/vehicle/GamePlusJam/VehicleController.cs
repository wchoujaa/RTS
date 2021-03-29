using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

namespace Assets.RTS.Scripts.Navigation.GamePlusJam
{

	public enum VehcileType
	{
		Tank,
		Fourwheel
	}

	public class VehicleController : MonoBehaviour
	{
		public Rigidbody rb;
		public float forwardaccel = 8f, backwardaccel = 4f, turnstrength = 180f, MaxSpeed = 50f, gravityforce = 10f;
		[SerializeField]
		private float speedinput, turninput, turnAngle = 0.1f;
		private bool grounded;

		public LayerMask ground;
		public float raylenght = 5f;
		public Transform raystartpoint;
		[SerializeField] 
		private VehcileType type;

		public Vector3 Speed { get => rb.velocity;  }
		public Vector3 AngularSpeed { get => rb.angularVelocity; }


		void Start()
		{
			//spehererb = GetComponent<Rigidbody>();
			rb.transform.parent = null;
		}

		void Update()
		{

			if (grounded == true)
			{
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turninput * turnstrength * Time.deltaTime, 0f));
			}
			transform.position = rb.transform.position;
		}

 
		private void FixedUpdate()
		{
			grounded = false;
			RaycastHit hit;

			if (Physics.Raycast(raystartpoint.position, -transform.up, out hit, raylenght, ground))
			{
				grounded = true;

				transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
			} 
			Vector3 force;
			if (grounded == true)
			{
				force = transform.forward * speedinput;

				if (Mathf.Abs(speedinput) > 0)
				{
					force *= forwardaccel;
				}
				if (Mathf.Abs(speedinput) < 0)
				{
					force *= -backwardaccel;
				}
			}
			else
			{
				force = Vector3.up * -gravityforce;
			}


			if(type == VehcileType.Tank && Mathf.Abs(turninput) > turnAngle)
			{
				force = Vector3.zero;
			}
			rb.AddForce(force);
		}

 
		internal void Move(float v, float h, float v3, float v4)
		{
			speedinput = v;
			turninput = h;
		}
	}
}
