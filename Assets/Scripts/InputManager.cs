using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IOnStart
{
    public static InputManager Instance { get; private set; }

    [SerializeField] bool canClick = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void OnStart()
    {
        StartCoroutine(EnableClick());
    }
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

    IEnumerator EnableClick()
    {
        yield return new WaitForSeconds(3f);

        canClick = true;
    }
}
