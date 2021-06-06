using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
