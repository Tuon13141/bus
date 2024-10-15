using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IOnStart
{
    public static InputManager Instance { get; private set; }

    [SerializeField] bool canClickonCar = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void OnStart()
    {
        StartCoroutine(EnableClickOnCar());
    }
    void Update()
    {
       
        if (Input.GetMouseButtonDown(0))
        {   
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Car car = hit.transform.GetComponent<Car>();
                if (car != null && canClickonCar)
                {
                    canClickonCar = false;
                    car.Clicked(AvailableCanClickOnCar); 
                }
                else
                {
                    AreaArrowButton arrowButton = hit.transform.GetComponent<AreaArrowButton>();

                    if (arrowButton != null)
                    {
                        arrowButton.OnClick();
                    }
                }
                
            }
        }
    }

    public void AvailableCanClickOnCar()
    {
        //Debug.Log(1);
        canClickonCar = true;
    }

    IEnumerator EnableClickOnCar()
    {
        yield return new WaitForSeconds(3f);

        canClickonCar = true;
    }
}
