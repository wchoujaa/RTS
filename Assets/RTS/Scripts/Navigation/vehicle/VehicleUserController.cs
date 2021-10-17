using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using VehicleController = Assets.RTS.Scripts.Navigation.GamePlusJam.VehicleController;
namespace Assets.RTS.Scripts.Navigation
{
	public class VehicleUserController : MonoBehaviour
	{

		private VehicleController carController;

		void Start()
		{
			carController = GetComponent<VehicleController>();
		}


		void Update()
		{
			Controls();
		}


		private void Controls()
		{
			float v = CrossPlatformInputManager.GetAxis("Vertical");
			float h = CrossPlatformInputManager.GetAxis("Horizontal");

			carController.Move(v, h, 0, 0);
		}

	}
}