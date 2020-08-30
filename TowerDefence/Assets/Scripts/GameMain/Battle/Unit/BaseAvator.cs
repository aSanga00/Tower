using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle.Unit
{

    public class BaseAvator : MonoBehaviour
    {
        [SerializeField] private int controlId;
        [SerializeField] private int id;
        [SerializeField] private string unitName;
        [SerializeField] private int hp;
        [SerializeField] private int cost;
        [SerializeField] private int attack;
        [SerializeField] private int defence;
        [SerializeField] private float speed;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float searchRange;
        [SerializeField] private float attackRange;
        [SerializeField] private float coolTime;

        [SerializeField] private Transform target;

        [SerializeField] protected Animator animator;

        private Vector3 homePosition;
        private Vector3 targetPosition;
        private Action action;
        private Queue<int> targetQueue = new Queue<int>();

        public int ControlId {
            get { return controlId; }
            set { controlId = value; }
        }


        private Vector3 GetTargetPosition()
        {
            return targetPosition - transform.localPosition;
        }

        public void SetupUnitData(BaseParameter parameter)
        {
            id = parameter.id;
            unitName = parameter.name;
            hp = parameter.hp;
            cost = parameter.cost;
            attack = parameter.attack;
            defence = parameter.defence;
            speed = parameter.speed;
            attackSpeed = parameter.attackSpeed;
            searchRange = parameter.searchRange;
            coolTime = parameter.coolTime;
        }

        public void SetMoveTarget(Transform targetPos)
        {

        }

        public void AddTarget(int id)
        {
            
        }

        public void RemoveTarget(int id)
        {
           
        }

        // Use this for initialization
        private void Awake()
        {
            targetPosition = target.localPosition;
            homePosition = transform.localPosition;
            action = Move;
        }

        // Update is called once per frame
        public void UpdateAvator()
        {
            action.Invoke();
        }

        private void MovePosition()
        {
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(homePosition, targetPosition, step);
            homePosition = transform.localPosition;
        }

        protected virtual void Attack()
        {
            //animator.SetTrigger();
        }

        protected virtual void Defeat()
        {

        }

        protected virtual void Wait()
        {
         
        }

        protected virtual void Move()
        {
            MovePosition();
        }
       

    }
}