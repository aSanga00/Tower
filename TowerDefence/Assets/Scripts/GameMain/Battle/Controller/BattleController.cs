using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle.Unit;

namespace Battle.Controller
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private Tower playerTower;
        [SerializeField] private Tower enemyTower;

        [SerializeField] private BaseAvator playerUnit;
        [SerializeField] private BaseAvator enemyUnit01;
        [SerializeField] private BaseAvator enemyUnit02;
        [SerializeField] private BaseAvator enemyUnit03;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateDamages();
            CheckDead();
        }

        private void UpdateDamages()
        {
            playerTower.UpdateDamage(0);
            enemyTower.UpdateDamage(0);

        }

        private void CheckDead()
        {
            if(playerTower.IsDead)
            {
                Lose();
            }
            else if(enemyTower.IsDead)
            {
                Win();
            }


        }

        private void Win()
        {

        }

        private void Lose()
        {

        }
    }
}