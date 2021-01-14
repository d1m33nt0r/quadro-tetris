using UnityEngine;
using UnityEngine.UI;

public class EditorButton : MonoBehaviour
{
    public Sprite active, disabled;
    public delegate void PressedButton(GameObject obj);
    public static event PressedButton pressedButtonEvent;
    public GameObject block;

    public string type;

    public bool Enabled => isEnabled;

    private bool isEnabled;
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    public void OnClick()
    {
        pressedButtonEvent(block);
    }

    public void Enable()
    {
        img.sprite = active;
        isEnabled = true;
    }

    public void Disable()
    {
        img.sprite = disabled;
        isEnabled = false;
    }
}
