using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();  
    }

    public void Draw(Vector2 firstPoint, Vector2 secondPoint)
    {
        Vector3 firstPosition = Camera.main.ScreenToWorldPoint(firstPoint);
        firstPosition.z = 0;
        Vector3 secondPosition = Camera.main.ScreenToWorldPoint(secondPoint);
        secondPosition.z = 0;

        lr.SetPosition(0, firstPosition);
        lr.SetPosition(1, secondPosition);
    }
}