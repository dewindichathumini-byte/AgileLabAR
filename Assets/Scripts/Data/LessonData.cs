using UnityEngine;

[CreateAssetMenu(fileName = "Lesson_", menuName = "AgileLab/Lesson")]
public class LessonData : ScriptableObject
{
    public enum ExtraType { None, Simulator, Layers, SprintBoard }

    [Header("Info")]
    public string lessonName;
    public string linkedModule = "Software Engineering";
    [TextArea(2, 5)] public string learningObjective;

    [Header("Extra interaction for this lesson")]
    public ExtraType extra;

    [Header("Code lines shown beside the 3D model")]
    public string[] codeLines;

    [Header("Steps (in order)")]
    public StepData[] steps;

    [Header("3D content")]
    public GameObject modelPrefab;
}