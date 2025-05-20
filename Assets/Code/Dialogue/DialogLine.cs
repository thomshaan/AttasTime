using UnityEngine;

[System.Serializable]
public class DialogLine
{
    [TextArea] public string text;
    public string animationTrigger;
    public Transform moveTarget; // Optional: used on last line
}
