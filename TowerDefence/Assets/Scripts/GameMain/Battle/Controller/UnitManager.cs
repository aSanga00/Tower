using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle.Unit;

namespace Battle.Controller
{
    public class UnitManager
    {
        private int unitNums;

        private List<Unit.BaseAvator> unitList;

        private Unit.BaseAvator playerTower;

        private Unit.BaseAvator enemyTower;

        public UnitManager()
        {
            unitList = new List<Unit.BaseAvator>();
            unitNums = 0;
        }

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
            else
            {
                unitList.Add(unit);
            }
            unitNums++;
        }

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

        public void InitializeUnit()
        {
            foreach(Unit.BaseAvator unit in unitList)
            {
                unit.InitializeUnit(SearchUnit);
            }
        }

        public void SearchUnit(int id)
        {
            for (int i = 0; i < unitList.Count; i++)
            {
                if(unitList[i].ControlId != id)
                {
                    continue;
                }

                var target = SearchTarget(unitList[i].ControlId, unitList[i].transform, unitList[i].MaxRenge, unitList[i].CurrentUnitType);

                unitList[i].SetMoveTarget(target);
            }
        }

        public void Search()
        {
            for (int i = 0; i < unitList.Count; i++)
            {
                var target = SearchTarget(unitList[i].ControlId, unitList[i].transform, unitList[i].MaxRenge, unitList[i].CurrentUnitType);

                unitList[i].SetMoveTarget(target);
            }
        }

        private Transform SearchTarget(int controlId, Transform baseTrans,float range, Unit.UnitType type)
        {
            Transform target = null;

            float targetDist = range;

            foreach (Unit.BaseAvator unit in unitList)
            {
                if (controlId == unit.ControlId || unit.CurrentUnitType == type)
                {
                    continue;
                }
                var pos = unit.transform.localPosition;
                var basepos = baseTrans.localPosition;
                float distance = Vector3.Distance(pos, basepos);

                if(targetDist > distance)
                {
                    target = unit.transform;
                    targetDist = distance;
                }
            }

            if(target == null)
            {
               target = SetTower(type);
            }

            return target;
        }


        private Transform SetTower(Unit.UnitType type)
        {
            switch(type)
            {
                case Unit.UnitType.player:
                    {
                        return enemyTower.transform;
                    }
                case Unit.UnitType.enemy:
                    {
                        return playerTower.transform;
                    }
                default:
                    return playerTower.transform;
            }
        }

        public void UpdateUnit()
        {
            foreach (Unit.BaseAvator unit in unitList)
            {
                unit.UpdateAvator();
            }
        }

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
    }
}