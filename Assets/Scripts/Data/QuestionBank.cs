using UnityEngine;

[CreateAssetMenu(menuName = "AgileLab/Question Bank")]
public class QuestionBank : ScriptableObject
{
    public QuestionData[] preQuestions;
    public QuestionData[] postQuestions;
    public QuestionData[] finalQuestions;
}