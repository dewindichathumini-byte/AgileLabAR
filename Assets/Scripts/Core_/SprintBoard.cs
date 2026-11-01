
using TMPro;
using UnityEngine;

public class SprintBoard : MonoBehaviour
{
    [SerializeField] RectTransform[] columns;   // 0 = To Do, 1 = In Progress, 2 = Done
    [SerializeField] TMP_Text feedbackText;
    [SerializeField] int totalCards = 5;
    int placed;

    public int ColumnAt(Vector2 screenPoint)
    {
        for (int i = 0; i < columns.Length; i++)
            if (RectTransformUtility.RectangleContainsScreenPoint(columns[i], screenPoint, null))
                return i;
        return -1;
    }

    public RectTransform GetColumn(int i) { return columns[i]; }

    public void Show(string message, bool good)
    {
        feedbackText.text = message;
        feedbackText.color = good ? new Color(0.55f, 1f, 0.65f) : new Color(1f, 0.65f, 0.45f);
    }

    public void CardPlaced()
    {
        placed++;
        if (placed >= totalCards)
        {
            Show("Sprint board complete! Great work.", true);
            SaveSystem.MarkCompleted("Sprint Board Challenge");
        }
    }
}