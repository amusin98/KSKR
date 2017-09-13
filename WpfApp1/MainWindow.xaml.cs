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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double[,] setka;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int rows = Convert.ToInt32(nI.Text) + 1;
            int cols = Convert.ToInt32(nR.Text) + 1;
            setka = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                setka[i, 0] = Convert.ToInt32(Tn.Text);
                setka[i, cols - 1] = Convert.ToInt32(Tk.Text);
            }
            double dx = Convert.ToInt32(L.Text) / Convert.ToInt32(nR.Text);
            double UL0 = Convert.ToInt32(Tk.Text);
            double U00 = Convert.ToInt32(Tn.Text);
            var x = dx;
            for (int i = 1; i < Convert.ToInt32(nR.Text); i++)
            {
                double u = x * (UL0 - U00) / Convert.ToInt32(L.Text) + U00;
                setka[rows - 1, i] = u;
                x += dx;
            }
            double gamma = (Convert.ToDouble(a.Text)*((Convert.ToDouble(tk.Text) - Convert.ToDouble(tn.Text))/Convert.ToDouble(nI.Text)))/(dx*dx);
            for (int i = Convert.ToInt32(nR.Text); i > 0; i--)
            {
                for (int j = 1; j < Convert.ToInt32(nI.Text); j++)
                {
                    double u = gamma * setka[i, j- 1] - (gamma - 1) * setka[i, j] + gamma * setka[i, j + 1];
                    setka[i - 1, j] = Math.Round(u, 3);
                }
            }
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    MessageBox.Show(setka[i, j].ToString());
                }
            }

        }
    }
}
