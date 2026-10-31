using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StepController : MonoBehaviour
{
    [Header("Optional: only for quick tests. In the AR scene leave empty.")]
    [SerializeField] LessonData lesson;
    [SerializeField] Transform modelRoot;

    [SerializeField] Animator modelAnimator;
    [SerializeField] AudioSource audioSource;

    [Header("UI")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text codeText;
    [SerializeField] TMP_Text counterText;

    const float LiftHeight = 0.04f;

    readonly List<Transform> stations = new List<Transform>();
    readonly List<float> restY = new List<float>();
    Transform marker;
    Vector3 markerTarget;
    bool hasMarkerTarget;
    bool begun;
    int index = -1;

    void Start()
    {
        if (!begun && lesson != null && modelRoot != null) Begin(lesson, modelRoot);
    }

    // Called by the AR placer after the model is placed
    public void Begin(LessonData l, Transform root)
    {
        begun = true;
        lesson = l;
        modelRoot = root;
        stations.Clear();
        restY.Clear();
        marker = null;
        hasMarkerTarget = false;

        foreach (Transform child in root)
        {
            if (child.name.StartsWith("Station_"))
            {
                stations.Add(child);
                restY.Add(child.localPosition.y);
            }
            else if (child.name == "Marker")
            {
                marker = child;
            }
        }
        index = -1;
        GoTo(0);
    }

    void Update()
    {
        if (marker != null && hasMarkerTarget)
            marker.localPosition = Vector3.Lerp(marker.localPosition, markerTarget, Time.deltaTime * 6f);
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
        if (counterText != null) counterText.text = (index + 1) + " / " + lesson.steps.Length;

        RenderCode(step.highlightLine);
        HighlightStation(index);

        if (modelAnimator != null && !string.IsNullOrEmpty(step.animationTrigger))
            modelAnimator.SetTrigger(step.animationTrigger);

        if (audioSource != null && step.narration != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(step.narration);
        }

        if (index == lesson.steps.Length - 1)
            SaveSystem.MarkCompleted(lesson.lessonName);
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
            p.y = restY[k] + (k == current ? LiftHeight : 0f);
            stations[k].localPosition = p;
        }
        if (marker != null && current < stations.Count)
        {
            Vector3 s = stations[current].localPosition;
            markerTarget = new Vector3(s.x, restY[current] + 0.12f, s.z);
            hasMarkerTarget = true;
        }
    }
}