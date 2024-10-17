using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour, IChangeStat
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] ColorType colorType = ColorType.Red;
    public ColorType ColorType { get { return colorType; } set { colorType = value; GetColor(); } } 
    public GridPassenger GridPassenger { get; set; }
    [SerializeField] PassengerStat passengerStat = PassengerStat.OnWait;
    public List<Vector3> MovePoints { get; set; } = new List<Vector3>();
    int currentMovePointIndex = 0;
    [SerializeField] float moveSpeed = 10f;
    bool isMoving = false; 
    
    public ExitArea ExitArea { get; set; }
    
    public void ChangeStat(PassengerStat passengerStat)
    {
        this.passengerStat = passengerStat;
        switch (passengerStat)
        {
            case PassengerStat.OnWait:

                break;
            case PassengerStat.OnPickedUp: 
                transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                break;
        }
    }
    IEnumerator MoveToPoints()
    {
        
        for (int i = currentMovePointIndex; i < MovePoints.Count; ++i)
        {
       
            Vector3 startPoint = transform.position;
            Vector3 endPoint = MovePoints[i];
            float time = 0f;

            while (Vector3.Distance(transform.position, endPoint) > 0.01f)
            {
                time += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPoint, endPoint, time);

                yield return null; 
            }

            transform.position = endPoint;
            currentMovePointIndex = i;
        }

        isMoving = false;
    }

    public void Move()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(MoveToPoints());
           
        }
        
    }

    void GetColor()
    {
        switch (colorType)
        {
            case ColorType.Red:
                SetColor(Color.red);
                break;
            case ColorType.Green:
                SetColor(Color.green);
                break;
            case ColorType.Blue:
                SetColor(Color.blue);
                break;
        }
    }

    void SetColor(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }

    public void ResetParameter(ExitArea exitArea, ColorType colorType)
    {
        this.ExitArea = exitArea;
        this.colorType = colorType;

        GetColor();
    }
}

public enum PassengerStat
{
    OnWait, OnPickedUp,
}
