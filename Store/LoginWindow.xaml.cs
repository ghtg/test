using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Store
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public MainWindow mainWindow { get; set; }
        public int userState { get; set; }
        public string userLogin { get; set; }
        MainClass mainClass = new MainClass();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text;

            string userId = mainClass.ExecuteQueryAndGetString("SELECT TOP (1) [Id] FROM [Users] where Login like '"+login.ToString()+"';");
            if (userId == String.Empty || userId == null)
                MessageBox.Show("Пользователь с таким логином не найден.");
            else
            {
                userLogin = mainClass.ExecuteQueryAndGetString("SELECT TOP (1) Login FROM [Users] where Login like '" + login.ToString() + "';");
                userState = int.Parse(mainClass.ExecuteQueryAndGetString("SELECT TOP (1) UserStateId FROM [Users] where Login like '" + login.ToString() + "';"));

                mainWindow.userState = this.userState;
                mainWindow.userLogin = this.userLogin;
                mainWindow.LoginButton.Width = 0;

                if (userState == 3) //покупатель
                {
                    //TODO: lock admin and manager buttons
                }    

                this.Hide();
            }
        }
    }
}
