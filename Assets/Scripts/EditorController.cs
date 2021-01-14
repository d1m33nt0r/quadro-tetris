using UnityEngine;

public class EditorController : MonoBehaviour
{
    public GameObject[] editorButtons = new GameObject[7];

    private void Start()
    {
        EditorButton.pressedButtonEvent += Switch;
    }

    public GameObject GetActiveButton()
    {
        GameObject result = null;
        for (int i = 0; i < editorButtons.Length; i++)
        {
            if (editorButtons[i].GetComponent<EditorButton>().Enabled)
                result = editorButtons[i];
        }
        return result;
    }

    public void DisableAllButtons()
    {
        for (int i = 0; i < editorButtons.Length; i++)
        {
            if (editorButtons[i].GetComponent<EditorButton>().Enabled)
            {
                editorButtons[i].GetComponent<EditorButton>().Disable();
            }
        }
    }

    public void Switch(GameObject choosed)
    {
        for(int i = 0; i < editorButtons.Length; i++)
        {
            if(choosed == editorButtons[i].GetComponent<EditorButton>().block)
                editorButtons[i].GetComponent<EditorButton>().Enable();
            else
                editorButtons[i].GetComponent<EditorButton>().Disable();
        }
    }
}
