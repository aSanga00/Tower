using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Map
{

    public class CheckerSquare : MonoBehaviour
    {
        [SerializeField] private int id;

        [SerializeField] private Vector3 position;

        [SerializeField] private int squarePosX;

        [SerializeField] private int squarePosY;

        [SerializeField] private int possessionId;

        [SerializeField] private int cost;

        bool highlight = false;

        public void SetupSquare(int setId, int x, int y)
        {
            id = setId;

            squarePosX = x;

            squarePosY = y;

            position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            possessionId = 0;
        }

        public void SetupUnitData(int controlId)
        {
            possessionId = controlId;
        }

        public void ResetId()
        {
            possessionId = 0;
        }

        /// <summary>
        /// ˆÚ“®‰Â”\‚Èƒ}ƒX‚©‚Ç‚¤‚©
        /// </summary>
        /// <value><c>true</c> if this instance is movable; otherwise, <c>false</c>.</value>
        public bool IsMovable
        {
            set
            {
                highlight= value;
            }
            get { return highlight; }
        }

        public bool IsAttackable
        {
            set
            {
                highlight = value;
            }
            get { return highlight; }
        }

        public int Cost
        {
            get { return cost; }
        }

        public int X
        {
            get { return squarePosX; }
        }

        public int Y
        {
            get { return squarePosY; }
        }

        public void OnClick()
        {
            if (IsMovable)
            {
               // map.MoveTo(map.FocusingUnit, this);
            }
        }

    }
}
