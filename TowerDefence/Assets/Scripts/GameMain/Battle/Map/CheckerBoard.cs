using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Battle.Unit;

namespace Battle.Map
{

    /// <summary>
    /// �}�b�v���
    /// </summary>
    public class CheckerBoard : MonoBehaviour
    {
        [SerializeField] private int Xsize;

        [SerializeField] private int Ysize;

        [SerializeField]
        private List<CheckerSquare> checkerSquares = new List<CheckerSquare>();

        [SerializeField]
        private Controller.UnitManager unitManager;

        private CheckerSquare[] tempSquares;

        private Unit.UnitType currentTeam;

        /// <summary>
        /// �}�b�v���̏�����
        /// </summary>
        public void InitializeBoard()
        {
            tempSquares = gameObject.GetComponentsInChildren<CheckerSquare>();

            var tempSize = 0;

            for (int i = 0; i < Xsize; i++)
            {
                for (int j = 0; j < Ysize; j++)
                {
                    tempSquares[tempSize].SetupSquare(tempSize, i, j);
                    checkerSquares.Add(tempSquares[tempSize]);
                    tempSize++;
                }
            }
        }

        /// <summary>
        /// �}�b�v���Z�b�g�A�b�v
        /// </summary>
        public void Setup(Controller.UnitManager manager)
        {
            unitManager = manager;

            var nums = unitManager.GetUnitNums();

            for(int i =0; i< nums; i++)
            {
                var unit= unitManager.GetUnitFromUnitNum(i);

                UpdateSquarePossesion(unit.ControlId, unit.CurrentX, unit.CurrentY);
                unit.SetMoveAction(UpdateSquarePossesion);
            }
        }

        public void UpdateSquarePossesion(int id, int x, int  y)
        {
            var resetFlag = false;
            var setFlag = false;
            foreach (var square in checkerSquares)
            {
                if(square.ResetUnitData(id, x, y))
                {
                    resetFlag = true;
                }

                if (square.SetupUnitData(id, x, y))
                {
                    setFlag = true;
                }

                if(resetFlag && setFlag)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// �w����W����e�}�X�܂ł̈ړ��R�X�g�̎Z�o
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public List<CoordinateAndValue> GetMoveCostToAllSquares(CheckerSquare from)
        {
            var infos = new List<CoordinateAndValue>();
            infos.Add(new CoordinateAndValue(from.X, from.Y, 0));
            var i = 0;
            while (true)
            {
                var appendInfos = new List<CoordinateAndValue>();
                foreach (var calcTargetInfo in infos.Where(info => info.value == i))
                {
                    // �l���̃}�X�̍��W�z����쐬
                    var calcTargetCoordinate = calcTargetInfo.coordinate;
                    var aroundCellCoordinates = new Coordinate[]
                    {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y + 1),
                    };
                    // �l���̃}�X�̎c�ړ��͂��v�Z
                    foreach (var aroundCellCoordinate in aroundCellCoordinates)
                    {
                        var targetCell = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetCell ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // �}�b�v�ɑ��݂��Ȃ��A�܂��͊��Ɍv�Z�ς݂̍��W�̓X���[
                            continue;
                        }
                        var remainingMoveAmount = i + targetCell.Cost;
                        appendInfos.Add(new CoordinateAndValue(aroundCellCoordinate.x, aroundCellCoordinate.y, remainingMoveAmount));
                    }
                }
                infos.AddRange(appendInfos);

                i++;
                if (i > infos.Max(x => x.value < 999 ? x.value : 0))
                {
                    break;
                }
            }
            return infos.Where(x => x.value < 999).ToList();
        }

        /// <summary>
        /// �w��ʒu�܂ł̈ړ����[�g�ƈړ��R�X�g���v�Z���ĕԂ�
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<CoordinateAndValue> CalcurateRouteCoordinatesAndMoveAmount(CheckerSquare from, CheckerSquare to)
        {
            //�R�X�g�Z�o
            var costs = GetMoveCostToAllSquares(from);
            if (!costs.Any(info => info.coordinate.x == to.X && info.coordinate.y == to.Y))
            {
                //�ړ��s��
                throw new ArgumentException(string.Format("x:{0}, y:{1} is not movable.", to.X, to.Y));
            }
            //�S�[���n�_�̓o�^
            var toCost = costs.First(info => info.coordinate.x == to.X && info.coordinate.y == to.Y);
            var route = new List<CoordinateAndValue>();
            route.Add(toCost);
            //�X�^�[�g�n�_�܂ł̃��[�g����
            while (true)
            {
                var currentCost = route.Last();
                var currentCell = checkerSquares.First(cell => cell.X == currentCost.coordinate.x && cell.Y == currentCost.coordinate.y);
                var prevMoveCost = currentCost.value - currentCell.Cost;
                var previousCost = costs.FirstOrDefault(info => (Mathf.Abs(info.coordinate.x - currentCell.X) + Mathf.Abs(info.coordinate.y - currentCell.Y)) == 1 && info.value == prevMoveCost);
                if (null == previousCost)
                {
                    break;
                }
                route.Add(previousCost);
            }
            route.Reverse();
            return route.ToList();
        }

