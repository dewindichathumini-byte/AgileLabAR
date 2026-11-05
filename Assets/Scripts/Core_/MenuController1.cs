using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] LessonCatalog catalog;
    [SerializeField] TMP_Text progressText;
    [SerializeField] GameObject onboardingPanel;

    static bool warningShown;

    // Resets the flag every time the app (or Editor Play mode) starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { warningShown = false; }

    void Start()
    {
        if (onboardingPanel != null)
        {
            onboardingPanel.SetActive(!warningShown);   // shown once per app launch
            warningShown = true;
        }
        RefreshProgress();
    }

    public void CloseOnboarding() { onboardingPanel.SetActive(false); }

    public void OpenLesson(int i)
    {
        if (catalog == null || i < 0 || i >= catalog.lessons.Length) return;
        AppState.SelectedLesson = catalog.lessons[i];
        SceneManager.LoadScene("ARLesson");
    }

    public void OpenQuiz(int mode)
    {
        AppState.CurrentQuiz = (AppState.QuizMode)mode;   // 0 = Pre, 1 = Post, 2 = Final
        SceneManager.LoadScene("Quiz");
    }

    void RefreshProgress()
    {
        SaveData d = SaveSystem.Load();
        progressText.text =
            "Lessons completed: " + d.completed.Count + " / " + catalog.lessons.Length +
            "\nPre-quiz: " + Fmt(d.pre) + "    Post-quiz: " + Fmt(d.post) +
            "\nFinal challenge: " + Fmt(d.finalScore);
    }

    static string Fmt(int s) { return s < 0 ? "-" : s + "%"; }
}