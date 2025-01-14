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
        //Private vars
        private DateTime _currentDate;
        private bool _isPaused;
        private CancellationTokenSource _pauseTokenSource;
        private List<Task> _dailyTasks;
        private Task _dayTimer;

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

        public GameState()
        {
            //Init day timer
            _dayTimer = Task.Delay(1000);

            //Init lists
            _dailyTasks = new List<Task>();
            _dailyTasks.Add(_dayTimer);

            //Setup commands
            PlayCommand = new RelayCommand(PlayGame);
            PauseCommand = new RelayCommand(PauseGame);
        }

        //Private funcs
        private async void StartGameTimer(CancellationToken cancelToken)
        {
            while (!IsPaused)
            {
                try
                {
                    // Start the delay and calculations in parallel
                    await Task.WhenAll(_dailyTasks);
                }
                catch (TaskCanceledException)
                {
                    // The task was cancelled by pausing the game, proceed as normal.
                    break;
                }

                // After delay and calculations, proceed the game timer
                CurrentDate.AddDays(1);
                Console.WriteLine(CurrentDate);
            }
        }

        //Public funcs
        public void RegisterDailyTask(Task task)
        {
            _dailyTasks.Add(task);
        }

        //Command handling
        private void PlayGame()
        {
            IsPaused = false;
            _pauseTokenSource = new CancellationTokenSource();
            StartGameTimer(_pauseTokenSource.Token);

        }

        private void PauseGame()
        {
            IsPaused = true;
            _pauseTokenSource?.Cancel();
        }
    }
}
