using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Dialog Data")]
public class DialogData : ScriptableObject
{
    public List<DialogLine> lines = new();
}
