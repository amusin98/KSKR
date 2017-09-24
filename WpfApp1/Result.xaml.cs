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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Result.xaml
    /// </summary>
    public partial class Result : Window
    {
        double[,] setka { get; set; }
        public Result(double[,] setka)
        {
            InitializeComponent();
            DataTable dt = new DataTable();
            this.setka = setka;
            for (int j = 0; j < setka.GetLength(1); j++)
                dt.Columns.Add(new DataColumn("Column " + j.ToString()));

            for (int i = 0; i < setka.GetLength(0); i++)
            {
                var newRow = dt.NewRow();
                for (int j = 0; j < setka.GetLength(1); j++)
                    newRow["Column " + j.ToString()] = setka[i, j];
                dt.Rows.Add(newRow);
            }
            this.solution.ItemsSource = dt.DefaultView;
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
