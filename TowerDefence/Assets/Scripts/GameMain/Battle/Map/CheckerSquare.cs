using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Map
{

    /// <summary>
    /// マス情報
    /// </summary>
    public class CheckerSquare : MonoBehaviour
    {
        [SerializeField] private int id;

        [SerializeField] private Vector3 position;

        [SerializeField] private int squarePosX;

        [SerializeField] private int squarePosY;

        [SerializeField] private int possessionId;

        [SerializeField] private int placementId;

        [SerializeField] private int cost;

        bool highlight = false;

        /// <summary>
        /// マスのセットアップ
        /// </summary>
        /// <param name="setId"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetupSquare(int setId, int x, int y)
        {
            id = setId;

            squarePosX = x;

            squarePosY = y;

            position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            possessionId = 0;

            placementId = 0;
        }

        public bool SetupUnitData(int controlId, int x, int y)
        {
            if(squarePosX == x && squarePosY == y)
            {
                SetPlacementId(controlId);
                return true;
            }

            return false;
        }

        public bool ResetUnitData(int controlId, int x, int y)
        {
            if((squarePosX != x || squarePosY != y)&& placementId == controlId)
            {
                ResetPlacementId();
                return true;
            }
            return false;
        }


        /// <summary>
        /// ユニット情報のセットアップ
        /// </summary>
        /// <param name="controlId"></param>
        private void SetPlacementId(int controlId)
        {
            placementId = controlId;
        }

        /// <summary>
        /// 占有IDの設定
        /// </summary>
        /// <param name="controlId"></param>
        private void SetPossessionId(int controlId)
        {
            possessionId = controlId;
        }

        /// <summary>
        /// 設定したユニットIDのリセット
        /// </summary>
        public void ResetPlacementId()
        {
            placementId = 0;
        }

        /// <summary>
        /// 占有IDのリセット
        /// </summary>
        public void ResetPossessionId()
        {
            possessionId = 0;
        }

        public bool CheckPossession()
        {
            if(placementId != 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移動可能なマスか
        /// </summary>
        public bool IsMovable
        {
            set
            {
                highlight= value;
            }
            get { return highlight; }
        }

        /// <summary>
        /// 攻撃可能なマスか
        /// </summary>
        public bool IsAttackable
        {
            set
            {
                highlight = value;
            }
            get { return highlight; }
        }

        /// <summary>
        /// コスト取得
        /// </summary>
        public int Cost
        {
            get { return cost; }
        }

        /// <summary>
        /// X座標の取得
        /// </summary>
        public int X
        {
            get { return squarePosX; }
        }

        /// <summary>
        /// Y座標の取得
        /// </summary>
        public int Y
        {
            get { return squarePosY; }
        }

        /// <summary>
        /// クリックした際の挙動
        /// </summary>
        public void OnClick()
        {
            if (IsMovable)
            {
               // map.MoveTo(map.FocusingUnit, this);
            }
        }

    }
}
