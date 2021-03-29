using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public float steer;
        public float accel;
        public float footbrake;
        public float brake;

        public bool debug = false;
        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
			// pass the input to the car!
			if (!debug)
			{
                steer = CrossPlatformInputManager.GetAxis("Horizontal");
                accel = CrossPlatformInputManager.GetAxis("Vertical");
                footbrake = accel;
#if !MOBILE_INPUT
                brake = CrossPlatformInputManager.GetAxis("Jump");
#endif
            }

            m_Car.Move(steer, accel, footbrake, brake); 
 
            }
        }
}
