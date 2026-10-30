using UnityEngine;

[CreateAssetMenu(fileName = "Lesson_", menuName = "AgileLab/Lesson")]
public class LessonData : ScriptableObject
{
    [Header("Info")]
    public string lessonName;
    public string linkedModule = "Software Engineering";
    [TextArea(2, 5)] public string learningObjective;

    [Header("Code shown beside the 3D model (one line per element)")]
    public string[] codeLines;

    [Header("Steps (in order)")]
    public StepData[] steps;

    [Header("3D content")]
    public GameObject modelPrefab;
}

