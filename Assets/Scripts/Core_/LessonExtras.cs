using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonExtras : MonoBehaviour
{
    [Header("Hidden for the sprint board")]
    [SerializeField] GameObject stepPanel;

    [Header("Simulator (Waterfall vs Agile)")]
    [SerializeField] GameObject simPanel;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text simLabel;

    [Header("Layers (Scrum)")]
    [SerializeField] GameObject layerPanel;
    [SerializeField] Toggle rolesToggle;
    [SerializeField] Toggle eventsToggle;
    [SerializeField] Toggle artifactsToggle;

    [Header("Sprint board")]
    [SerializeField] GameObject boardPanel;

    Transform waterfallBar, agileBar;
    Renderer waterfallRenderer;
    GameObject rolesGroup, eventsGroup, artifactsGroup;

    static void Set(GameObject g, bool on) { if (g != null) g.SetActive(on); }

    void Awake()
    {
        Hide();
        if (slider != null) slider.onValueChanged.AddListener(ApplySim);
        if (rolesToggle != null) rolesToggle.onValueChanged.AddListener(v => Set(rolesGroup, v));
        if (eventsToggle != null) eventsToggle.onValueChanged.AddListener(v => Set(eventsGroup, v));
        if (artifactsToggle != null) artifactsToggle.onValueChanged.AddListener(v => Set(artifactsGroup, v));
    }

    public void Hide()
    {
        Set(simPanel, false);
        Set(layerPanel, false);
        Set(boardPanel, false);
    }

    public void Setup(LessonData lesson, Transform root)
    {
        Hide();
        Set(stepPanel, lesson.extra != LessonData.ExtraType.SprintBoard);

        if (lesson.extra == LessonData.ExtraType.Simulator)
        {
            Set(simPanel, true);
            waterfallBar = root.Find("WaterfallBar");
            agileBar = root.Find("AgileBar");
            waterfallRenderer = waterfallBar != null ? waterfallBar.GetComponent<Renderer>() : null;
            if (slider != null) slider.value = 0f;
            ApplySim(0f);
        }
        else if (lesson.extra == LessonData.ExtraType.Layers)
        {
            Set(layerPanel, true);
            rolesGroup = Find(root, "Group_Roles");
            eventsGroup = Find(root, "Group_Events");
            artifactsGroup = Find(root, "Group_Artifacts");
            if (rolesToggle != null) rolesToggle.isOn = true;
            if (eventsToggle != null) eventsToggle.isOn = true;
            if (artifactsToggle != null) artifactsToggle.isOn = true;
        }
        else if (lesson.extra == LessonData.ExtraType.SprintBoard)
        {
            Set(boardPanel, true);
        }
    }

    static GameObject Find(Transform root, string childName)
    {
        Transform t = root.Find(childName);
        return t != null ? t.gameObject : null;
    }

    void ApplySim(float v)
    {
        if (waterfallBar == null || agileBar == null) return;
        SetHeight(waterfallBar, 0.04f + v * 0.30f);
        SetHeight(agileBar, 0.04f + v * 0.06f);
        if (waterfallRenderer != null)
            waterfallRenderer.material.color = Color.Lerp(new Color(0f, 0.62f, 0.45f), new Color(0.84f, 0.37f, 0f), v);
        if (simLabel != null)
            simLabel.text = "Requirement changes: " + Mathf.RoundToInt(v * 100f) + "%\nBar height = rework cost";
    }

    static void SetHeight(Transform t, float h)
    {
        Vector3 s = t.localScale; s.y = h; t.localScale = s;
        Vector3 p = t.localPosition; p.y = h * 0.5f; t.localPosition = p;
    }
}