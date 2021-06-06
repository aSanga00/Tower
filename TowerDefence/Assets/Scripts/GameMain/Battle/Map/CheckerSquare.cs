using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerSquare : MonoBehaviour
{
    [SerializeField] private int id;

    [SerializeField] private Vector3 position;

    [SerializeField] private int squarePosX;

    [SerializeField] private int squarePosY;

    [SerializeField] private int possessionId;

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

}
