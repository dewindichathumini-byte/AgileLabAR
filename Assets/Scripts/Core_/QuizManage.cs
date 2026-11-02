using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [SerializeField] QuestionBank bank;
    [SerializeField] TMP_Text titleText, progressText, questionText, feedbackText, resultText;
    [SerializeField] Button[] optionButtons;
    [SerializeField] TMP_Text[] optionTexts;
    [SerializeField] Button nextButton;
    [SerializeField] TMP_Text nextButtonText;
    [SerializeField] GameObject quizPanel, resultPanel;

    static readonly Color Good = new Color(0.55f, 0.85f, 0.6f);
    static readonly Color Bad = new Color(0.95f, 0.6f, 0.55f);

    QuestionData[] questions;
    int index, correct;
    bool answered;

    void Start()
    {
        switch (AppState.CurrentQuiz)
        {
            case AppState.QuizMode.Pre:
                questions = bank.preQuestions; titleText.text = "Pre-Quiz"; break;
            case AppState.QuizMode.Post:
                questions = bank.postQuestions; titleText.text = "Post-Quiz"; break;
            default:
                questions = bank.finalQuestions; titleText.text = "Final Challenge"; break;
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int c = i;
            optionButtons[i].onClick.AddListener(() => Answer(c));
        }
        nextButton.onClick.AddListener(Next);
        resultPanel.SetActive(false);

        if (questions == null || questions.Length == 0)
        {
            questionText.text = "No questions found. Run AgileLab > Import Questions from CSV.";
            return;
        }
        Show(0);
    }

    void Show(int i)
    {
        index = i;
        answered = false;
        QuestionData q = questions[i];
        progressText.text = "Question " + (i + 1) + " / " + questions.Length;
        questionText.text = q.question;
        feedbackText.text = "";

        for (int k = 0; k < optionButtons.Length; k++)
        {
            bool has = k < q.options.Length;
            optionButtons[k].gameObject.SetActive(has);
            if (!has) continue;
            optionTexts[k].text = q.options[k];
            optionButtons[k].image.color = Color.white;
        }
        nextButton.gameObject.SetActive(false);
    }

    void Answer(int c)
    {
        if (answered) return;
        answered = true;
        QuestionData q = questions[index];
        bool ok = c == q.correctIndex;
        if (ok) correct++;

        optionButtons[c].image.color = ok ? Good : Bad;
        optionButtons[q.correctIndex].image.color = Good;
        feedbackText.text = (ok ? "Correct! " : "Not quite. ") + q.feedback;
        nextButtonText.text = index == questions.Length - 1 ? "Finish" : "Next";
        nextButton.gameObject.SetActive(true);
    }

    void Next()
    {
        if (index + 1 < questions.Length) Show(index + 1);
        else Finish();
    }

    void Finish()
    {
        int percent = Mathf.RoundToInt(100f * correct / questions.Length);
        SaveData d = SaveSystem.Load();
        string extra = "";

        switch (AppState.CurrentQuiz)
        {
            case AppState.QuizMode.Pre: d.pre = percent; break;
            case AppState.QuizMode.Post:
                d.post = percent;
                if (d.pre >= 0) extra = "\nPre-quiz: " + d.pre + "%   Gain: " + (percent - d.pre) + " points";
                break;
            default: d.finalScore = percent; break;
        }
        SaveSystem.Save(d);

        quizPanel.SetActive(false);
        resultPanel.SetActive(true);
        resultText.text = titleText.text + " score\n" + correct + " / " + questions.Length + "  (" + percent + "%)" + extra;
    }

    public void BackToMenu() { SceneManager.LoadScene("MainMenu"); }
}