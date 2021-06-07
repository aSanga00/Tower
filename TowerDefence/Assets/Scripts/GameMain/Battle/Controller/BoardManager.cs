using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Map
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private CheckerBoard board;

        public void InitializeManager()
        {
            board.InitializeBoard();
        }

        public void SetUnitData()
        {

        }

        public Unit.BaseAvator GetUnit(int  posX, int posY)
        {
            board.GetUnit(posX,posY);

            return null;
        }

    }
}