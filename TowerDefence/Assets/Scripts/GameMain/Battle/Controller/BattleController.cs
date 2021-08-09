using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battle.Unit;
using Battle.Map;

namespace Battle.Controller
{
    /// <summary>
    /// バトルの制御部分
    /// </summary>
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private Tower playerTower;
        [SerializeField] private Tower enemyTower;

        [SerializeField] private UnitManager unitManager = new UnitManager();
        [SerializeField] private Button summonButton;

        [SerializeField] private List<BaseAvator> playerUnit;

        [SerializeField] private List<BaseAvator> enemyUnit;

        [SerializeField] private CheckerBoard board;


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

            board.Setup(unitManager);

            unitManager.SetBoard(board);

            unitManager.InitializeUnit();

        }

        // Update is called once per frame
        void Update()
        {
            unitManager.UpdateUnit();
        }

        /// <summary>
        /// ダメージ更新
        /// </summary>
        private void UpdateDamages()
        {
            playerTower.UpdateDamage(0);
            enemyTower.UpdateDamage(0);

        }

        /// <summary>
        /// 死亡チェック
        /// </summary>
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

        /// <summary>
        /// 勝利判定
        /// </summary>
        private void Win()
        {

        }

        /// <summary>
        /// 敗北判定
        /// </summary>
        private void Lose()
        {

        }
    }
}