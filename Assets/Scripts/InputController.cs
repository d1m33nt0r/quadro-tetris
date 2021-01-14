using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public delegate void OnSwipeInput(SwipeType type);
    public static event OnSwipeInput SwipeEvent;

    [Range(0, 2)]
    [SerializeField] private float horizontalSwipeLength;

    [Range(0, 3)]
    [SerializeField] private float verticalSwipeLength;

    private Vector2 tapPoint;
    private float horizontalSwipeDelta;
    private float verticalSwipeDelta;
    private float lastTimeCheck;

    public enum SwipeType
    {
        LEFT,
        RIGHT,
        DOWN
    }

    private void Start()
    {
        horizontalSwipeDelta = 0;
        verticalSwipeDelta = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastTimeCheck = Time.timeSinceLevelLoad;
            tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            //Debug.Log("Tap point: " + tapPoint.x);
            //Debug.Log("Mouse position: " + Camera.main.ScreenToWorldPoint(Input.mousePosition).x);

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(GameManager.state == GameManager.Statee.Game)
            {
                if (tapPoint.x > mousePosition.x)
                {
                    horizontalSwipeDelta = Mathf.Abs(tapPoint.x - mousePosition.x);
                    if (horizontalSwipeDelta > horizontalSwipeLength)
                    {
                        SwipeEvent(SwipeType.LEFT);
                        lastTimeCheck = Time.timeSinceLevelLoad;
                        tapPoint.x = tapPoint.x - horizontalSwipeDelta;
                        horizontalSwipeDelta = 0;
                    }

                    //Debug.Log("Left swipe delta - " + horizontalSwipeDelta);
                }
                
                if (tapPoint.x < mousePosition.x)
                {
                    horizontalSwipeDelta = Mathf.Abs(tapPoint.x - mousePosition.x);
                    if (horizontalSwipeDelta > horizontalSwipeLength)
                    {
                        SwipeEvent(SwipeType.RIGHT);
                        lastTimeCheck = Time.timeSinceLevelLoad;
                        tapPoint.x = tapPoint.x + horizontalSwipeDelta;
                        horizontalSwipeDelta = 0;
                    }

                    //Debug.Log("Right swipe delta - " + horizontalSwipeDelta);
                }
                
                if(tapPoint.y > mousePosition.y)
                {
                    verticalSwipeDelta = Mathf.Abs(tapPoint.y - mousePosition.y);
                    if(verticalSwipeDelta > verticalSwipeLength)
                    {
                        SwipeEvent(SwipeType.DOWN);
                        lastTimeCheck = Time.timeSinceLevelLoad;
                        tapPoint.y = tapPoint.y - verticalSwipeDelta;
                        verticalSwipeDelta = 0;
                    }
                }
            }

            if (Time.timeSinceLevelLoad - lastTimeCheck >= 2f)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().SwitchState();
                lastTimeCheck = Time.timeSinceLevelLoad;
            }

        }
    }
}