        /// <summary>
        /// �w��}�X�̍��W���擾
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public CheckerSquare GetSquare(int x, int y)
        {
            return checkerSquares.First(c => c.X == x && c.Y == y);
        }


        /// <summary>
        /// �w��}�X�܂ł̋������Z�o
        /// </summary>
        /// <param name="baseSquare"></param>
        /// <param name="distanceMin"></param>
        /// <param name="distanceMax"></param>
        /// <returns></returns>
        public CheckerSquare[] GetCellsByDistance(CheckerSquare baseSquare, int distanceMin, int distanceMax)
        {
            return checkerSquares.Where(x =>
            {
                var distance = Math.Abs(baseSquare.X - x.X) + Math.Abs(baseSquare.Y - x.Y);
                return distanceMin <= distance && distance <= distanceMax;
            }).ToArray();
        }

        /// <summary>
        /// �U���\�ȃ}�X���n�C���C�g���܂�
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="moveAmount">Move amount.</param>
        public bool HighlightAttackableCells(int x, int y, int attackRangeMin, int attackRangeMax)
        {
            var startSquare = checkerSquares.First(c => c.X == x && c.Y == y);
            var hasTarget = false;
            foreach (var square in GetCellsByDistance(startSquare, attackRangeMin, attackRangeMax))
            {
                var unit = unitManager.GetUnit(square.X, square.Y);
                if (null != unit && unit.CurrentUnitType != currentTeam)
                {
                    hasTarget = true;
                    square.IsAttackable = true;
                    unit.GetComponent<Button>().interactable = true;
                }
            }
            return hasTarget;
        }

        /// <summary>
        /// �ړ��o�H�ƂȂ�}�X��Ԃ��܂�
        /// </summary>
        /// <returns>The route cells.</returns>
        /// <param name="startCell">Start cell.</param>
        /// <param name="moveAmount">Move amount.</param>
        /// <param name="endSquare">End cell.</param>
        public CheckerSquare[] CalculateRouteCells(int x, int y, int moveAmount, CheckerSquare endSquare)
        {
            //�X�^�[�g�ʒu�̎擾
            var startSquare = checkerSquares.First(c => c.X == x && c.Y == y);
            //�ړ��\�͈͂̌v�Z
            var infos = GetRemainingMoveAmountInfos(startSquare, moveAmount);
            //�S�[���̈ړ��\����
            if (!infos.Any(info => info.coordinate.x == endSquare.X && info.coordinate.y == endSquare.Y))
            {
                throw new ArgumentException(string.Format("endCell(x:{0}, y:{1}) is not movable.", endSquare.X, endSquare.Y));
            }

            var routeSquares = new List<CheckerSquare>();
            //�S�[���n�_�̓o�^
            routeSquares.Add(endSquare);
            while (true)//���̒��̃��[�g�������������܂������ĂȂ��͗l 7/27
            {
                //�ړ��͈͓��ƃ��[�g�̍ŐV�ň�v������̂����o��
                var currentSquareInfo = infos.First(info => info.coordinate.x == routeSquares[routeSquares.Count - 1].X && info.coordinate.y == routeSquares[routeSquares.Count - 1].Y);
                //
                var currentSquare = checkerSquares.First(square => square.X == currentSquareInfo.coordinate.x && square.Y == currentSquareInfo.coordinate.y);
                var previousMoveAmount = currentSquareInfo.value + currentSquare.Cost +1;
                var previousSquareInfo = infos.FirstOrDefault(info => (Mathf.Abs(info.coordinate.x - currentSquare.X) + Mathf.Abs(info.coordinate.y - currentSquare.Y)) == 1 && info.value == previousMoveAmount);
                if (null == previousSquareInfo)
                {
                    break;
                }
                routeSquares.Add(checkerSquares.First(c => c.X == previousSquareInfo.coordinate.x && c.Y == previousSquareInfo.coordinate.y));
            }
            routeSquares.Reverse();
            return routeSquares.ToArray();
        }

        /// <summary>
        /// �ڕW�ɍł��߂��}�X�܂ł̃��[�g��Ԃ��܂�
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        public CheckerSquare[] GetRouteCells(BaseAvator unit, BaseAvator target)
        {
            var square = GetNearSquare(unit,target);

            return CalculateRouteCells(unit.CurrentX, unit.CurrentY, unit.MovePoint, square);
         }
        
        /// <summary>
        /// �ڕW�ʒu�̎��͂ł����Ƃ��߂��}�X���擾
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private CheckerSquare GetNearSquare(BaseAvator from, BaseAvator to)
        {
            var posx = to.CurrentX;
            var posy = to.CurrentY;
            var distX = posx - from.CurrentX;
            var distY = posy - from.CurrentY;

            if(Math.Abs(distX) >= Math.Abs(distY))
            {
                if(distX > 0)
                {
                    posx -= 1;
                }
                else
                {
                    posx += 1;
                }
            }
            else
            {
                if (distY > 0)
                {
                    posy -= 1;
                }
                else
                {
                    posy += 1;
                }
            }

            return GetSquare(posx,posy);
        }

