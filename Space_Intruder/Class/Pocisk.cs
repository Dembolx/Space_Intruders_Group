using Space_Intruder.Class;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using Space_Intruder;

public class Pocisk
{
    private Rectangle visual; // Wizualna reprezentacja pocisku
    private double speed; // Prędkość pocisku
    private Canvas canvas; // Canvas, na którym znajduje się pocisk
    private List<Enemy> enemies; // Lista przeciwników
    private DispatcherTimer timer; // Timer do aktualizacji pozycji pocisku
    private int direction; // Kierunek pocisku: 1 = w górę, -1 = w dół
    private Rectangle player; // Gracz (Klocek)
    private string postac;
    private Brush color = Brushes.White;
    private bool isSpiderShot;


    public Pocisk(string postac, double speed, double startX, double startY, Canvas canvas, List<Enemy> enemies, Rectangle player, int direction = 1)
    {
        this.speed = speed;
        this.canvas = canvas;
        this.enemies = enemies;
        this.direction = direction;
        this.player = player;
        this.postac = postac;
        this.isSpiderShot = postac == "spider"; // Oznaczamy pocisk od spidera

        switch (postac)
        {
            case "bohater":
                color = Brushes.Cyan;
                break;
            case "mag":
                color = Brushes.DarkViolet;
                break;
            case "spider":
                color = Brushes.White;
                break;
            default:
                break;
        };

        visual = new Rectangle
        {
            Width = 5,
            Height = 15,
            Fill = color
        };

        Canvas.SetLeft(visual, startX);
        Canvas.SetBottom(visual, startY);
        canvas.Children.Add(visual);

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        // Przesuwamy pocisk w zależności od kierunku
        double currentY = Canvas.GetBottom(visual);
        Canvas.SetBottom(visual, currentY + speed * direction);

        // Sprawdzamy kolizję z przeciwnikami (jeśli pocisk leci w górę)
        if (direction == 1)
        {
            CheckCollision();
        }
        // Sprawdzamy kolizję z graczem (jeśli pocisk leci w dół)
        else if (direction == -1)
        {
            CheckPlayerCollision();
        }

        // Jeśli pocisk wyjdzie poza canvas, usuwamy go
        if (currentY > canvas.ActualHeight || currentY < 0)
        {
            timer.Stop();
            canvas.Children.Remove(visual);
        }
    }

    private void CheckPlayerCollision()
    {
        Rect pociskRect = new Rect(Canvas.GetLeft(visual), Canvas.GetBottom(visual), visual.Width, visual.Height);
        Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetBottom(player), player.Width, player.Height);

        if (pociskRect.IntersectsWith(playerRect))
        {
            // Kolizja! Usuwamy pocisk
            canvas.Children.Remove(visual);
            timer.Stop();

            if (postac == "spider")
            {
                // Spowolnienie ruchu gracza, jeśli to strzał od spidera
                if (isSpiderShot)
                {
                    MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.SlowDownPlayer();
                    }
                }
            }
            else if (postac == "mag")
            {
                // Zadajemy obrażenia graczowi
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.PlayerHit();
                }
            }
        }
    }


    private void CheckCollision()
    {
        // Pobieramy pozycję i rozmiar pocisku
        Rect pociskRect = new Rect(Canvas.GetLeft(visual), Canvas.GetBottom(visual), visual.Width, visual.Height);

        // Sprawdzamy kolizję z każdym przeciwnikiem
        foreach (var enemy in enemies.ToList()) // Używamy ToList(), aby uniknąć modyfikacji kolekcji podczas iteracji
        {
            Rect enemyRect = new Rect(Canvas.GetLeft(enemy.Visual), Canvas.GetBottom(enemy.Visual), enemy.Visual.Width, enemy.Visual.Height);

            if (pociskRect.IntersectsWith(enemyRect))
            {
                // Kolizja! Zadajemy obrażenia przeciwnikowi
                enemy.Health--;

                // Jeśli przeciwnik nie ma już żyć, usuwamy go
                if (enemy.Health <= 0)
                {
                    // Jeśli przeciwnik to MageEnemy, zatrzymujemy jego strzelanie
                    if (enemy is MageEnemy mageEnemy)
                    {
                        mageEnemy.StopShooting();
                    }
                    // Jeśli przeciwnik to MageEnemy, zatrzymujemy jego strzelanie
                    if (enemy is SpiderEnemy spiderEnemy)
                    {
                        spiderEnemy.StopShooting();
                    }

                    canvas.Children.Remove(enemy.Visual);
                    enemies.Remove(enemy);
                }

                // Usuwamy pocisk
                canvas.Children.Remove(visual);
                timer.Stop();

                break; // Przerywamy pętlę, ponieważ pocisk został zniszczony
            }
        }
    }

}