using Space_Intruder.GameObjects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Space_Intruder.Class
{
    public class Level_Gry
    {
        public int CurrentLevel { get; private set; } = 1;
        public int TotalLevels { get; } = 9;
        public bool IsGameCompleted { get; private set; }

        private Canvas gameCanvas;
        private Hero player;
        private List<Enemy> enemies = new List<Enemy>();

        public Level_Gry(Canvas gameCanvas, Hero player)
        {
            this.gameCanvas = gameCanvas;
            this.player = player;
        }

        public void LoadLevel(int level)
        {
            ClearEnemies();
            CurrentLevel = level;

            switch (level)
            {
                case 1:
                    CreateLevel1();
                    break;
                case 2:
                    CreateLevel2();
                    break;
                case 3:
                    CreateLevel3();
                    break;
                case 4:
                    CreateLevel4();
                    break;
                case 5:
                    CreateLevel5();
                    break;
                case 6:
                    CreateLevel6();
                    break;
                case 7:
                    CreateLevel7();
                    break;
                case 8:
                    CreateLevel8();
                    break;
                case 9:
                    CreateLevel9();
                    break;
                default:
                    IsGameCompleted = true;
                    break;
            }
        }

        public void NextLevel()
        {
            if (CurrentLevel < TotalLevels)
            {
                LoadLevel(CurrentLevel + 1);
            }
            else
            {
                IsGameCompleted = true;
            }
        }

        public void StopAllEnemies()
        {
            foreach (var enemy in enemies.ToList())
            {
                if (enemy is MageEnemy mage) mage.StopShooting();
                if (enemy is SpiderEnemy spider) spider.StopShooting();
                gameCanvas.Children.Remove(enemy.Visual);
            }
            enemies.Clear();
        }

        public bool AreAllEnemiesDefeated()
        {
            return enemies.Count == 0;
        }

        public List<Enemy> GetCurrentEnemies() => enemies;

        public void UpdateEnemies()
        {
            foreach (var enemy in enemies.ToList())
            {
                enemy.Move();

                // Check collision with edges
                double enemyX = Canvas.GetLeft(enemy.Visual);
                if (enemyX <= 0 || enemyX + enemy.Visual.Width >= gameCanvas.ActualWidth)
                {
                    enemy.Direction *= -1;
                    double currentY = Canvas.GetBottom(enemy.Visual);
                    Canvas.SetBottom(enemy.Visual, currentY - 20);
                }

                // Check collision with player
                Rect enemyRect = new Rect(
                    Canvas.GetLeft(enemy.Visual),
                    Canvas.GetBottom(enemy.Visual),
                    enemy.Visual.Width,
                    enemy.Visual.Height);

                Rect playerRect = new Rect(
                    player.PositionX,
                    player.PositionY,
                    player.Width,
                    player.Height);

                if (enemyRect.IntersectsWith(playerRect))
                {
                    enemy.TakeDamage(1);
                    player.TakeDamage();
                }
            }
        }

        private void ClearEnemies()
        {
            foreach (var enemy in enemies)
            {
                if (enemy is MageEnemy mage) mage.StopShooting();
                if (enemy is SpiderEnemy spider) spider.StopShooting();
                gameCanvas.Children.Remove(enemy.Visual);
            }
            enemies.Clear();
        }

        private void CreateEnemyPattern(int rows, int cols, double spacingX, double spacingY,
                                      double startX, double startY, Func<int, double, double, Enemy> enemyCreator)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    double x = startX + col * spacingX;
                    double y = startY - row * spacingY;
                    var enemy = enemyCreator(row, x, y);
                    enemies.Add(enemy);
                    gameCanvas.Children.Add(enemy.Visual);
                }
            }
        }

        private void CreateLevel1()
        {
            CreateEnemyPattern(2, 5, 80, 60, 50, gameCanvas.ActualHeight - 100,
                (row, x, y) => new BasicEnemy(x, y, CurrentLevel));
        }

        private void CreateLevel2()
        {
            CreateEnemyPattern(3, 4, 90, 70, 60, gameCanvas.ActualHeight - 100,
                (row, x, y) => new BasicEnemy(x, y, CurrentLevel));
        }

        private void CreateLevel3()
        {
            CreateEnemyPattern(2, 6, 70, 60, 40, gameCanvas.ActualHeight - 100,
                (row, x, y) => row < 1 ? new BasicEnemy(x, y, CurrentLevel) :
                                       new MageEnemy(x, y, gameCanvas, player, CurrentLevel));
        }

        private void CreateLevel4()
        {
            CreateEnemyPattern(3, 5, 80, 65, 50, gameCanvas.ActualHeight - 100,
                (row, x, y) => row == 0 ? new BasicEnemy(x, y, CurrentLevel) :
                          row == 1 ? new MageEnemy(x, y, gameCanvas, player, CurrentLevel) :
                          new TankEnemy(x, y, CurrentLevel));
        }

        private void CreateLevel5()
        {
            CreateEnemyPattern(4, 6, 75, 60, 40, gameCanvas.ActualHeight - 100,
                (row, x, y) => row < 2 ? new BasicEnemy(x, y, CurrentLevel) :
                          row == 2 ? new MageEnemy(x, y, gameCanvas, player, CurrentLevel) :
                          new TankEnemy(x, y, CurrentLevel));
        }

        private void CreateLevel6()
        {
            CreateEnemyPattern(3, 7, 70, 65, 30, gameCanvas.ActualHeight - 100,
                (row, x, y) => row == 0 ? new BasicEnemy(x, y, CurrentLevel) :
                          row == 1 ? new MageEnemy(x, y, gameCanvas, player, CurrentLevel) :
                          new SpiderEnemy(x, y, gameCanvas, player, CurrentLevel));
        }

        private void CreateLevel7()
        {
            CreateEnemyPattern(4, 6, 80, 70, 40, gameCanvas.ActualHeight - 120,
                (row, x, y) => row < 1 ? new BasicEnemy(x, y, CurrentLevel) :
                          row < 3 ? new MageEnemy(x, y, gameCanvas, player, CurrentLevel) :
                          new TankEnemy(x, y, CurrentLevel));
        }

        private void CreateLevel8()
        {
            CreateEnemyPattern(5, 5, 85, 60, 50, gameCanvas.ActualHeight - 150,
                (row, x, y) => row < 2 ? new BasicEnemy(x, y, CurrentLevel) :
                          row < 4 ? new MageEnemy(x, y, gameCanvas, player, CurrentLevel) :
                          new SpiderEnemy(x, y, gameCanvas, player, CurrentLevel));
        }

        private void CreateLevel9()
        {
            CreateEnemyPattern(6, 6, 70, 55, 30, gameCanvas.ActualHeight - 180,
                (row, x, y) => row < 1 ? new BasicEnemy(x, y, CurrentLevel) :
                          row < 3 ? new MageEnemy(x, y, gameCanvas, player, CurrentLevel) :
                          row < 5 ? new TankEnemy(x, y, CurrentLevel) :
                          new SpiderEnemy(x, y, gameCanvas, player, CurrentLevel));
        }
    }
}