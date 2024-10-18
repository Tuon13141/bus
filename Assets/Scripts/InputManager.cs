using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour, IOnStart
{
    public static InputManager Instance { get; private set; }

    [SerializeField] bool canClickonCar = false;
    public bool CanClick { get; set; } = true;
    [SerializeField] float swipeSensitivity = 0.5f;
    [SerializeField] float minSwipeDistance = 50f;
    //[SerializeField] private float smoothTime = 0.1f;
    private Vector2 startTouchPosition, endTouchPosition;
    private bool isSwiping = false;
    private bool isSwipeDetected = false;

    public float XLimitMin { get; set; }
    public float XLimitMax { get; set; } 
    public float ZLimitMin { get; set; } 
    public float ZLimitMax { get; set; }
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
        if(!CanClick) return;
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            isSwiping = true;
            isSwipeDetected = false;
        }

        if (Input.GetMouseButton(0) && isSwiping)
        {
            endTouchPosition = Input.mousePosition;
            Vector2 swipeDistance = endTouchPosition - startTouchPosition;

            if (swipeDistance.magnitude > minSwipeDistance)
            {
                isSwipeDetected = true;
                MoveCamera(swipeDistance);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;

            if (!isSwipeDetected)
            {
                HandleClick();
            }
        }
    }


    void MoveCamera(Vector2 swipeDirection)
    {
        float moveX = Mathf.Ceil(-swipeDirection.x * swipeSensitivity / 10) * 10;
        float moveZ = Mathf.Ceil(-swipeDirection.y * swipeSensitivity / 10) * 10;

        Vector3 movement = new Vector3(moveX, 0, moveZ) * Time.deltaTime;

        Vector3 newCameraPosition = Camera.main.transform.position + movement;

        if (newCameraPosition.x >= XLimitMin && newCameraPosition.x <= XLimitMax &&
        newCameraPosition.z >= ZLimitMin && newCameraPosition.z <= ZLimitMax)
        {
            Camera.main.transform.Translate(movement, Space.World);
        }

        startTouchPosition = Input.mousePosition;
    }

    void HandleClick()
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

    public void AvailableCanClickOnCar()
    {
        canClickonCar = true;
    }

    public void SetCanClickOnCar(bool canClickonCar)
    {
        this.canClickonCar = canClickonCar;
    }
    IEnumerator EnableClickOnCar()
    {
        yield return new WaitForSeconds(2f);
        canClickonCar = true;
    }
}
