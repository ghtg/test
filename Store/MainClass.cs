using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace Store
{
    internal class MainClass
    {
        public string connectionString = GetConnectionStringFromEDM();
        public static string GetConnectionStringFromEDM()
        {
            using (var context = new JewelryMilenkovEntities())
            {
                var entityConnection = context.Database.Connection;
                return entityConnection.ConnectionString;
            }
        }

        public void LoadDataIntoDataGrid(string sqlQuery, System.Windows.Controls.DataGrid dataGrid)
        {
            // Create a SqlConnection object with the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlCommand object with the SQL query and the SqlConnection
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Create a SqlDataAdapter to retrieve the data from the SQL query
                    SqlDataAdapter adapter = new SqlDataAdapter(command);

                    // Create a DataTable to hold the retrieved data
                    DataTable dataTable = new DataTable();

                    // Open the database connection
                    connection.Open();

                    // Fill the DataTable with the data from the SqlDataAdapter
                    adapter.Fill(dataTable);

                    // Set the DataTable as the ItemsSource for the DataGrid
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
        }

        public void ExecuteQuery(string sqlQuery)
        {
            // Create a SqlConnection object with the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlCommand object with the SQL query and the SqlConnection
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Open the database connection
                    connection.Open();

                    // Execute the query
                    command.ExecuteNonQuery();
                }
            }
        }

        public string ExecuteQueryAndGetString(string sqlQuery)
        {
            // Create a SqlConnection object with the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlCommand object with the SQL query and the SqlConnection
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Open the database connection
                    connection.Open();

                    // Execute the query and retrieve the result as a string
                    string result = command.ExecuteScalar()?.ToString();

                    // Return the result
                    return result;
                }
            }
        }

        public byte[] ExecuteQueryAndGetBytes(string sqlQuery)
        {
            // Create a SqlConnection object with the connection string
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Create a SqlCommand object with the SQL query and the SqlConnection
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // Open the database connection
                    connection.Open();

                    byte[] result = new byte[0];
                    if (!(command.ExecuteScalar() is DBNull))
                    {
                        // Execute the query and retrieve the result as binary data
                        result = (byte[])command.ExecuteScalar();
                    }

                    // Return the result
                    return result;
                }
            }
        }

        public void UpdateImages(string directoryPath)
        {
            JewelryMilenkovEntities ent = new JewelryMilenkovEntities();

            foreach (var good in ent.Goods)
            {
                string path = Path.Combine(directoryPath, good.Name + ".jpg");
                if (File.Exists(path))
                {
                    byte[] imageInBytes = File.ReadAllBytes(path);
                    good.Image = imageInBytes;
                }
                else
                {
                    Console.WriteLine(good.Name);
                }
            }

            ent.SaveChanges();
        }

        public void UpdateGoodImageFromFileDialog(int Id)
        {
            JewelryMilenkovEntities ent = new JewelryMilenkovEntities();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                string selectedFileName = Path.GetFileNameWithoutExtension(selectedFilePath);

                var targetGood = ent.Goods.FirstOrDefault(g => g.Id == Id);

                if (targetGood != null)
                {
                    byte[] imageInBytes = System.IO.File.ReadAllBytes(selectedFilePath);
                    targetGood.Image = imageInBytes;
                }

                ent.SaveChanges();
            }
        }

        public void LoadImageIntoImageBox(byte[] imageData, System.Windows.Controls.Image imageBox)
        {
            if (imageData != null && imageData.Length > 0)
            {
                using (MemoryStream memoryStream = new MemoryStream(imageData))
                {
                    try
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(memoryStream);
                        
                        // Create a new Bitmap with the same width and height as the original Image
                        Bitmap bitmap = new Bitmap(image.Width, image.Height);

                        // Draw the original Image onto the new Bitmap
                        using (Graphics graphics = Graphics.FromImage(bitmap))
                        {
                            graphics.DrawImage(image, 0, 0);
                        }

                        imageBox.Source = BitmapToImageSource(bitmap);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message);
                    }
                }
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public void PopulateComboBoxByQuery(string query, System.Windows.Controls.ComboBox comboBox)
        {
            comboBox.Items.Clear();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
    
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
    
                    while (reader.Read())
                    {
                        comboBox.Items.Add(reader.GetString(0));
                    }
    
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                //do nothing
            }
        }
    }
}
