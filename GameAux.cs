using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace QuizAppWPF
{
    class GameAux
    {
        private readonly static Random Rand = new Random();

        // creates list with random values from a list of possible values
        public static void CreateListWithRandomValues<T>(List<T> list, int numberOfPositions, List<T> listPossibleValues)
        {
            for (int i = 0; i < numberOfPositions; i++)
            {
                list.Add(listPossibleValues[Rand.Next(0, listPossibleValues.Count)]);
            }
        }

        public static Button GetButton(object buttonObject)
        {
            return ((Button)buttonObject);
        }

    }

    class Timer
    {
        private DispatcherTimer dispatcherTimer;
        public void StartTimer(Game obj)
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(obj.TimerCountdown);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        public void CounterTimeout()
        {
            dispatcherTimer.Stop();
        }

    }

}
