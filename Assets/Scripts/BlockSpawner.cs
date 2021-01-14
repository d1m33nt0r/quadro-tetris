using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    private Map map;
    public GameObject[] blocks;

    private void Start()
    {
        map = GameObject.Find("Map").GetComponent<Map>();
    }

    private GameObject GetRandomBlock()
    {
        return blocks[Random.Range(0, blocks.Length-1)];
    }

    public void SpawnBlockInRandomPosition()
    {
        List<Vector2> availableIndexes = map.GetAvailableIdexes();
        if(availableIndexes.Count != 0)
        {
            int rand = Random.Range(0, availableIndexes.Count);
            Vector2 index = availableIndexes[rand];

            map.blocks[(int)index.x, (int)index.y] = Instantiate(GetRandomBlock());
            map.blocks[(int)index.x, (int)index.y].GetComponent<Block>().ChangePosition(map.cells[(int)index.x, (int)index.y].transform.position, new Vector2((int)index.x, (int)index.y));
            map.blocks[(int)index.x, (int)index.y].GetComponent<Block>().SubscribeInputController();
            map.cells[(int)index.x, (int)index.y].GetComponent<Cell>().state = Cell.State.Assigned;
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().SwitchState();
        }
    }
}
