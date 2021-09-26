using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Battle.Map;

namespace Battle.Unit
{
    public enum UnitType { player, enemy, neutral,playerTower, enemyTower};

    public enum ActionType { wait,move,attack,defeat,end };


    /// <summary>
    /// キャラのベースデータ
    /// </summary>
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
        private Action currentAction;
        private Queue<CheckerSquare> targetQueue = new Queue<CheckerSquare>();
        private Action<int> RootAction;
        private Action<int,int> DamageAction;
        private Action<int, int, int> MoveAction;
        private int targetID = -1;

        public int CurrentX;

        public int CurrentY;

        public int NextX;

        public int NextY;

        public bool IsMoved;

        private int MoveCount = 0;

        private int UpdateCount = 1;

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


        /// <summary>
        /// ユニットデータのセットアップ
        /// </summary>
        /// <param name="parameter"></param>
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

        public void SetMoveRouteQueue(CheckerSquare[] squares,int id)
        {
            targetQueue.Clear();
            foreach (var square in squares)
            {
                targetQueue.Enqueue(square);
            }
            targetID = id;
            SetMoveTarget(true);
            PresetMove();
        }

        public void SetRootAction(Action<int> action)
        {
            RootAction = null;
            RootAction = action;
        }

        public void SetDamageAction(Action<int,int> action)
        {
            DamageAction = null;
            DamageAction = action;
        }

        public void SetMoveAction(Action<int,int,int> action)
        {
            MoveAction = null;
            MoveAction = action;
        }

        /// <summary>
        /// 移動先の設定
        /// </summary>
        /// <param name="targetTrans"></param>
        public bool SetMoveTarget(bool isSetTarget = false)
        {
            if (targetQueue.Count > 0)
            {
                var square = targetQueue.Dequeue();
                CurrentX = NextX;
                CurrentY = NextY;
                NextX = square.X;
                NextY = square;
                target = square.transform;
                targetPosition = target.localPosition;
                homePosition = transform.localPosition;
                MoveCount++;
                return true;
            }
            return false;
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
        public void InitializeUnit()
        {
            if (target != null)
            {
                targetPosition = target.localPosition;
            }
            homePosition = transform.localPosition;
            currentAction = Wait;
            NextX = CurrentX;
            NextY = CurrentY;
        }

        
        // Update is called once per frame
        public void UpdateAvator()
        {
            currentAction?.Invoke();

            UpdateCoolTime();
        }

        private void Update()
        {
            
        }

        public void HitDamage(int damage)
        {
            hp -= damage;
        }

        private void UpdateCoolTime()
        {
            if(actionType == ActionType.wait || actionType == ActionType.attack)
            {
                currentCoolTime += Time.deltaTime;
            }

            if(currentCoolTime >= coolTime || actionType == ActionType.move)
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
            if(transform.position == targetPosition)
            {
                if(!SetMoveTarget())
                {
                    if (targetID < 0)
                    {
                        PresetWait();
                    }
                    else
                    {
                        PresetAttack();
                    }
                    return;
                }

                if(MoveCount > UpdateCount)
                {
                    RootAction?.Invoke(controlId);
                    MoveCount = 0;
                }

            }
            MovePosition();
        }

        protected virtual void PresetAttack()
        {
            if (currentCoolTime >= coolTime)
            {
                actionType = ActionType.attack;
                SetAnimator(actionType);
                SetAnimatorTrigger(actionType);
            }
        }

        protected virtual void PresetDefeat()
        {
            actionType = ActionType.defeat;
            SetAnimator(actionType);
        }

        protected virtual void PresetWait()
        {
            actionType = ActionType.wait;
            SetAnimator(actionType);
            currentAction = Wait;
        }

        protected virtual void PresetMove()
        {
            actionType = ActionType.move;
            currentAction = Move;
            SetAnimator(actionType);

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