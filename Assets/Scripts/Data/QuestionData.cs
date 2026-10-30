using UnityEngine;

[CreateAssetMenu(fileName = "Q_", menuName = "AgileLab/Question")]
public class QuestionData : ScriptableObject
{
    [TextArea(2, 6)] public string question;
    public string[] options;          // answer choices (usually 4)
    public int correctIndex;          // 0 = first option
    [TextArea(2, 6)] public string feedback;  // shown after answering

    [Header("Quiz set")]
    public bool usedInPreQuiz;        // tick = pre-quiz, untick = post-quiz
}