using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WalkCalc
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор программы, инициализирующий все компоненты
        /// </summary>
        public MainWindow() => InitializeComponent();
        
        private double _speed = double.NaN;

        /// <summary>
        /// Обработчик события выбора скорости (нажатой кнопки)
        /// </summary>
        /// <param name="sender">Выбранная скорость (нажатая кнопка)</param>
        /// <param name="e">Дополнительные параметры события</param>
        private void Button_Speed_Click(object sender, RoutedEventArgs e)
        {
            Button butt = sender as Button;

            switch (butt.Name)
            {
                case "Button_Walk_Slow":
                    Radio_Walk_Slow.IsChecked = true;
                    _speed = 3.8;
                    break;
                case "Button_Walk_Calm":
                    Radio_Walk_Calm.IsChecked = true;
                    _speed = 5.2;
                    break;
                case "Button_Walk_Fast":
                    Radio_Walk_Fast.IsChecked = true;
                    _speed = 6.5;
                    break;
                case "Button_Run_Calm":
                    Radio_Run_Calm.IsChecked = true;
                    _speed = 10;
                    break;
                case "Button_Run_Fast":
                    Radio_Run_Fast.IsChecked = true;
                    _speed = 15;
                    break;
                case "Button_Speed_Custom":
                    Radio_Speed_Custom.IsChecked = true;
                    Custom_Speed_Box.IsEnabled = true;

                    // Обновить поле скорости на случай, если собственное значение было уже ранее записано.
                    _speed = double.TryParse(Custom_Speed_Box.Text, out double s) ? s : 0;
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// Обработчик события снятия флажка с радио кнопки пользовательского ввода
        /// </summary>
        /// <param name="sender">Радио кнопка со снятым флажком</param>
        /// <param name="e">Дополнительные параметры события</param>
        private void Radio_Speed_Custom_Unchecked(object sender, RoutedEventArgs e)
            => Custom_Speed_Box.IsEnabled = false;

        /// <summary>
        /// Предотвращает ввод недопустимых символов в поля
        /// </summary>
        /// <param name="sender">Поле, с которым имеют дело</param>
        /// <param name="e">Нажатая клавиша</param>
        private void Box_KeyDown(object sender, KeyEventArgs e)
            => e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);

        /// <summary>
        /// Записывает в поле _speed значение из поля ввода своей скорости после того, как поле вышло из фокуса
        /// </summary>
        /// <param name="sender">Поле собственной скорости</param>
        /// <param name="e">Дополнительные параметры события</param>
        private void Custom_Speed_Box_LostFocus(object sender, RoutedEventArgs e)
            => _speed = double.TryParse(Custom_Speed_Box.Text, out double s) ? s : 0;

        /// <summary>
        /// Обработчик события нажатия на кнопку вычисления, рассчитывающий и выводящий итоговую 
        /// информацию исходя из введённых данных
        /// </summary>
        /// <param name="sender">Экземпляр объекта кнопки "Рассчитать"</param>
        /// <param name="e">Дополнительные параметры события</param>
        private void Button_Calc_Click(object sender, RoutedEventArgs e)
        {
            // Использовать нулевую дистанцию, если значение поля недопустимо
            double dist = double.TryParse(Distance_Box.Text, out double d) ? d : 0;

            if (_speed > 0 && dist > 0)
            {
                double speed = _speed;

                // Проверяем, отмечена ли галочка "Не разделять разряды"
                bool dividerChecked = (bool)Divider.IsChecked;

                double time = dist / speed;
                // Если отмечена, то округлить значение часов с точностью до сотых. Если нет -
                // отбросить дробную часть
                double hours = dividerChecked ? Math.Round(time, 2) : Math.Truncate(time);

                double minutes = default;

                const int MINUTES = 60;

                if (!dividerChecked)
                    minutes = Math.Round(MINUTES * (time - hours));

                Resul.Content = "РЕЗУЛЬТАТ:";
                // Если значение часов или минут - 0, отбросить их в выводе результата
                // Игнорировать минуты, если выключен режим разделения разрядов
                THE_RESULT.Content = (hours > 0 ? $"{hours} часов " : "") +
                    (minutes != default(double) && minutes > 0 ? (dividerChecked  ? "" : $"{minutes} минут") : "");

                if (hours == 0 && minutes == 0)
                    THE_RESULT.Content = "меньше 1 минуты";
            }
        }

        /// <summary>
        /// Метод для проверки на то, числовая ли подаётся клавиша
        /// </summary>
        /// <param name="inKey">Клавиша, поступившая на вход</param>
        /// <returns>Возвращает true, если значение числовое. Иначе - false</returns>
        private bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Метод для проверки на то, служебаня ли подаётся клавиша (Delete, Backspace, Tab, Comma)
        /// </summary>
        /// <param name="inKey">Клавиша, поступившая на вход</param>
        /// <returns>Возвращает true, если клавиша служебная. Иначе - false</returns>
        private bool IsDelOrBackspaceOrTabKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Tab ||
                inKey == Key.OemComma;
        }

        /// <summary>
        /// Обработчик события ссылки "ОБ ИСПОЛЬЗОВАНИИ", который открывает информационное окно
        /// </summary>
        /// <param name="sender">Экземпляр объекта ссылки</param>
        /// <param name="e">Дополнительные параметры события</param>
        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            => new Window1().Show();
    }
}
