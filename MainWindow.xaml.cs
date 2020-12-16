using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IS_Lab1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel.VM_IS vm_is = new ViewModel.VM_IS();
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private Models.Question question = null;

        public MainWindow()
        {
            InitializeComponent();
            //var db = new Models.Database();
            //db.Parse();
            //db.Save();
            //db.GetCSRFToken();
            //db.GetPage(1, 0, 25000, 6);
            //db.GetCharacters("2b3d20cb-4645-11e1-ac39-001517c526f0", null);
            //SetQuestion(vm_is.Question);

        }

        private bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Next() 
        {
            if(question.Next != null) 
            {
                question = question.Next;
                SetQuestion(question);
                return;
            }
            //TBQuestion.Text = "Результат...";
            //TextEnd.Text = "Ваша стриальная машина: " + vm_is.GetWasher();
            //SetLayerEnd();
            SetLayerEnd();
            itemResult.ItemsSource = vm_is.GetOffers();
            LayerOffers.IsEnabled = true;
            LayerOffers.Visibility = Visibility.Visible;
            this.Width = 800;
            this.Height = 600;
        }

        private void SetQuestion(Models.Question q) 
        {
            question = q;
            TBQuestion.Text = question.Value;
            if (q.Type == typeof(Models.Answer))
            {
                SPAnswer.Children.Clear();
                foreach(var ans in q.Answers) 
                {
                    var button = new Button()
                    {
                        Content = ans.Value,
                        Margin = new Thickness(20,10,20,10)
                    };
                    button.Click += Button_Click_Answer;
                    button.Tag = ans;
                    SPAnswer.Children.Add(button);
                }
            }
            SetLayer(q);
        }

        private void SetLayer(Models.Question q) 
        {
            if (q.Type == typeof(bool)) SetLayerBool();
            if (q.Type == typeof(Models.Answer))  SetLayerAnswer();
            if (q.Type == typeof(int)) SetLayerInt();
        }

        private void SetLayerBool() 
        {
            LayerAnswer.IsEnabled = false;
            LayerAnswer.Visibility = Visibility.Hidden;
            LayerInt.IsEnabled = false;
            LayerInt.Visibility = Visibility.Hidden;
            LayerBool.IsEnabled = true;
            LayerBool.Visibility = Visibility.Visible;
        }
        private void SetLayerAnswer()
        {
            LayerInt.IsEnabled = false;
            LayerInt.Visibility = Visibility.Hidden;
            LayerBool.IsEnabled = false;
            LayerBool.Visibility = Visibility.Hidden;
            LayerAnswer.IsEnabled = true;
            LayerAnswer.Visibility = Visibility.Visible;
        }
        private void SetLayerInt()
        {
            LayerBool.IsEnabled = false;
            LayerBool.Visibility = Visibility.Hidden;
            LayerAnswer.IsEnabled = false;
            LayerAnswer.Visibility = Visibility.Hidden;
            LayerInt.IsEnabled = true;
            LayerInt.Visibility = Visibility.Visible;
        }
        private void SetLayerEnd()
        {
            LayerBool.IsEnabled = false;
            LayerBool.Visibility = Visibility.Hidden;
            LayerAnswer.IsEnabled = false;
            LayerAnswer.Visibility = Visibility.Hidden;
            LayerInt.IsEnabled = false;
            LayerInt.Visibility = Visibility.Hidden;
        }

        private void Button_Click_True(object sender, RoutedEventArgs e)
        {
            question.ValueBoolean = true;
            question.SetValue();
            Next();
        }

        private void Button_Click_False(object sender, RoutedEventArgs e)
        {
            question.ValueBoolean = false;
            question.SetValue();
            Next();
        }

        private void Button_Click_Answer(object sender, RoutedEventArgs e)
        {
            var ans = (sender as Button).Tag as Models.Answer;
            if(ans == null)
            {
                MessageBox.Show("Ошибка приведение типов!");
                return;
            }
            ans.Action(question);
            Next();
        }

        private void Button_Click_Int(object sender, RoutedEventArgs e)
        {
            var str = TextNumber.Text;
            if (!IsTextAllowed(str) || str =="") 
            {
                MessageBox.Show("Введено не число!");
                return;
            }
            question.ValueInt = Int32.Parse(str);
            if (question.ValueInt < 1)
            {
                MessageBox.Show("Кол-во человек не может быть меньше 1!");
                return;
            }
            question.SetValue();
            Next();
        }

        private void Button_Click_Parse(object sender, RoutedEventArgs e)
        {
            var db = vm_is.Database;
            db.Parse();
            db.Save();
            LayerStart.IsEnabled = false;
            LayerStart.Visibility = Visibility.Hidden;
            TBCntWasher.Text = "В базе " + db.Offers.Count + " стиралльных машин";
            SetQuestion(vm_is.Question);
        }

        private void Button_Click_Load(object sender, RoutedEventArgs e)
        {
            var db = vm_is.Database;
            db.Load();
            LayerStart.IsEnabled = false;
            LayerStart.Visibility = Visibility.Hidden;
            TBCntWasher.Text = "В базе " + db.Offers.Count + " стиралльных машин";
            SetQuestion(vm_is.Question);
        }
    }
}
