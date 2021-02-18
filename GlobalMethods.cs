using QuizAppWPF;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using System;
using System.Windows.Input;
using FireSharp.Interfaces;
using FireSharp.Config;

namespace GlobalMethods
{
    public static class GlobalMethods
    {
        public static string ReplaceString(string baseString, string expressionToReplace, string replacementValue)
        {
            return baseString.Replace(expressionToReplace, replacementValue);
        }

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


