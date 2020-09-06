using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Unit
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private int hp;

        public bool IsDead { get { return hp <= 0; } }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateDamage(int damage)
        {
            hp -= damage;
        }
    }
}