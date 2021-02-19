using System.Windows.Input;

namespace QuizAppWPF
{
    class LoadingCursor
    {
        public static void StartLoadingCursor()
        {
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public static void StopLoadingCursor()
        {
            Mouse.OverrideCursor = null;
        }

    }
}
