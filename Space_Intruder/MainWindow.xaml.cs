using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Space_Intruder.Class;
using Space_Intruder.GameObjects;

namespace Space_Intruder
{
    public partial class MainWindow : Window
    {
        private bool isGameOver = false;
        private Level_Gry gameLevel;
        private DispatcherTimer gameTimer;
        private Hero player;

        public MainWindow()
        {
            InitializeComponent();
            MyCanvas.Loaded += (sender, e) => InitializeGame();
        }

        private void InitializeGame()
        {
            Debug.WriteLine($"Canvas dimensions: {MyCanvas.ActualWidth}x{MyCanvas.ActualHeight}");

            // Create hero
            player = new Hero(MyCanvas, 200, 20);

            // Initialize game level
            gameLevel = new Level_Gry(MyCanvas, player);
            gameLevel.LoadLevel(5);
            Debug.WriteLine($"Level 1 loaded with {gameLevel.GetCurrentEnemies().Count} enemies");

            // Game loop
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(16);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            // Update UI
            current_level.Text = $"Level {gameLevel.CurrentLevel}";
            current_life.Text = new string('❤', player.Lives);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (isGameOver) return;

            if (e.Key == Key.Left) player.MoveLeft(0);
            else if (e.Key == Key.Right) player.MoveRight(MyCanvas.ActualWidth);
            else if (e.Key == Key.Space) player.Shoot(MyCanvas, gameLevel.GetCurrentEnemies());
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isGameOver) return;

            gameLevel.UpdateEnemies();
            current_life.Text = new string('❤', player.Lives);

            if (gameLevel.AreAllEnemiesDefeated())
            {
                if (gameLevel.IsGameCompleted)
                {
                    EndGame(true);
                }
                else
                {
                    gameLevel.NextLevel();
                    current_level.Text = $"Level {gameLevel.CurrentLevel}";
                    Debug.WriteLine($"Level {gameLevel.CurrentLevel} loaded with {gameLevel.GetCurrentEnemies().Count} enemies");
                }
            }
        }

        private void EndGame(bool isWin)
        {
            isGameOver = true;
            gameTimer?.Stop();
            gameLevel?.StopAllEnemies();
            MessageBox.Show(isWin ? "Congratulations! You won!" : "Game Over!");
        }

        public void SlowDownPlayer()
        {
            player.ApplySlowEffect(0.5, 2.0);
        }

        public void PlayerHit()
        {
            player.TakeDamage();
            UpdateLifeDisplay();
            if (!player.IsAlive) EndGame(false);
        }

        private void UpdateLifeDisplay()
        {
            if (player.Lives >= 0)
            {
                current_life.Text = new string('❤', player.Lives);
            }
        }
    }
}