using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public delegate void PressedCell(GameObject original, Vector3 pos, TypeEvent typeEvent, Vector2 cellIndex);
    public static event PressedCell pressedCellEvent;

    public State GetState => state;
    public Vector2 GetCellIndex => cellIndex;

    public State state;

    private string type;
    private Vector2 cellIndex;

    private bool mousePressed;
    private float timeLeft;

    public void InitCell(float x, float y)
    {
        cellIndex = new Vector2(x, y);
    }

    void Start()
    {
        //state = State.NotAssigned;
        mousePressed = false;
    }

    private void OnMouseDown()
    {
        mousePressed = true;
    }

    private void OnMouseUp()
    {
        if(GameManager.state == GameManager.Statee.Editor)
        {
            if(GameObject.Find("EditorCanvas").GetComponent<EditorController>().GetActiveButton() != null)
            {
                if (mousePressed)
                {
                    EditorButton btn = GameObject.Find("EditorCanvas").GetComponent<EditorController>().GetActiveButton().GetComponent<EditorButton>();

                    type = btn.type == type ? "" : btn.type;

                    if (state == State.NotAssigned)
                    {
                        state = State.Assigned;
                        pressedCellEvent(btn.block, transform.position, TypeEvent.Assign, cellIndex);
                    }
                    else
                    {
                        if (type == "")
                        {

                            state = State.NotAssigned;
                            pressedCellEvent(btn.block, transform.position, TypeEvent.Remove, cellIndex);
                        }
                        else
                        {
                            state = State.Assigned;
                            pressedCellEvent(btn.block, transform.position, TypeEvent.Replace, cellIndex);
                        }
                    }

                    mousePressed = false;
                }
            }
            
        }        
    }



    public enum State
    {
        Assigned,
        NotAssigned
    }

    public enum TypeEvent
    {
        Assign,
        Remove,
        Replace
    }
}