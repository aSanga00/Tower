using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle.Unit
{
    public enum UnitType { player, enemy, neutral,playerTower, enemyTower};

    public enum ActionType { wait,move,attack,defeat,end };

    public class BaseAvator : MonoBehaviour
    {
        [SerializeField] private int controlId;
        [SerializeField] private int id;
        [SerializeField] private UnitType unitType;
        [SerializeField] private ActionType actionType;
        [SerializeField] private string unitName;
        [SerializeField] protected int hp;
        [SerializeField] protected int cost;
        [SerializeField] protected int attack;
        [SerializeField] protected int defence;
        [SerializeField] protected float speed;
        [SerializeField] public    int MovePoint = 3;
        [SerializeField] protected float attackSpeed;
        [SerializeField] protected float searchRange;
        [SerializeField] protected float attackRange;
        [SerializeField] protected float coolTime;

        [SerializeField] private Transform target;

        [SerializeField] protected Animator animator;

        private Vector3 homePosition;
        private Vector3 targetPosition;
        private Action presetAction;
        private Action currentAction;
        private Action<int> searchAction;
        private Queue<int> targetQueue = new Queue<int>();

        public int CurrentX;

        public int CurrentY;

        public bool IsMoved;

        private float currentCoolTime;

        public int ControlId {
            get { return controlId; }
            set { controlId = value; }
        }

        public UnitType CurrentUnitType
        {
            get { return unitType; }
            set { unitType = value; }
        }

        public ActionType CurrentActionType
        {
            get { return actionType; }
        }

        public float MaxRenge
        {
            get { return PrepareRange() ; }
        }

        public int AttackRange
        {
            get { return (int)attackRange; }
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
            actionType = ActionType.wait;
        }

        public void SetMoveTarget(Transform targetTrans)
        {
            target = targetTrans;
        }

        public void AddTarget(int id)
        {
            
        }

        public void RemoveTarget(int id)
        {
           
        }

        private float PrepareRange()
        {
            if (searchRange >= attackRange)
            {
                return searchRange;
            }
            return attackRange;
        }

        // Use this for initialization
        public void InitializeUnit(Action<int> action)
        {
            targetPosition = target.localPosition;
            homePosition = transform.localPosition;
            presetAction = Move;
            searchAction = action;
        }

        
        // Update is called once per frame
        public void UpdateAvator()
        {
            PresetAction();
            currentAction?.Invoke();

            UpdateCoolTime();

        }

        private void Update()
        {
            
        }

        private void UpdateCoolTime()
        {
            if(actionType == ActionType.wait || actionType == ActionType.attack)
            {
                currentCoolTime += Time.deltaTime;
            }

            if(currentCoolTime >= coolTime)
            {
                currentCoolTime = coolTime;
            }
        }

        private void ResetCoolTime()
        {
            currentCoolTime = 0;
        }

        private void MovePosition()
        {
            var step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(homePosition, targetPosition, step);
            homePosition = transform.localPosition;
        }

        private void Search()
        {
            searchAction?.Invoke(controlId);
        }

        protected virtual void Attack()
        {

        }

        protected virtual void Defeat()
        {

        }

        protected virtual void Wait()
        {
            if (currentCoolTime >= coolTime)
            {
                
            }
        }

        protected virtual void Move()
        {
            MovePosition();
        }

        protected virtual void PresetAction()
        {
            presetAction?.Invoke();
        }


        protected virtual void PresetAttack()
        {
            if (currentCoolTime >= coolTime)
            {
                actionType = ActionType.attack;
                SetAnimator(actionType);
                SetAnimatorTrigger(actionType);
                presetAction = null;
            }
        }

        protected virtual void PresetDefeat()
        {
            actionType = ActionType.defeat;
            SetAnimator(actionType);
            presetAction = null;
        }

        protected virtual void PresetWait()
        {
            actionType = ActionType.wait;
            SetAnimator(actionType);
            currentAction = Wait;
            presetAction = null;

        }

        protected virtual void PresetMove()
        {
            actionType = ActionType.move;
            MovePosition();
            SetAnimator(actionType);
            presetAction = null;

        }

        private void SetAnimator(ActionType type)
        {
            switch (type)
            {
                case ActionType.move:
                    {
                        animator.SetBool("Bool_Die_01", false);
                        animator.SetBool("Bool_Run_01", true);
                    }
                    break;
                case ActionType.defeat:
                    {
                        animator.SetBool("Bool_Die_01", true);
                        animator.SetBool("Bool_Run_01", false);
                    }
                    break;
                default:
                    {
                        animator.SetBool("Bool_Die_01", false);
                        animator.SetBool("Bool_Run_01", false);
                    }
                    break;

            }
        }

        private void SetAnimatorTrigger(ActionType type)
        {
            animator.SetTrigger("Trigger_Attack_01");
        }
       

    }
}