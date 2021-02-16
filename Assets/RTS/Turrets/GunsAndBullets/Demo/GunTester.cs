using UnityEngine;

namespace GNB.Demo
{
    public class GunTester : MonoBehaviour
    {
        public Gun[] guns;
		public Transform target;
        private void Update()
        {
			foreach (var gun in guns)
			{
				Vector3 newDir = Vector3.RotateTowards(gun.transform.forward, target.transform.position, Time.deltaTime, 0.0f);
				Debug.DrawRay(transform.position, newDir, Color.red);

				// Move our position a step closer to the target.
				gun.transform.rotation = Quaternion.LookRotation(newDir);
			}
			if (Input.GetMouseButton(0) == true)
            {
                foreach (var gun in guns)
                {

                    gun.Fire(gun.transform.forward);
                }
            }
        }
    }
}
