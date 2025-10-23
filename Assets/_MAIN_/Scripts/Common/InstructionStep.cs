using UnityEngine;

[System.Serializable]
public struct InstructionStep
{
    public string title;
    [TextArea]
    public string description;
    public float duration;
}