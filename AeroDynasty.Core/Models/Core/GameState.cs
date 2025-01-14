using AeroDynasty.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AeroDynasty.Core.Models.Core
{
    public class GameState : _BaseModel
    {
        //Singleton Instance
        private static GameState _instance;
        public static GameState Instance => _instance ?? (_instance = new GameState());

        //Private vars
        private DateTime _currentDate;
        private bool _isPaused;
        private CancellationTokenSource _pauseTokenSource;
        private List<Func<Task>> _dailyTasks;

        //Public vars
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(CurrentDate));
            }
        }
        public string FormattedCurrentDate => CurrentDate.ToString("dd-MMM-yyyy");
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                OnPropertyChanged(nameof(IsPaused));
            }
        }

        //Commands
        public ICommand PlayCommand { get; set; }
        public ICommand PauseCommand { get; set; }

        private GameState()
        {
            //Init lists
            _dailyTasks = new List<Func<Task>>();

            //Set startdate
            CurrentDate = new DateTime(1946, 1, 1);
            IsPaused = true;

            //Setup commands
            PlayCommand = new RelayCommand(PlayGame);
            PauseCommand = new RelayCommand(PauseGame);
        }

        //Private funcs
        private async void StartGameTimer(CancellationToken cancelToken)
        {
            while (true) // Use CancellationToken to break the loop
            {
                try
                {
                    // Create a new delay task for each iteration (1 second)
                    var tasksToWait = new List<Task> { Task.Delay(1000, cancelToken) };

                    // Add any additional tasks (daily calculations) to the list
                    tasksToWait.AddRange(_dailyTasks.Select(task => task())); // Tasks registered via RegisterDailyTask

                    // Wait for all tasks to complete
                    await Task.WhenAll(tasksToWait);

                    // Advance the game date after waiting for all tasks
                    CurrentDate = CurrentDate.AddDays(1);
                    Console.WriteLine(DateTime.Now + ": " + CurrentDate);
                }
                catch (TaskCanceledException)
                {
                    // Exit the loop gracefully when cancellation is requested
                    break;
                }
            }
        }

        //Public funcs
        public void RegisterDailyTask(Func<Task> task)
        {
            _dailyTasks.Add(task);
        }

        //Command handling
        private void PlayGame()
        {
            if (IsPaused)
            {
                IsPaused = false;
                _pauseTokenSource = new CancellationTokenSource();
                StartGameTimer(_pauseTokenSource.Token);
            }
        }

        private void PauseGame()
        {
            IsPaused = true;
            _pauseTokenSource?.Cancel();
        }
    }
}
