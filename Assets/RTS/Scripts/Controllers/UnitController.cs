using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour {

    public NavmeshPathfinding navmeshPathfinding;
    public Transform target;
    private float attackTimer;

    public UnitStats unitStats;
	public bool targetAcquired;



	private void Start()
    {
        navmeshPathfinding = GetComponent<NavmeshPathfinding>();
        attackTimer = unitStats.attackSpeed;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        if(Target != null)
        {
            var distance = (transform.position - Target.position).magnitude;

			if (distance <= unitStats.attackRange)
            {
 				Attack();
            }
        }
    }

	public void CancelOrder()
	{
		targetAcquired = false;
		Target = null;
	}

	public void StopShooting()
	{
		targetAcquired = false;
	}

	void Rotate(Transform target)
	{
		Vector3 targetDir = target.position - transform.position;

		// The step size is equal to speed times frame time.
		float step = unitStats.rotationSpeed * Time.deltaTime;

		Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
		Debug.DrawRay(transform.position, newDir, Color.red);

		// Move our position a step closer to the target.
		transform.rotation = Quaternion.LookRotation(newDir);
	}

	public void MoveUnit(Vector3 dest, List<GameObject> selectedUnits)
    {
        navmeshPathfinding.SetDestination(dest, selectedUnits);
    }

 

    public void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);
    }

    public void SetNewTarget(Transform enemy, List<GameObject> selectedUnits)
    {
 
        Target = enemy;
		Vector3 position = Target.position;
		Vector3 aimTarget = new Vector3();
		aimTarget.Set(position.x, position.y, position.z);
		targetAcquired = true;

        navmeshPathfinding.SetDestination(position, selectedUnits);

    }

    public void Attack()
    {
        if(attackTimer >= unitStats.attackSpeed)
        {
            RTSGameManager.UnitTakeDamage(this, Target.GetComponent<UnitController>());
            attackTimer = 0;
		}
        
    }

    public void TakeDamage(UnitController enemy, float damage)
    {
        StartCoroutine(Flasher(GetComponent<Renderer>().material.color));
    }

    IEnumerator Flasher(Color defaultColor)
    {
        var renderer = GetComponent<Renderer>();
        for (int i = 0; i < 2; i++)
        {
            renderer.material.color = Color.gray;
            yield return new WaitForSeconds(.05f);
            renderer.material.color = defaultColor;
            yield return new WaitForSeconds(.05f);
        }
    }


	public Transform Target
	{
		get
		{
			return target;
		}

		set
		{
			target = value;
		}
	}


}
