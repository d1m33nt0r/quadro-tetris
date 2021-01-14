using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Sprite sprite;
    private SpriteRenderer spr;
    public State state;
    private Vector2 cellIndex;
    public string type;
    private BoxCollider2D collider;
    private Map map;

    private void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = sprite;
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        map = GameObject.Find("Map").GetComponent<Map>();
    }

    public void ChangePosition(Vector3 pos, Vector2 index)
    {
        transform.position = pos;
        cellIndex = index;
        state = State.Idle;
    }

    public void CheckCollision()
    {
        map.DisableColliderCell(cellIndex);
        RaycastHit2D down = Physics2D.Raycast(transform.position, Vector2.down, Mathf.Infinity);
        RaycastHit2D left = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Infinity);
        RaycastHit2D right = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity);
        RaycastHit2D up = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity);
        map.EnableColliderCell(cellIndex);
        StartCoroutine(CheckCollisionAndDestroyBlocks(down, left, right, up));
    }

    private IEnumerator CheckCollisionAndDestroyBlocks(RaycastHit2D down, RaycastHit2D left, RaycastHit2D right, RaycastHit2D up)
    {
        //GameObject.Find("GameManager").GetComponent<GameManager>().iterationLength = 1f;
        yield return new WaitForSeconds(0.5f);
        
        if (down.collider != null)
        {
            if (down.collider.GetComponent<Cell>().state == Cell.State.Assigned)
            {
                if (map.GetBlockByCellIndex(down.collider.GetComponent<Cell>().GetCellIndex).GetComponent<Block>().type == type)
                {
                    map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, down.collider.GetComponent<Cell>().GetCellIndex);
                    map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, cellIndex);
                    map.cells[(int)down.collider.GetComponent<Cell>().GetCellIndex.x, (int)down.collider.GetComponent<Cell>().GetCellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    map.cells[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                }
            }
        }

        if (left.collider != null)
        {
            if (left.collider.GetComponent<Cell>().state == Cell.State.Assigned)
            {
                if (map.GetBlockByCellIndex(left.collider.GetComponent<Cell>().GetCellIndex).GetComponent<Block>().type == type)
                {
                    if(map.blocks[(int)left.collider.GetComponent<Cell>().GetCellIndex.x, (int)left.collider.GetComponent<Cell>().GetCellIndex.y].GetComponent<Block>().state == State.Fallen)
                    {
                        map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, left.collider.GetComponent<Cell>().GetCellIndex);
                        map.cells[(int)left.collider.GetComponent<Cell>().GetCellIndex.x, (int)left.collider.GetComponent<Cell>().GetCellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                        map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, cellIndex);
                        map.cells[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    }
                }
            }
        }

        if (right.collider != null)
        {
            if (right.collider.GetComponent<Cell>().state == Cell.State.Assigned)
            {
                if (map.GetBlockByCellIndex(right.collider.GetComponent<Cell>().GetCellIndex).GetComponent<Block>().type == type)
                {
                    if (map.blocks[(int)right.collider.GetComponent<Cell>().GetCellIndex.x, (int)right.collider.GetComponent<Cell>().GetCellIndex.y].GetComponent<Block>().state == State.Fallen)
                    {
                        map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, right.collider.GetComponent<Cell>().GetCellIndex);
                        map.cells[(int)right.collider.GetComponent<Cell>().GetCellIndex.x, (int)right.collider.GetComponent<Cell>().GetCellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                        map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, cellIndex);
                        map.cells[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    }  
                }
            }
        }

        if (up.collider != null)
        {
            if (up.collider.GetComponent<Cell>().state == Cell.State.Assigned)
            {
                if (map.GetBlockByCellIndex(up.collider.GetComponent<Cell>().GetCellIndex).GetComponent<Block>().type == type)
                {
                    map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, up.collider.GetComponent<Cell>().GetCellIndex);
                    map.PressedCellEvent(gameObject, Vector3.zero, Cell.TypeEvent.Remove, cellIndex);
                    map.cells[(int)up.collider.GetComponent<Cell>().GetCellIndex.x, (int)up.collider.GetComponent<Cell>().GetCellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    map.cells[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    
                }
            }
        }
    }

    private void OnDestroy()
    {
        UnSubscribeInputController();
        if(GameManager.state == GameManager.Statee.Game)
        {
            GameObject.Find("Score").GetComponent<Score>().AddToScore(1);
        }
    }

    public void SubscribeInputController()
    {
        InputController.SwipeEvent += CheckSwipe;
    }

    public void UnSubscribeInputController()
    {
        InputController.SwipeEvent -= CheckSwipe;
    }

    private void CheckSwipe(InputController.SwipeType type)
    {

        map.MoveBlock(type, cellIndex);     
    }

    public enum State
    {
        Idle,
        Moving,
        Fallen
    }
}