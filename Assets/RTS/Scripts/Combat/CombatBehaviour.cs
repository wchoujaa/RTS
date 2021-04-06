using Assets.RTS.Scripts.Controllers;
using LOS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.RTS.Scripts.Combat
{
    public class CombatBehaviour : MonoBehaviour
    {
        public CombatStats combatStats;
        public Transform target;
        private float attackTimer;
        public bool targetAcquired;
        private UnitController unitController;
        private int health;
        public LayerMask unit;
        private bool takingDamage = false;
        // Start is called before the first frame update
        private GameObject pointer;
        public string pointerTag = "Pointer";
        private float targetSearchDelay = 1f;
        public bool debug = true;

        void Start()
        {

            attackTimer = combatStats.rate;
            health = combatStats.health;
            unitController = GetComponent<UnitController>();
            pointer = GameObject.FindGameObjectWithTag(pointerTag);

            StartCoroutine(LookForTarget());
        }

        private void Update()
        {
            attackTimer += Time.deltaTime;

            if (target != null)
            {
                Attack();
            }
        }


        private void FixedUpdate()
        { 
            if (unitController.IsSelected)
            { 
                SetNewTarget(pointer.transform); 
            }
        }

 
        void OnDrawGizmos()
        {
            if (debug && target != null)
            {
                // Draw a yellow sphere at the transform's position
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(target.position, 1f);
            }
        }

        public void Attack()
        {
            //if (attackTimer >= combatStats.rate)
            //{
            //	CombatBehaviour targetBehaviour = target.GetComponent<CombatBehaviour>();

            //	targetBehaviour.TakeDamage(this);
            //	attackTimer = 0;
            //}

        }

        public void SetNewTarget(Transform target)
        {
            this.target = target;
            targetAcquired = true;
        }

        public void TakeDamage(CombatBehaviour enemy)
        {
            health -= enemy.combatStats.damage;
            if (!takingDamage)
                StartCoroutine(Flasher(unitController.colorRenderer.material.color));
        }


        IEnumerator LookForTarget()
        {
            yield return new WaitForSeconds(1f); // New line

            while (true)
            {
                if (!unitController.IsSelected)
                {

                    Collider[] hitColliders = Physics.OverlapSphere(transform.position, combatStats.range, unit);
                    var distance = Mathf.Infinity;
                    foreach (var hitCollider in hitColliders)
                    {
                        var target = hitCollider.GetComponent<UnitController>();
                        if (target == null) continue;
                        var los = target.GetComponentInChildren<LOSCuller>();
                        var newDist = (transform.position - target.transform.position).magnitude;
                        //Debug.Log(los);
                        if (los != null && los.Visibile && target.team != unitController.team && newDist < distance)
                        {
                            SetNewTarget(hitCollider.transform);
                            distance = newDist;
                        }
                    }
                }

                yield return new WaitForSeconds(targetSearchDelay);
            }
        }

        IEnumerator Flasher(Color defaultColor)
        {
            takingDamage = true;
            var renderer = unitController.colorRenderer;
            for (int i = 0; i < 2; i++)
            {
                renderer.material.color = Color.gray;
                yield return new WaitForSeconds(.05f);
                renderer.material.color = defaultColor;
                yield return new WaitForSeconds(.05f);
            }
            takingDamage = false;
        }

        public void Disangage()
        {
            targetAcquired = false;
            target = null;

        }

        public bool IsEnemy(CombatBehaviour unit)
        {
            return this.unitController.team != unit.unitController.team;
        }
    }
}
