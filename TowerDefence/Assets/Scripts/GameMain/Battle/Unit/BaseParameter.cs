using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Unit
{

    public class BaseParameter : ScriptableObject
    {
        [SerializeField] public int id;
        [SerializeField] public string name;
        [SerializeField] public int hp;
        [SerializeField] public int cost;
        [SerializeField] public int attack;
        [SerializeField] public int defence;
        [SerializeField] public float speed;
        [SerializeField] public float attackSpeed;
        [SerializeField] public float searchRange;
        [SerializeField] public float attackRange;
        [SerializeField] public float coolTime;

        public BaseParameter()
        {
            id = 0;
            name = string.Empty;
            hp = 100;
            cost = 10;
            attack = 10;
            defence = 5;
            speed = 1.0f;
            attackSpeed = 2.0f;
            searchRange = 1f;
            attackRange = 1f;
            coolTime = 30f;
        }
    }
}
