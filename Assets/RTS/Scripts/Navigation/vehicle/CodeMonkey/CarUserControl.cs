using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
 


namespace Assets.RTS.Scripts.Navigation.CodeMonkey
{
	public class CarUserControl : MonoBehaviour
	{

		private CarController carController;

		void Start()
		{
			carController = GetComponent<CarController>();
		}


		void Update()
		{
			Controls();
		}


		private void Controls()
		{
			float v  = CrossPlatformInputManager.GetAxis("Vertical");
			float h = CrossPlatformInputManager.GetAxis("Horizontal");

			carController.SetInputs(v, h);
		}
 
	}
}