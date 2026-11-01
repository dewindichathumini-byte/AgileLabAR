using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("0 = To Do, 1 = In Progress, 2 = Done")]
    [SerializeField] int correctColumn;
    [TextArea][SerializeField] string explanation;

    SprintBoard board;
    Image image;
    Vector3 startPos;

    void Awake()
    {
        board = GetComponentInParent<SprintBoard>();
        image = GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        startPos = transform.position;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData e) { transform.position = e.position; }

    public void OnEndDrag(PointerEventData e)
    {
        int col = board.ColumnAt(e.position);
        if (col < 0)
        {
            transform.position = startPos;
            board.Show("Drop the card inside a column.", false);
            return;
        }
        if (col == correctColumn)
        {
            transform.SetParent(board.GetColumn(col), false);
            if (image != null) image.color = new Color(0.6f, 0.9f, 0.7f);
            board.Show("Correct! " + explanation, true);
            board.CardPlaced();
            enabled = false;   // lock the card
        }
        else
        {
            transform.position = startPos;
            board.Show("Not there yet. " + explanation, false);
        }
    }
}