using UnityEngine;

[CreateAssetMenu(fileName = "Step_", menuName = "AgileLab/Step")]
public class StepData : ScriptableObject
{
    [Header("Text (on-screen + subtitle)")]
    public string title;
    [TextArea(2, 6)] public string description;

    [Header("Audio")]
    public AudioClip narration;

    [Header("3D Animation")]
    [Tooltip("Animator trigger name to fire in this step (leave empty for none)")]
    public string animationTrigger;

    [Header("Code Highlight")]
    [Tooltip("Index of the code line to highlight (0 = first line, -1 = none)")]
    public int highlightLine = -1;
}
