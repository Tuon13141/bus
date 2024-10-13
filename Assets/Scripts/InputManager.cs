using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] bool canClick = true;
    void Update()
    {
        if(!canClick) return;
        if (Input.GetMouseButtonDown(0))
        {   
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Car car = hit.transform.GetComponent<Car>();
                if (car != null)
                {
                    canClick = false;
                    car.Clicked(AvailableCanClick); 
                }
            }
        }
    }

    public void AvailableCanClick()
    {
        //Debug.Log(1);
        canClick = true;
    }
}
