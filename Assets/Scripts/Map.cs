using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Map : MonoBehaviour
{
    public float cellWidth, cellHeight;

    [SerializeField] private GameObject cell;
    [SerializeField] private GameObject line;
    public float columns = 6, rows = 10;
    [SerializeField] private float marginSides;
    public bool isMoving;
    public bool isDone;
    private List<GameObject> horizontalLines = new List<GameObject>();
    private List<GameObject> verticalLines = new List<GameObject>();
    private BlockSpawner blockSpawner;
    public GameObject[,] cells;
    public GameObject[,] blocks;
    
    void Start()
    {
        blockSpawner = GameObject.Find("BlockSpawner").GetComponent<BlockSpawner>();
        isDone = true;
        Cell.pressedCellEvent += PressedCellEvent;
        cells = new GameObject[(int)rows, (int)columns];
        blocks = new GameObject[(int)rows, (int)columns];
        
        cellWidth = (Screen.width - marginSides * 2) / columns;
        cellHeight = (Screen.height - ((Screen.height / 6) + (Screen.height / 50))) / rows;

        for (int i = 0; i <= rows; i++)
            horizontalLines.Add(Instantiate(line, transform, true));

        for (int i = 0; i <= columns; i++)
            verticalLines.Add(Instantiate(line, transform, true));

        DrawMap();

        
    }

    public void DrawMap()
    {
        float i = Screen.height - (Screen.height / 6);
        int k = 0;
        foreach(var line in horizontalLines)
        {
            Vector2 firstPoint = new Vector2(marginSides, i);
            Vector2 secondPoint = new Vector2(Screen.width - marginSides, i);
            line.GetComponent<Line>().Draw(firstPoint, secondPoint);

            if(k < horizontalLines.Count - 1)
            {
                CreateRow(k, firstPoint);
                k++;
            }

            i -= cellHeight;
        }
        
        float j = marginSides;
        foreach (var line in verticalLines)
        {
            Vector2 firstPoint = new Vector2(j, (Screen.height / 50));
            Vector2 secondPoint = new Vector2(j, Screen.height - (Screen.height / 6));
            line.GetComponent<Line>().Draw(firstPoint, secondPoint);
            j += cellWidth;
        }

        LoadEditorState();
    }

    private void CreateRow(int rowIndex, Vector2 firstPoint)
    {
        Vector2.Distance(firstPoint, Camera.main.ScreenToWorldPoint(new Vector2(firstPoint.x + cellWidth, firstPoint.y)));
        for (int n = 0; n < (int)columns; n++)
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(new Vector2(firstPoint.x + (n * cellWidth) + (cellWidth / 2), firstPoint.y - cellHeight / 2));
            position.z = 0;
            cells[rowIndex, n] = Instantiate(cell, position, Quaternion.identity);
            cells[rowIndex, n].AddComponent<Cell>().InitCell(rowIndex, n);
        }
    }

    public void PressedCellEvent(GameObject block, Vector3 pos, Cell.TypeEvent typeEvent, Vector2 cellIndex)
    {
        switch(typeEvent)
        {
            case Cell.TypeEvent.Assign:
                blocks[(int)cellIndex.x, (int)cellIndex.y] = Instantiate(block);
                blocks[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Block>().ChangePosition(pos, cellIndex);
                break;
            case Cell.TypeEvent.Remove:
                Destroy(blocks[(int)cellIndex.x, (int)cellIndex.y]);
                break;
            case Cell.TypeEvent.Replace:
                Destroy(blocks[(int)cellIndex.x, (int)cellIndex.y]);
                blocks[(int)cellIndex.x, (int)cellIndex.y] = Instantiate(block);
                blocks[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Block>().ChangePosition(pos, cellIndex);
                break;
        }
    }

    public struct RowData
    {
        public Vector2 cellIndex;
        public Cell.State state;

        public RowData(Vector2 index, Cell.State st)
        {
            cellIndex = index;
            state = st;
        }
    }

    public void DisableColliderCell(Vector2 index)
    {
        cells[(int)index.x, (int)index.y].GetComponent<BoxCollider2D>().enabled = false;
    }

    public void EnableColliderCell(Vector2 index)
    {
        cells[(int)index.x, (int)index.y].GetComponent<BoxCollider2D>().enabled = true;
    }

    public void MoveBlocks()
    {
        isMoving = true;
        for(int i = (int)rows-1; i >= 0; i--)
        {
            MoveRow(i);
        }
        
        for (int rowIndex = (int)rows - 1; rowIndex >= 0; rowIndex--)
        {
            for (int j = 0; j < (int)columns; j++)
            {
                if (cells[rowIndex, j].GetComponent<Cell>().GetState == Cell.State.Assigned)
                {
                    if (blocks[rowIndex, j].GetComponent<Block>().state != Block.State.Fallen)
                    {
                        isDone = false;
                    }
                }
            }
        }

        if (isDone)
        {
            GameObject.Find("BlockSpawner").GetComponent<BlockSpawner>().SpawnBlockInRandomPosition();
            // GameObject.Find("GameManager").GetComponent<GameManager>().iterationLength = 1f;
            Time.timeScale = 1;
        }

        isDone = true;
        isMoving = false;
    }

    public GameObject GetBlockByCellIndex(Vector2 index)
    {
        return blocks[(int)index.x, (int)index.y];
    }

    public void MoveRow(int rowIndex)
    {
        RowData[] rowInfo = new RowData[(int)columns];

        for(int j = 0; j < (int)columns; j++)
        {
            Cell.State st = cells[rowIndex, j].GetComponent<Cell>().state;
            Vector2 index = cells[rowIndex, j].GetComponent<Cell>().GetCellIndex;
            rowInfo[j] = new RowData(index, st);
        }

        for (int j = 0; j < (int)columns; j++)
        {
            if (rowInfo[j].state == Cell.State.NotAssigned)
            {
                if (rowIndex - 1 != -1)
                {
                    if (cells[rowIndex - 1, j].GetComponent<Cell>().GetState == Cell.State.Assigned)
                    {
                    
                        blocks[rowIndex - 1, j].GetComponent<Block>().ChangePosition(cells[rowIndex, j].transform.position, new Vector2(rowIndex, j));
                        blocks[rowIndex, j] = blocks[rowIndex - 1, j];
                        cells[rowIndex - 1, j].GetComponent<Cell>().state = Cell.State.NotAssigned;
                        cells[rowIndex, j].GetComponent<Cell>().state = Cell.State.Assigned;
                        //blocks[rowIndex - 1, j].GetComponent<Block>().ChangeState(Block.State.Moving);
                        isDone = false;
                        if (rowIndex == 9)
                        {
                            blocks[rowIndex, j].GetComponent<Block>().state = Block.State.Fallen;
                            blocks[rowIndex, j].GetComponent<Block>().UnSubscribeInputController();
                            blocks[rowIndex, j].GetComponent<Block>().CheckCollision();
                            
                        }
                        else if (rowIndex != 9)
                        {
                            if (cells[rowIndex + 1, j].GetComponent<Cell>().GetState == Cell.State.Assigned && blocks[rowIndex + 1, j].GetComponent<Block>().state == Block.State.Fallen)
                            {
                                blocks[rowIndex, j].GetComponent<Block>().state = Block.State.Fallen;
                                blocks[rowIndex, j].GetComponent<Block>().UnSubscribeInputController();
                                blocks[rowIndex, j].GetComponent<Block>().CheckCollision();
                                
                            }
                        }
                    }
                }
            }
            else
            {
                blocks[rowIndex, j].GetComponent<Block>().state = Block.State.Fallen;
                blocks[rowIndex, j].GetComponent<Block>().UnSubscribeInputController();
                blocks[rowIndex, j].GetComponent<Block>().CheckCollision();
                

            }
        }
    }

    public void MoveBlock(InputController.SwipeType type, Vector2 cellIndex)
    {
        if (type == InputController.SwipeType.LEFT)
        {
            if (cellIndex.y - 1 >= 0)
            {
                if (cells[(int)cellIndex.x, (int)cellIndex.y - 1].GetComponent<Cell>().state == Cell.State.NotAssigned)
                {
                    blocks[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Block>().ChangePosition(cells[(int)cellIndex.x, (int)cellIndex.y - 1].transform.position, new Vector2(cellIndex.x, cellIndex.y - 1));
                    blocks[(int)cellIndex.x, (int)cellIndex.y - 1] = blocks[(int)cellIndex.x, (int)cellIndex.y];

                    cells[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    cells[(int)cellIndex.x, (int)cellIndex.y - 1].GetComponent<Cell>().state = Cell.State.Assigned;

                    if (cells[(int)cellIndex.x + 1, (int)cellIndex.y - 1].GetComponent<Cell>().GetState == Cell.State.Assigned && blocks[(int)cellIndex.x + 1, (int)cellIndex.y - 1].GetComponent<Block>().state == Block.State.Fallen)
                    {
                        blocks[(int)cellIndex.x, (int)cellIndex.y - 1].GetComponent<Block>().state = Block.State.Fallen;
                        blocks[(int)cellIndex.x, (int)cellIndex.y - 1].GetComponent<Block>().UnSubscribeInputController();
                        blocks[(int)cellIndex.x, (int)cellIndex.y - 1].GetComponent<Block>().CheckCollision();
                        
                    }
                }
            }
        }

        if (type == InputController.SwipeType.RIGHT)
        {
            if (cellIndex.y + 1 < columns)
            {
                if (cells[(int)cellIndex.x, (int)cellIndex.y + 1].GetComponent<Cell>().state == Cell.State.NotAssigned)
                {
                    blocks[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Block>().ChangePosition(cells[(int)cellIndex.x, (int)cellIndex.y + 1].transform.position, new Vector2(cellIndex.x, cellIndex.y + 1));
                    blocks[(int)cellIndex.x, (int)cellIndex.y + 1] = blocks[(int)cellIndex.x, (int)cellIndex.y];

                    cells[(int)cellIndex.x, (int)cellIndex.y].GetComponent<Cell>().state = Cell.State.NotAssigned;
                    cells[(int)cellIndex.x, (int)cellIndex.y + 1].GetComponent<Cell>().state = Cell.State.Assigned;

                    if (cells[(int)cellIndex.x + 1, (int)cellIndex.y + 1].GetComponent<Cell>().GetState == Cell.State.Assigned && blocks[(int)cellIndex.x + 1, (int)cellIndex.y + 1].GetComponent<Block>().state == Block.State.Fallen)
                    {
                        blocks[(int)cellIndex.x, (int)cellIndex.y + 1].GetComponent<Block>().state = Block.State.Fallen;
                        blocks[(int)cellIndex.x, (int)cellIndex.y + 1].GetComponent<Block>().UnSubscribeInputController();
                        blocks[(int)cellIndex.x, (int)cellIndex.y + 1].GetComponent<Block>().CheckCollision();
                        
                    }
                }
            }
        }

        if (type == InputController.SwipeType.DOWN)
        {
            //GameObject.Find("GameManager").GetComponent<GameManager>().iterationLength = 0.1f;
            Time.timeScale = 10;
            Debug.Log("DOWN SWIPE EVENT!");
        }

        if (cells[(int)cellIndex.x + 1, (int)cellIndex.y].GetComponent<Cell>().GetState == Cell.State.Assigned && 
            blocks[(int)cellIndex.x + 1, (int)cellIndex.y].GetComponent<Block>().state == Block.State.Fallen)
        {
            blocks[(int)cellIndex.x + 1, (int)cellIndex.y].GetComponent<Block>().UnSubscribeInputController();
        }
    }

    public List<Vector2> GetAvailableIdexes()
    {
        List<Vector2> availableIndexes = new List<Vector2>();
        for (int i = 0; i < columns; i++)
        {
            if(cells[0, i].GetComponent<Cell>().GetState == Cell.State.NotAssigned)
            {
                availableIndexes.Add(new Vector2(0, i));
            }
        }
        return availableIndexes;
    }

    public void SaveEditorState()
    {
        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                PlayerPrefs.SetInt(i + " " + j, (cells[i, j].GetComponent<Cell>().state == Cell.State.Assigned) ? Convert.ToInt32(blocks[i, j].GetComponent<Block>().type) : -1);
            }
        }
    }

    public void LoadEditorState()
    {  
        ClearMap();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (PlayerPrefs.HasKey(i + " " + j))
                {
                    if (PlayerPrefs.GetInt(i + " " + j) != -1)
                    {
                        cells[i, j].GetComponent<Cell>().state = Cell.State.Assigned;
                        blocks[i, j] = Instantiate(blockSpawner.blocks[PlayerPrefs.GetInt(i + " " + j)-1]);
                        blocks[i, j].GetComponent<Block>().ChangePosition(cells[i, j].transform.position, new Vector2(i, j));
                    } 
                }
            }
        }
    }

    public void ClearMap()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if(cells[i, j].GetComponent<Cell>().state == Cell.State.Assigned)
                {
                    Destroy(blocks[i, j]);
                    cells[i, j].GetComponent<Cell>().state = Cell.State.NotAssigned;
                }
            }
        }
    }
}