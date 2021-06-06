using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
    [SerializeField] private int Xsize;

    [SerializeField] private int Ysize;

    [SerializeField] private CheckerSquare[,] checkerSquares;

    private CheckerSquare[] tempSquares;

    public void InitializeBoard()
    {
        tempSquares = gameObject.GetComponentsInChildren<CheckerSquare>();

        var tempSize = 0;

        checkerSquares = new CheckerSquare[Xsize,Ysize];

        for(int i = 0; i< Xsize; i++)
        {
            for(int j = 0; j < Ysize; j++)
            {
                tempSquares[tempSize].SetupSquare(tempSize,i,j);
                checkerSquares[i, j] = tempSquares[tempSize];
                tempSize++;
            }
        }
    }

    private void Setup()
    {
        
    }
}
