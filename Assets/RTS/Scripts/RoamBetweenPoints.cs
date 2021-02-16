using UnityEngine;
using System.Collections;

public class RoamBetweenPoints : MonoBehaviour {

	public Transform[] targets;
	public LayerMask groundLayer;

	private Transform target;
	private UnityEngine.AI.NavMeshAgent agent;

	private Quaternion smoothTilt;

	private float waitFor = 5f;
	private float wait = 4f;
	private bool waiting = true;

	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		smoothTilt = new Quaternion();
	}

	void Update () {

		if(waiting){
			if(wait < waitFor){
				wait += Time.deltaTime;
				
				agent.destination = transform.position;
			}else{
				
				Transform newTarget = target;
				
				while(newTarget == target){
					int randomIndex = Random.Range(0,(targets.Length));
					target = targets[randomIndex];
				}
				
				agent.SetDestination(target.position);
				
				wait = 0f;
				waiting = false;

			}
		}


		float distanceToTarget = 0;

		if(target != null){
			distanceToTarget = Vector3.Distance(transform.position, target.position);
		}

		if(distanceToTarget < 20){
			waiting = true;
		}


		RaycastHit rcHit;
		Vector3 theRay = transform.TransformDirection(Vector3.down);
		
		if (Physics.Raycast(transform.position, theRay, out rcHit, 50f, groundLayer))
		{
			float GroundDis = rcHit.distance;
			Quaternion grndTilt = Quaternion.FromToRotation(Vector3.up, rcHit.normal);
			smoothTilt = Quaternion.Slerp(smoothTilt, grndTilt, Time.deltaTime * 2.0f);

			transform.rotation = smoothTilt * transform.rotation;

			Vector3 locPos = transform.localPosition;
			locPos.y = (transform.localPosition.y - GroundDis);
			transform.localPosition = locPos;
		}
	}
}
