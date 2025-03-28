using Space_Intruder.Class;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Space_Intruder.GameObjects
{
    public class Hero
    {
        public Rectangle Visual { get; private set; }

        public UIElement PlayerVisual => Visual;
        public double PositionX { get; private set; }
        public double PositionY { get; private set; }
        public double Width => Visual.Width;
        public double Height => Visual.Height;
        public int Lives { get; set; }
        public bool IsAlive => Lives > 0;

        // Stats
        public double _movementSpeed = 20;
        private int _damage = 5;
        private double _attackSpeed = 10; // attacks per second
        private DispatcherTimer _attackTimer;
        private bool _canAttack = true;

        // Animation
        private readonly Storyboard _moveStoryboard;
        private readonly DoubleAnimation _moveAnimation;

        public Hero(Canvas gameCanvas, double initialX, double initialY)
        {
            Visual = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Images/boss.png")))
            };

            PositionX = initialX;
            PositionY = initialY;
            Lives = 3;

            gameCanvas.Children.Add(Visual);
            Canvas.SetLeft(Visual, PositionX);
            Canvas.SetBottom(Visual, PositionY);

            // Initialize movement animation
            _moveStoryboard = new Storyboard();
            _moveAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };
            Storyboard.SetTarget(_moveAnimation, Visual);
            Storyboard.SetTargetProperty(_moveAnimation, new PropertyPath("(Canvas.Left)"));
            _moveStoryboard.Children.Add(_moveAnimation);

            // Initialize attack timer
            _attackTimer = new DispatcherTimer();
            _attackTimer.Interval = TimeSpan.FromSeconds(1 / _attackSpeed);
            _attackTimer.Tick += (s, e) => _canAttack = true;
        }

        public void MoveLeft(double gameAreaLeft)
        {
            if (PositionX > gameAreaLeft)
            {
                MoveTo(PositionX - _movementSpeed);
            }
        }

        public void MoveRight(double gameAreaRight)
        {
            if (PositionX < gameAreaRight - Width)
            {
                MoveTo(PositionX + _movementSpeed);
            }
        }

        private void MoveTo(double newX)
        {
            _moveStoryboard.Stop();
            _moveAnimation.To = newX;
            _moveStoryboard.Begin();
            PositionX = newX;
        }

        public void Shoot(Canvas gameCanvas, List<Enemy> enemies)
        {
            if (!_canAttack || !IsAlive) return;

            double bulletX = PositionX + Width / 2;
            double bulletY = PositionY + Height;

            var bullet = new Pocisk(
                "bohater",
                _damage,
                bulletX,
                bulletY,
                gameCanvas,
                enemies,
                Visual
            );

            _canAttack = false;
            _attackTimer.Start();
        }

        public void TakeDamage()
        {
            Lives--;
            if (!IsAlive)
            {
                Visual.Visibility = Visibility.Collapsed;
            }
        }

        public void ApplySlowEffect(double duration = 2.0, double slowFactor = 0.5)
        {
            double originalSpeed = _movementSpeed;
            _movementSpeed *= slowFactor;

            var restoreSpeedTimer = new DispatcherTimer();
            restoreSpeedTimer.Interval = TimeSpan.FromSeconds(duration);
            restoreSpeedTimer.Tick += (s, e) =>
            {
                _movementSpeed = originalSpeed;
                restoreSpeedTimer.Stop();
            };
            restoreSpeedTimer.Start();
        }

        public void UpgradeStat(string stat, double value)
        {
            switch (stat.ToLower())
            {
                case "damage":
                    _damage += (int)value;
                    break;
                case "attackspeed":
                    _attackSpeed += value;
                    _attackTimer.Interval = TimeSpan.FromSeconds(1 / _attackSpeed);
                    break;
                case "movementspeed":
                    _movementSpeed += value;
                    break;
                case "health":
                    Lives += (int)value;
                    break;
            }
        }

    }
}