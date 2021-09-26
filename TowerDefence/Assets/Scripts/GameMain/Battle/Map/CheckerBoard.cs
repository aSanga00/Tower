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
    /// マップ情報
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
        /// マップ情報の初期化
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
        /// マップ情報セットアップ
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
        /// 指定座標から各マスまでの移動コストの算出
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
                    // 四方のマスの座標配列を作成
                    var calcTargetCoordinate = calcTargetInfo.coordinate;
                    var aroundCellCoordinates = new Coordinate[]
                    {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y + 1),
                    };
                    // 四方のマスの残移動力を計算
                    foreach (var aroundCellCoordinate in aroundCellCoordinates)
                    {
                        var targetCell = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetCell ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // マップに存在しない、または既に計算済みの座標はスルー
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
        /// 指定位置までの移動ルートと移動コストを計算して返す
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public List<CoordinateAndValue> CalcurateRouteCoordinatesAndMoveAmount(CheckerSquare from, CheckerSquare to)
        {
            //コスト算出
            var costs = GetMoveCostToAllSquares(from);
            if (!costs.Any(info => info.coordinate.x == to.X && info.coordinate.y == to.Y))
            {
                //移動不可
                throw new ArgumentException(string.Format("x:{0}, y:{1} is not movable.", to.X, to.Y));
            }
            //ゴール地点の登録
            var toCost = costs.First(info => info.coordinate.x == to.X && info.coordinate.y == to.Y);
            var route = new List<CoordinateAndValue>();
            route.Add(toCost);
            //スタート地点までのルート検索
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
        /// 指定マスの座標を取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public CheckerSquare GetSquare(int x, int y)
        {
            return checkerSquares.First(c => c.X == x && c.Y == y);
        }


        /// <summary>
        /// 指定マスまでの距離を算出
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
        /// 攻撃可能なマスをハイライトします
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
        /// 移動経路となるマスを返します
        /// </summary>
        /// <returns>The route cells.</returns>
        /// <param name="startCell">Start cell.</param>
        /// <param name="moveAmount">Move amount.</param>
        /// <param name="endSquare">End cell.</param>
        public CheckerSquare[] CalculateRouteCells(int x, int y, int moveAmount, CheckerSquare endSquare)
        {
            //スタート位置の取得
            var startSquare = checkerSquares.First(c => c.X == x && c.Y == y);
            //移動可能範囲の計算
            var infos = GetRemainingMoveAmountInfos(startSquare, moveAmount);
            //ゴールの移動可能判定
            if (!infos.Any(info => info.coordinate.x == endSquare.X && info.coordinate.y == endSquare.Y))
            {
                throw new ArgumentException(string.Format("endCell(x:{0}, y:{1}) is not movable.", endSquare.X, endSquare.Y));
            }

            var routeSquares = new List<CheckerSquare>();
            //ゴール地点の登録
            routeSquares.Add(endSquare);
            while (true)//この中のルート検索部分がうまく動いてない模様 7/27
            {
                //移動範囲内とルートの最新で一致するものを取り出す
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
        /// 目標に最も近いマスまでのルートを返します
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="target"></param>
        public CheckerSquare[] GetRouteCells(BaseAvator unit, BaseAvator target)
        {
            var square = GetNearSquare(unit,target);

            return CalculateRouteCells(unit.CurrentX, unit.CurrentY, unit.MovePoint, square);
         }
        
        /// <summary>
        /// 目標位置の周囲でもっとも近いマスを取得
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
        /// 対象ユニットに攻撃します
        /// </summary>
        /// <param name="fromUnit">From unit.</param>
        /// <param name="toUnit">To unit.</param>
        public void AttackTo(Unit.BaseAvator fromUnit, Unit.BaseAvator toUnit)
        {
           
        }

        /// <summary>
        /// 移動力を元に移動可能範囲の計算を行います
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
                foreach (var calcTargetInfo in infos.Where(info => info.value == i)) //コスト計算の部分が不完全なので修正を行う　7/26
                {
                    // 四方のマスの座標配列を作成
                    var calcTargetCoordinate = calcTargetInfo.coordinate;
                    var aroundCellCoordinates = new Coordinate[]
                    {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y + 1),
                    };
                    // 四方のマスの残移動力を計算
                    foreach (var aroundCellCoordinate in aroundCellCoordinates)
                    {
                        var targetSquare = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetSquare ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // マップに存在しない、または既に計算済みの座標はスルー
                            continue;
                        }
                        var remainingMoveAmount = i - 1;
                        appendInfos.Add(new CoordinateAndValue(aroundCellCoordinate.x, aroundCellCoordinate.y, remainingMoveAmount));
                    }
                }
                infos.AddRange(appendInfos);
            }
            // 残移動力が0以上（移動可能）なマスの情報だけを返す
            return infos.Where(x => x.value >= 0).ToArray();
        }

        /// <summary>
        /// 攻撃可能範囲の計算を行います
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
                    // 四方のマスの座標配列を作成
                    var calcTargetCoordinate = calcTargetInfo.coordinate;
                    var aroundCellCoordinates = new Coordinate[]
                    {
                    new Coordinate(calcTargetCoordinate.x - 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x + 1, calcTargetCoordinate.y),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y - 1),
                    new Coordinate(calcTargetCoordinate.x, calcTargetCoordinate.y + 1),
                    };
                    // 四方のマスの残攻撃範囲を計算
                    foreach (var aroundCellCoordinate in aroundCellCoordinates)
                    {
                        var targetSquare = checkerSquares.FirstOrDefault(c => c.X == aroundCellCoordinate.x && c.Y == aroundCellCoordinate.y);
                        if (null == targetSquare ||
                            infos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y) ||
                            appendInfos.Any(info => info.coordinate.x == aroundCellCoordinate.x && info.coordinate.y == aroundCellCoordinate.y))
                        {
                            // マップに存在しない、または既に計算済みの座標はスルー
                            continue;
                        }
                        var remainingMoveAmount = i - 1;
                        appendInfos.Add(new CoordinateAndValue(aroundCellCoordinate.x, aroundCellCoordinate.y, remainingMoveAmount));
                    }
                }
                infos.AddRange(appendInfos);
            }
            // 攻撃範囲内のマスの情報だけを返す
            return infos.Where(x => 0 <= x.value && x.value <= (attackRangeMax - attackRangeMin)).ToArray();
        }

        /// <summary>
        /// 座標と数値情報を紐付けるためのクラス
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
        /// 座標クラス
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