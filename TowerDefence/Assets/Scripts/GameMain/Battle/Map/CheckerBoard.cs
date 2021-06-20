using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Battle.Map
{

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

        private void Setup()
        {

        }

        /// <summary>
        /// �w����W����e�}�X�܂ŁA�ړ��R�X�g�����ōs���邩���v�Z���܂�
        /// </summary>
        /// <returns>The move amount to cells.</returns>
        /// <param name="from">From.</param>
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
        /// �w��ʒu�܂ł̈ړ����[�g�ƈړ��R�X�g��Ԃ��܂�
        /// </summary>
        /// <returns>The route coordinates and move amount.</returns>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        public List<CoordinateAndValue> CalcurateRouteCoordinatesAndMoveAmount(CheckerSquare from, CheckerSquare to)
        {
            var costs = GetMoveCostToAllSquares(from);
            if (!costs.Any(info => info.coordinate.x == to.X && info.coordinate.y == to.Y))
            {
                throw new ArgumentException(string.Format("x:{0}, y:{1} is not movable.", to.X, to.Y));
            }

            var toCost = costs.First(info => info.coordinate.x == to.X && info.coordinate.y == to.Y);
            var route = new List<CoordinateAndValue>();
            route.Add(toCost);
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
        /// �C�ӂ̃}�X���擾���܂�
        /// </summary>
        /// <returns>The cell.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public CheckerSquare GetSquare(int x, int y)
        {
            return checkerSquares.First(c => c.X == x && c.Y == y);
        }


        /// <summary>
        /// �ڕW�̃}�X�܂ł̋������擾���܂�
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
        /// �ړ��\�ȃ}�X���n�C���C�g���܂�
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="moveAmount">Move amount.</param>
        public void HighlightMovableSquares(int x, int y, int moveAmount)
        {
            var startSquare = checkerSquares.First(c => c.X == x && c.Y == y);
            foreach (var info in GetRemainingMoveAmountInfos(startSquare, moveAmount))
            {
                var square = checkerSquares.First(c => c.X == info.coordinate.x && c.Y == info.coordinate.y);
                var unit = unitManager.GetUnit(square.X, square.Y);
                if (null == unit)
                {
                    checkerSquares.First(c => c.X == info.coordinate.x && c.Y == info.coordinate.y).IsMovable = true;
                }
            }
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
        /// �}�X�̃n�C���C�g�������܂�
        /// </summary>
        public void ClearHighlight()
        {
            foreach (var square in checkerSquares)
            {
                if (square.IsAttackable)
                {

                    var unit = unitManager.GetUnit(square.X, square.Y);
                    unit.GetComponent<Button>().interactable = false;
                }
                square.IsMovable = false;
            }
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
            var startSquare = checkerSquares.First(c => c.X == x && c.Y == y);
            var infos = GetRemainingMoveAmountInfos(startSquare, moveAmount);
            if (!infos.Any(info => info.coordinate.x == endSquare.X && info.coordinate.y == endSquare.Y))
            {
                throw new ArgumentException(string.Format("endCell(x:{0}, y:{1}) is not movable.", endSquare.X, endSquare.Y));
            }

            var routeSquares = new List<CheckerSquare>();
            routeSquares.Add(endSquare);
            while (true)
            {
                var currentSquareInfo = infos.First(info => info.coordinate.x == routeSquares[routeSquares.Count - 1].X && info.coordinate.y == routeSquares[routeSquares.Count - 1].Y);
                var currentSquare = checkerSquares.First(cell => cell.X == currentSquareInfo.coordinate.x && cell.Y == currentSquareInfo.coordinate.y);
                var previousMoveAmount = currentSquareInfo.value + currentSquare.Cost;
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
        /// �w����W�Ƀ��j�b�g��z�u���܂�
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="unitPrefab">Unit prefab.</param>
        public void PutUnit(int x, int y, Unit.BaseAvator unitPrefab, Unit.UnitType team)
        {
            var unit = Instantiate(unitPrefab);
            unit.CurrentUnitType = team;
            switch (unit.CurrentUnitType)
            {
                case Unit.UnitType.enemy:
                    // �G���j�b�g�͂�����ƐF��ς��Ĕ��]
                    var image = unit.GetComponent<Image>();
                    image.color = new Color(1f, 0.7f, 0.7f);
                    var scale = image.transform.localScale;
                    scale.x *= -1f;
                    image.transform.localScale = scale;
                    break;
            }
            //unit.gameObject.SetActive(true);
            //unit.transform.SetParent(unitContainer);
            //unit.transform.position = checkerSquares.First(c => c.X == x && c.Y == y).transform.position;
            //unit.x = x;
            //unit.y = y;
        }

        /// <summary>
        /// ���j�b�g��Ώۂ̃}�X�Ɉړ������܂�
        /// </summary>
        /// <param name="square">Cell.</param>
        public void MoveTo(Unit.BaseAvator unit, CheckerSquare square)
        {
            ClearHighlight();
            var routeCells = CalculateRouteCells(unit.CurrentX, unit.CurrentY, unit.MovePoint, square);
            var sequence = DOTween.Sequence();
            for (var i = 1; i < routeCells.Length; i++)
            {
                var routeCell = routeCells[i];
                sequence.Append(unit.transform.DOMove(routeCell.transform.position, 0.1f).SetEase(Ease.Linear));
            }
            sequence.OnComplete(() =>
            {
                unit.CurrentX = routeCells[routeCells.Length - 1].X;
                unit.CurrentY = routeCells[routeCells.Length - 1].Y;
                // �U���\�͈͂̃`�F�b�N
                int attackRangeMin = 1;

                var isAttackable = HighlightAttackableCells(unit.CurrentX, unit.CurrentY, attackRangeMin, unit.AttackRange);
                if (!isAttackable)
                {
                    unit.IsMoved = true;
                }
            });
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
                        var targetSquare = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetSquare ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // �}�b�v�ɑ��݂��Ȃ��A�܂��͊��Ɍv�Z�ς݂̍��W�̓X���[
                            continue;
                        }
                        var remainingMoveAmount = i - targetSquare.Cost;
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