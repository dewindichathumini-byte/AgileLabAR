using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StepController : MonoBehaviour
{
    [Header("Content")]
    [SerializeField] LessonData lesson;

    [Header("3D (optional parts can stay empty)")]
    [SerializeField] Transform modelRoot;
    [SerializeField] Animator modelAnimator;

    [Header("Audio (optional)")]
    [SerializeField] AudioSource audioSource;

    [Header("UI")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text codeText;
    [SerializeField] TMP_Text counterText;

    const float RestY = 0.015f;
    const float LiftedY = 0.06f;

    readonly List<Transform> stations = new List<Transform>();
    int index = -1;

    void Start()
    {
        if (modelRoot != null)
        {
            foreach (Transform child in modelRoot)
                if (child.name.StartsWith("Station_"))
                    stations.Add(child);
        }
        GoTo(0);
    }

    public void Next() { GoTo(index + 1); }
    public void Back() { GoTo(index - 1); }

    void GoTo(int i)
    {
        if (lesson == null || lesson.steps == null) return;
        if (i < 0 || i >= lesson.steps.Length) return;
        if (lesson.steps[i] == null) return;

        index = i;
        StepData step = lesson.steps[index];

        if (titleText != null) titleText.text = step.title;
        if (descText != null) descText.text = step.description;
        if (counterText != null)
            counterText.text = (index + 1) + " / " + lesson.steps.Length;

        RenderCode(step.highlightLine);
        HighlightStation(index);

        if (modelAnimator != null && !string.IsNullOrEmpty(step.animationTrigger))
            modelAnimator.SetTrigger(step.animationTrigger);

        if (audioSource != null && step.narration != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(step.narration);
        }
    }

    void RenderCode(int highlight)
    {
        if (codeText == null || lesson.codeLines == null) return;

        var sb = new StringBuilder();
        for (int l = 0; l < lesson.codeLines.Length; l++)
        {
            if (l == highlight)
                sb.AppendLine("<mark=#FFD54F55><b>" + lesson.codeLines[l] + "</b></mark>");
            else
                sb.AppendLine(lesson.codeLines[l]);
        }
        codeText.text = sb.ToString();
    }

    void HighlightStation(int current)
    {
        for (int k = 0; k < stations.Count; k++)
        {
            Vector3 p = stations[k].localPosition;
            p.y = (k == current) ? LiftedY : RestY;
            stations[k].localPosition = p;
        }
    }
}