using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battle.Unit;

namespace Battle.Controller
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private Tower playerTower;
        [SerializeField] private Tower enemyTower;

        [SerializeField] private UnitManager unitManager = new UnitManager();
        [SerializeField] private Button summonButton;

        [SerializeField] private List<BaseAvator> playerUnit;

        [SerializeField] private List<BaseAvator> enemyUnit;

        [SerializeField] private CheckerBoard board;

        [SerializeField] private BoardManager boardManager;



        // Start is called before the first frame update
        void Start()
        {

            board.InitializeBoard();

            unitManager.AddUnit(playerTower);

            unitManager.AddUnit(enemyTower);

            foreach (Unit.BaseAvator unit in playerUnit)
            {
                unitManager.AddUnit(unit);
            }

            foreach (Unit.BaseAvator unit in enemyUnit)
            {
                unitManager.AddUnit(unit);
            }

            unitManager.Search();

            unitManager.InitializeUnit();

        }

        // Update is called once per frame
        void Update()
        {
            unitManager.UpdateUnit();
           // UpdateDamages();
           // CheckDead();
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

        private void UpdateTarget()
        {

        }

        private void UpdatePlayerTarget()
        {
           
        }

        private void Win()
        {

        }

        private void Lose()
        {

        }
    }
}