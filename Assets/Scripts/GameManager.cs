using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static Statee state { get; private set; }
    public float iterationLength = 1;
    private Map map;
    private Coroutine coroutine;
    public Statee State => state;
    
    private void Start()
    {
        map = GameObject.Find("Map").GetComponent<Map>(); 
        state = Statee.Editor;
        GameObject.Find("EditorCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;
    }

    public void SwitchState()
    {
        if (state == Statee.Editor)
            StartGame();        
        else
            StopGame();
    }


    public void StartGame()
    {
        GameObject.Find("Score").GetComponent<Score>().ResetScore();
        GameObject.Find("EditorCanvas").GetComponent<EditorController>().DisableAllButtons();
        map.SaveEditorState();
        GameObject.Find("EditorCanvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = true;
        state = Statee.Game;
        coroutine = StartCoroutine(NextIteration());
    }

    private IEnumerator NextIteration()
    {
        if (state == Statee.Game)
        {
            map.MoveBlocks();
            yield return new WaitForSeconds(iterationLength);
            coroutine = StartCoroutine(NextIteration());
        }   
    }

    public void StopGame()
    {
        GameObject.Find("Score").GetComponent<Score>().ResetScore();
        map.LoadEditorState();
        GameObject.Find("EditorCanvas").GetComponent<Canvas>().enabled = true;
        GameObject.Find("GameCanvas").GetComponent<Canvas>().enabled = false;
        state = Statee.Editor;
    }

    public enum Statee
    {
        Editor,
        Game
    }
}


/*
 using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static Statee state;
    private static Map map;
    private static Coroutine coroutine;
    public static Statee State => state;
    
    private static void Start()
    {
        map = GameObject.Find("Map").GetComponent<Map>(); 
        state = Statee.Editor;
    }

    public static void SwitchState()
    {
        if (state == Statee.Editor)
            StartGame();        
        else
            StopGame();
    }


    public static void StartGame()
    {
        state = Statee.Game;
        //SetTimeout(() => NextIteration(), 1000);
    }

    private static IEnumerator NextIteration()
    {
        
        Map.MoveBlocks();
        Debug.Log(true);
        if (state == Statee.Game)
        {
            yield return new WaitForSeconds(1);
            coroutine = StartCoroutine(NextIteration());
            
            //SetTimeout(() => NextIteration(), 1000);
        }
        
    }

    public static void StopGame()
    {
        state = Statee.Editor;
    }

    private static void SetTimeout(Action action, int delay) =>
    Task.Run(async () =>
    {
        await Task.Delay(delay);
        action();
    });

    public enum Statee
    {
        Editor,
        Game
    }
}

*/