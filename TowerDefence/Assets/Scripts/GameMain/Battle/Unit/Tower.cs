using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Unit
{
    public class Tower : BaseAvator
    {
       
        public bool IsDead { get { return hp <= 0; } }


        public void UpdateDamage(int damage)
        {
            hp -= damage;
        }
    }
}