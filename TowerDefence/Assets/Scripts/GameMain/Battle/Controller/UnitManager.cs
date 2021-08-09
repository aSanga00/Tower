using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Battle.Unit;
using Battle.Map;

namespace Battle.Controller
{
    /// <summary>
    /// ユニット管理
    /// </summary>
    public class UnitManager
    {
        private int unitNums;

        private List<BaseAvator> unitList;

        private BaseAvator playerTower;

        private BaseAvator enemyTower;

        private CheckerBoard board;

        public UnitManager()
        {
            unitList = new List<Unit.BaseAvator>();
            unitNums = 0;
        }

        public int GetUnitNums()
        {
            return unitNums;
        }

        public void SetBoard(CheckerBoard checkerBoard)
        {
            board = checkerBoard;
        }

        /// <summary>
        /// ユニット追加
        /// </summary>
        /// <param name="unit"></param>
        public void AddUnit(Unit.BaseAvator unit)
        {
            unit.ControlId = unitNums;
            if(unit.CurrentUnitType == Unit.UnitType.playerTower)
            {
                playerTower = unit;
            }
            else if(unit.CurrentUnitType == Unit.UnitType.enemyTower)
            {
                enemyTower = unit;
            }
            unitList.Add(unit);
            unitNums++;
        }

        /// <summary>
        /// ユニット削除
        /// </summary>
        /// <param name="id"></param>
        public void RemoveUnit(int id)
        {
            for(int i = 0; i < unitList.Count;i++)
            {
                if(unitList[i].ControlId == id)
                {
                    unitList.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// ユニットの初期化
        /// </summary>
        public void InitializeUnit()
        {
                SearchUnit();
        }

        /// <summary>
        /// ユニット検索
        /// </summary>
        /// <param name="id"></param>
        public void SearchUnit()
        {
            for (int i = 0; i < unitList.Count; i++)
            {
                if(unitList[i].CurrentUnitType == UnitType.playerTower || unitList[i].CurrentUnitType == UnitType.enemyTower)
                {
                    continue;
                }

                var target = SearchTarget(unitList[i]);

                var route = board.GetRouteCells(unitList[i],target);

                unitList[i].SetMoveRouteQueue(route);
            }
        }

        /// <summary>
        /// 目標検索
        /// </summary>
        /// <param name="controlId"></param>
        /// <param name="baseTrans"></param>
        /// <param name="range"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private BaseAvator SearchTarget(BaseAvator baseUnit)
        {
            BaseAvator target = null;

            var targetDist = baseUnit.MaxRenge;

            var basePosX = baseUnit.CurrentX;
            var basePosY = baseUnit.CurrentY;

            foreach (Unit.BaseAvator unit in unitList)
            {
                if (baseUnit.ControlId == unit.ControlId || unit.CurrentUnitType == baseUnit.CurrentUnitType)
                {
                    continue;
                }

                if(unit.CurrentUnitType == UnitType.playerTower && baseUnit.CurrentUnitType == UnitType.player)
                {
                    continue;
                }

                if (unit.CurrentUnitType == UnitType.enemyTower && baseUnit.CurrentUnitType == UnitType.enemy)
                {
                    continue;
                }

                var pos = unit.transform.localPosition;
                var distX = unit.CurrentX - basePosX;
                var distY = unit.CurrentY - basePosY;
                var distance = Math.Abs(distX)+ Math.Abs(distY);

                if(targetDist > distance)
                {
                    target = unit;
                    targetDist = distance;
                }
            }

            if(target == null)
            {
               target = GetTower(baseUnit.CurrentUnitType);
            }

            return target;
        }

        /// <summary>
        /// タワーのデータ取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private BaseAvator GetTower(Unit.UnitType type)
        {
            switch(type)
            {
                case Unit.UnitType.player:
                    {
                        return enemyTower;
                    }
                case Unit.UnitType.enemy:
                    {
                        return playerTower;
                    }
                default:
                    return playerTower;
            }
        }

        /// <summary>
        /// ユニットの更新
        /// </summary>
        public void UpdateUnit()
        {
            foreach (Unit.BaseAvator unit in unitList)
            {
                unit.UpdateAvator();
            }
        }

        /// <summary>
        /// ユニットの取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public BaseAvator GetUnit(int x, int y)
        {
            foreach (BaseAvator unit in unitList)
            {
                if(unit.CurrentX == x && unit.CurrentY == y)
                {
                    return unit;
                }
            }

            return null;
        }

        public BaseAvator GetUnitFromUnitNum(int num)
        {
           return unitList[num];
        }
    }
}