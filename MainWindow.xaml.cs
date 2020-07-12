using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Stormotion_test_task
{
    
    public partial class MainWindow : Window
    {
        private bool _isGameStarted = false;
        private int _userScore = 0;
        private int _aiScore = 0;
        private static readonly Regex _regex = new Regex("[^0-9]+");

        public MainWindow()
        {
            InitializeComponent();
        }

        //Описание хода игрока
        public void HumaneTurn()
        {
            try
            {
                _isGameStarted = true;
                nChoose.IsEnabled = false;
                mChoose.IsEnabled = false;
                FirstTurn.IsEnabled = false;

                int userInput = int.Parse(Number.Text);
                int countUser = int.Parse(CountUser.Text);
                _userScore = userInput + countUser;
                CountUser.Text = _userScore.ToString();

                TotalNumber.Text = (int.Parse(TotalNumber.Text) - userInput).ToString();

                HistoryUser.Text = $"{HistoryUser.Text}  {userInput}";

            }
            catch (Exception exception)
            {
                Debug.Print(exception.Message);
                return;
            }


        }

        //Описание хода компьютера с расчетом на победу
        public void ComputerTurn()
        {
            int total = int.Parse(TotalNumber.Text);
            int m = int.Parse(mChoose.Text);
            int countComputer = int.Parse(CountComputer.Text);
            int countAi = 0;

            if (total <= 0)
            {
                return;
            }

            Turn.Text = "Ход компьютера!";

            _isGameStarted = true;
            nChoose.IsEnabled = false;
            mChoose.IsEnabled = false;
            FirstTurn.IsEnabled = false;

            Random random = new Random();

            if (m%2 == 0)
            {
                for (int i = 1; i <= m; i++)
                {
                    if ((_aiScore + i) % 2 == 0 && ((total - i) % (m + 2) == 0 || (total - i) % (m + 2) == 1))
                    {
                        countAi = i;
                        break;
                    }
                    if ((_aiScore + i) % 2 != 0 && (total - i) % (m + 2) == (m + 1))
                    {
                        countAi = i;
                        break;
                    }

                    countAi = random.Next(1, m);
                }
            }
            else //if (m%2 != 0)
            {
                for (int i = 1; i <= m; i++)
                {
                    if ((_aiScore + i) % 2 == 0 && ((total - i) % (2*m + 2) == 0 | (total - i) % (2*m + 2) == 1))
                    {
                        countAi = i;
                        break;
                    }
                    if ((_aiScore + i) % 2 != 0 && ((total - i) % (2*m + 2) == (m + 1) | (total - i) % (2*m + 2) == (m + 2)))
                    {
                        countAi = i;
                        break;
                    }

                    countAi = random.Next(1, m);
                }
            }
            
            _aiScore = countComputer + countAi;

            CountComputer.Text = _aiScore.ToString();
            TotalNumber.Text = (int.Parse(TotalNumber.Text) - countAi).ToString();

            HistoryAI.Text = $"{HistoryAI.Text}  {countAi}";

            Turn.Text = "Ваш ход!";
        }

        //Новая игра
        private void NewGame_OnClick(object sender, RoutedEventArgs e)
        {
            int total = 2 * int.Parse(nChoose.Text) + 1;
            TotalNumber.Text = total.ToString();
            _userScore = 0;
            _aiScore = 0;

            Turn.Text = FirstTurn.IsChecked == true ? "Ваш ход!" : "Ход компьютера!";
            
            CountUser.Text = "0";
            CountComputer.Text = "0";
            HistoryUser.Text = "";
            HistoryAI.Text = "";

            Start.IsEnabled = true;
            Number.IsEnabled = true;
            FirstTurn.IsEnabled = true;

            if (ExtendedVersion.IsChecked == true)
            {
                nChoose.IsEnabled = true;
                mChoose.IsEnabled = true;
            }
        }

        private void FirstTurn_OnChecked(object sender, RoutedEventArgs e)
        {
            Turn.Text = "Ваш ход!";
            Start.Visibility = Visibility.Collapsed;
        }

        private void FirstTurn_OnUnchecked(object sender, RoutedEventArgs e)
        {
            Turn.Text = "Ход компьютера!";
            Start.Visibility = Visibility.Visible;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            FirstTurn.IsChecked = true;
            UserChoice.Text = $"Возьмите от 1 до {int.Parse(mChoose.Text)} спичек";
        }

        //Проверка пользовательского ввода
        private void Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (Turn.Text == "Ход компьютера!")
                {
                    MessageBox.Show("Сейчас не ваш ход! Подождите!");
                    return;
                }

                int choice = int.Parse(Number.Text);
                int total = int.Parse(TotalNumber.Text);

                if ((choice < 1 | choice > int.Parse(mChoose.Text)) || choice > total)
                {
                    MessageBox.Show("Проверьте правильность ввода!");
                    Number.Focus();
                    Number.SelectAll();
                    return;
                }

                HumaneTurn();
                ComputerTurn();
                Number.SelectAll();
            }

            if (int.Parse(TotalNumber.Text).Equals(0))
            {
                TotalNumber.Text = _aiScore % 2 == 0 ? "Вы проиграли!" : "Вы выиграли!";
                _isGameStarted = false;
                Number.IsEnabled = false;
            }
        }

        //Начать ход ИИ при его первом ходе
        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            ComputerTurn();
            Start.IsEnabled = false;
        }

        //Проверка ввода параметра n
        private void NChoose_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }

            if (nChoose.Text == "")
            {
                TotalNumber.Text = "0";
                return;
            }

            try
            {
                int total = 2 * int.Parse(nChoose.Text) + 1;
                TotalNumber.Text = total.ToString();
            }
            catch (Exception exception)
            {
                Debug.Print(exception.Message);
                return;
            }
        }


        private void MChoose_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            
            try
            {
                UserChoice.Text = $"Возьмите от 1 до {int.Parse(mChoose.Text)} спичек";
            }
            catch (Exception exception)
            {
                Debug.Print(exception.Message);
                return;
            }
           
        }

        private void ExtendedVersion_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_isGameStarted || !Number.IsEnabled)
            {
                return;
            }
            nChoose.IsEnabled = true;
            mChoose.IsEnabled = true;
        }

        private void ExtendedVersion_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (_isGameStarted || !Number.IsEnabled)
            {
                return;
            }
            nChoose.IsEnabled = false;
            mChoose.IsEnabled = false;
        }

        //Ограничение ввода в TextBox'ы
        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == " ")
            {
                e.Handled = true;
                return;
            }

            e.Handled = _regex.IsMatch(e.Text);
        }
    }
}
