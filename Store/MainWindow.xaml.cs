using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Store
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainClass mainClass = new MainClass();
        LoginWindow loginWindow = new LoginWindow();

        public int userState { get; set; }
        public string userLogin { get; set; }
        string[] cart = new string[0];
        string loadedDB = null;

        public MainWindow()
        {
            InitializeComponent();
            LoginButton.Width = 798;
            loginWindow.Hide();
            loginWindow.mainWindow = this;

            LoadGoods();
        }

        private void LoadInventory_Click(object sender, RoutedEventArgs e)
        {
            LoadGoods();
        }

        private void LoadGoods()
        {
            mainClass.LoadDataIntoDataGrid("SELECT TOP (1000) [Id] as Номер ,[Name] as Название ,[Price] as Цена ,[Discount] as Скидка ,[Article] as Артикул ,[Description] as Описание FROM [Goods]", MainDataGrid);
            loadedDB = "Goods";
        }

        private void LoadOrders()
        {
            mainClass.LoadDataIntoDataGrid("SELECT TOP (1000) [Order].[Id] as 'Номер заказа', [Login] as Пользователь, OrderState.Name as Статус FROM [Order] join Users on Users.Id = UserId join OrderState on OrderState.Id = OrderStateId;", MainDataGrid);
            loadedDB = "Orders";
        }

        private void CreateNewOrder(int userId)
        {
            mainClass.ExecuteQuery("INSERT INTO [Order] (UserId, OrderStateId) VALUES ("+userId.ToString()+", 0)");
        }

        private void ChangeOrderState(int orderId, int orderStateId)
        {
            mainClass.ExecuteQuery("update [Order] set OrderStateId = "+orderStateId.ToString()+" where [Order].Id like '"+orderId.ToString()+"'");
        }

        private void LoadOrders_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            loginWindow.Show();
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedCells.Count > 0 && loadedDB == "Goods")
                mainClass.UpdateGoodImageFromFileDialog(int.Parse(GetCell(0)));
        }

        private void GetHighlightedRow()
        {
            // Get the selected item (highlighted row)
            var selectedItem = MainDataGrid.SelectedItem;

            // Check if an item is selected
            if (selectedItem != null)
            {
                Console.WriteLine(selectedItem.ToString());
            }
        }

        private string GetCell(int cell)
        {
            // Get the selected cells
            var selectedCells = MainDataGrid.SelectedCells;

            // Check if any cells are selected
            if (selectedCells.Count > 0)
            {
                // Get the first selected cell
                var firstSelectedCell = selectedCells[0];

                // Access the value of the first cell
                var dataRowView = firstSelectedCell.Item as DataRowView;
                if (dataRowView != null)
                {
                    var firstCellValue = dataRowView.Row[cell];

                    // Print the value
                    return firstCellValue.ToString();
                }
            }
            return null;
        }

        private void MainDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mainClass.LoadImageIntoImageBox(mainClass.ExecuteQueryAndGetBytes("SELECT TOP (1) [Image] FROM [Goods] where Id like '"+GetCell(0)+"'"), PictureBox);
            
            if (loadedDB == "Orders")
            {
                mainClass.PopulateComboBoxByQuery("SELECT OrderState.Name FROM OrderState", OrderStateComboBox);

                FindAndSelectItem(GetCell(2), OrderStateComboBox);
            }
        }

        public void FindAndSelectItem(string searchString, ComboBox comboBox)
        {
            foreach (var item in comboBox.Items)
            {
                if (item.ToString() == searchString)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string productId = GetCell(0);
            string[] output = null;
            bool isChanged = false;

            if (cart.Length>0)
                for (int i = 0; i < cart.Length; i++)
                {
                    output = cart[i].Split('|'); // 0|1
                    // output[0] = 0 -товар
                    // output[1] = 1 -количество

                    if (productId == output[0])
                    {
                        output[1] = (int.Parse(output[1]) + 1).ToString();
                        cart[i] = string.Join("|", output);
                        isChanged = true;
                        break;
                    }
                }

            if (isChanged == false)
            {
                cart = IncreaseCartSize();
                cart[cart.Length - 1] = productId + "|" + 1;
            }

            DisplayCart();
        }

        private string GetCart()
        {
            string output = null;
            for (int i = 0; i < cart.Length; i++)
            {
                output += cart[i] + "\n";
            }

            return output;
        }

        private void ClearCart()
        {
            cart = new string[0];
            CartDataGrid.ItemsSource = null;
        }

        private string[] IncreaseCartSize()
        {
            string[] newCart = new string[cart.Length + 1];
            for (int i = 0; i < cart.Length; i++)
            {
                newCart[i] = cart[i];
            }
            
            return newCart;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            string productId = GetCell(0);
            string[] output = null;
            for (int i = 0; i < cart.Length; i++)
            {
                output = cart[i].Split('|'); // 0|1
                // output[0] = 0 -товар
                // output[1] = 1 -количество

                if (productId == output[0] && int.Parse(output[1]) >= 1)
                {
                    if (int.Parse(output[1]) - 1 <= 0)
                    {
                        cart[i] = null;
                        cart = cart.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    }
                    else
                    {
                        output[1] = (int.Parse(output[1]) - 1).ToString();
                        cart[i] = string.Join("|", output);
                    }

                    break;
                }
            }

                DisplayCart();
        }

        public class LocalCart
        {
            public int Номер { get; set; }
            public int Количество { get; set; }
        }


        void DisplayCart()
        {
            List<LocalCart> dataList = new List<LocalCart>();

            string inputText = GetCart();

            if (inputText == null)
            {
                CartDataGrid.ItemsSource = null;
                CheckLabel.Content = "Чек: 0";
                return;
            }
            string[] lines = inputText.Split('\n');

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 2 && int.TryParse(parts[0], out int column1Value) && int.TryParse(parts[1], out int column2Value))
                {
                    dataList.Add(new LocalCart { Номер = column1Value, Количество = column2Value });
                }
            }
            CartDataGrid.ItemsSource = dataList;
            UpdateCheck();
        }

        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            ClearCart();
        }

        private void UpdateCheck()
        {
            float check = 0;
            string[] item = null;

            for (int i = 0; i < cart.Length; i++)
            {
                item = cart[i].Split('|'); // 0|1
                check += float.Parse(mainClass.ExecuteQueryAndGetString("SELECT [Price] FROM [JewelryMilenkov].[dbo].[Goods] where Goods.Id = " + int.Parse(item[0]))) * int.Parse(item[1]);
            }
            CheckLabel.Content = "Чек: " + check.ToString();
        }

        private void ChangeOrderStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (loadedDB == "Orders")
            {
                string stateId = mainClass.ExecuteQueryAndGetString("select OrderState.Id from OrderState where [OrderState].Name like '" + OrderStateComboBox.SelectedValue.ToString() + "'");
                if (stateId != null)
                    mainClass.ExecuteQuery("update [Order] set OrderStateId = " + stateId + " where [Order].Id like '" + GetCell(0) + "'");
                LoadOrders();
            }
        }
    }
}