        /// <summary>
        /// �Ώۃ��j�b�g�ɍU�����܂�
        /// </summary>
        /// <param name="fromUnit">From unit.</param>
        /// <param name="toUnit">To unit.</param>
        public void AttackTo(Unit.BaseAvator fromUnit, Unit.BaseAvator toUnit)
        {
           
        }

        /// <summary>
        /// �ړ��͂����Ɉړ��\�͈͂̌v�Z���s���܂�
        /// </summary>
        /// <returns>The remaining move amount infos.</returns>
        /// <param name="startSquare">Start cell.</param>
        /// <param name="moveAmount">Move amount.</param>
        CoordinateAndValue[] GetRemainingMoveAmountInfos(CheckerSquare startSquare, int moveAmount)
        {
            var infos = new List<CoordinateAndValue>();
            infos.Add(new CoordinateAndValue(startSquare.X, startSquare.Y, moveAmount));
            for (var i = moveAmount; i >= 0; i--)
            {
                var appendInfos = new List<CoordinateAndValue>();
                foreach (var calcTargetInfo in infos.Where(info => info.value == i)) //�R�X�g�v�Z�̕������s���S�Ȃ̂ŏC�����s���@7/26
                {
                    // �l���̃}�X�̍��W�z����쐬
                    var calcTargetCoordinate = calcTargetInfo.coordinate;
                    var aroundCellCoordinates = new Coordinate[]
                    {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y + 1),
                    };
                    // �l���̃}�X�̎c�ړ��͂��v�Z
                    foreach (var aroundCellCoordinate in aroundCellCoordinates)
                    {
                        var targetSquare = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetSquare ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // �}�b�v�ɑ��݂��Ȃ��A�܂��͊��Ɍv�Z�ς݂̍��W�̓X���[
                            continue;
                        }
                        var remainingMoveAmount = i - 1;
                        appendInfos.Add(new CoordinateAndValue(aroundCellCoordinate.x, aroundCellCoordinate.y, remainingMoveAmount));
                    }
                }
                infos.AddRange(appendInfos);
            }
            // �c�ړ��͂�0�ȏ�i�ړ��\�j�ȃ}�X�̏�񂾂���Ԃ�
            return infos.Where(x => x.value >= 0).ToArray();
        }

        /// <summary>
        /// �U���\�͈͂̌v�Z���s���܂�
        /// </summary>
        /// <returns>The remaining move amount infos.</returns>
        /// <param name="startSquare">Start cell.</param>
        /// <param name="moveAmount">Move amount.</param>
        CoordinateAndValue[] GetRemainingAccountRangeInfos(CheckerSquare startSquare, int attackRangeMin, int attackRangeMax)
        {
            var infos = new List<CoordinateAndValue>();
            infos.Add(new CoordinateAndValue(startSquare.X, startSquare.Y, attackRangeMax));
            for (var i = attackRangeMax; i >= 0; i--)
            {
                var appendInfos = new List<CoordinateAndValue>();
                foreach (var calcTargetInfo in infos.Where(info => info.value == i))
                {
                    // �l���̃}�X�̍��W�z����쐬
                    var calcTargetCoordinate = calcTargetInfo.coordinate;
                    var aroundCellCoordinates = new Coordinate[]
                    {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y + 1),
                    };
                    // �l���̃}�X�̎c�U���͈͂��v�Z
                    foreach (var aroundCellCoordinate in aroundCellCoordinates)
                    {
                        var targetSquare = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetSquare ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // �}�b�v�ɑ��݂��Ȃ��A�܂��͊��Ɍv�Z�ς݂̍��W�̓X���[
                            continue;
                        }
                        var remainingMoveAmount = i - 1;
                        appendInfos.Add(new CoordinateAndValue(aroundCellCoordinate.x, aroundCellCoordinate.y, remainingMoveAmount));
                    }
                }
                infos.AddRange(appendInfos);
            }
            // �U���͈͓��̃}�X�̏�񂾂���Ԃ�
            return infos.Where(x => 0 <= x.value && x.value <= (attackRangeMax - attackRangeMin)).ToArray();
        }

        /// <summary>
        /// ���W�Ɛ��l����R�t���邽�߂̃N���X
        /// </summary>
        public class CoordinateAndValue
        {
            public readonly Coordinate coordinate;
            public readonly int value;

            public CoordinateAndValue(int x, int y, int value)
            {
                this.coordinate = new Coordinate(x, y);
                this.value = value;
            }
        }

        /// <summary>
        /// ���W�N���X
        /// </summary>
        public class Coordinate
        {
            public readonly int x;
            public readonly int y;

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }

}