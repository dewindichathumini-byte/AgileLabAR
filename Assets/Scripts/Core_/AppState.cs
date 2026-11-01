public static class AppState
{
    public enum QuizMode { Pre = 0, Post = 1, Final = 2 }

    public static LessonData SelectedLesson;
    public static QuizMode CurrentQuiz = QuizMode.Pre;
}