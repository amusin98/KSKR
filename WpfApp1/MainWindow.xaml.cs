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
        int rows, cols;
        double TN, TK, dx, UL0, U00, dt, A;
        public MainWindow()
        {
            
            InitializeComponent();
        }

        /// <summary>
        /// Инициализируем данные
        /// </summary>
        private void Initialize()
        {
            rows = Convert.ToInt32(nI.Text) + 1;
            cols = Convert.ToInt32(nR.Text) + 1;
            //Температура на лнвом конце стержня
            TN = Convert.ToInt32(Tn.Text);
            //Температура на правом конце стержня
            TK = Convert.ToInt32(Tk.Text);
            setka = new double[rows, cols];
            //Заполняем граничные условия
            for (int i = 0; i < rows; i++)
            {
                setka[i, 0] = TN;
                setka[i, cols - 1] = TK;
            }
            //Шаг сетки по x
            dx = Convert.ToInt32(L.Text) / Convert.ToInt32(nR.Text);
            //Температура на правом конце стержня в начальный момент времени
            UL0 = TK;
            //Температура в начальгый момент времени на левом конце стержня
            U00 = TN;
            var x = dx;
            //Коэффициент теплопроводности
            A = Convert.ToDouble(a.Text);
            //Временной интервал
            dt = (Convert.ToDouble(tk.Text) - Convert.ToDouble(tn.Text)) / Convert.ToInt32(nI.Text);
            //Заполняем матрицу решения начальными условиями
            for (int i = 1; i < Convert.ToInt32(nR.Text); i++)
            {
                double u = x * (UL0 - U00) / Convert.ToInt32(L.Text) + U00;
                setka[rows - 1, i] = u;
                x += dx;
            }
        }

        /// <summary>
        /// Неявная схема
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Initialize();
            //Неявная схема
            //Вычисляем коэффициенты для решения СЛАУ
            double K1 = -(A * A) / (dx * dx);
            double K2 = 1 / dt + 2 * A * A / (dx * dx);
            double K3 = K1;

            //Для каждого временного слоя наачиная со 2 составляем СЛАУ, и решая ее получаем значения температур на текущем слое
            for (int i = rows - 2; i >= 0; i--)
            {
                //создаем матрицу коэффициентов
                double[,] mas = new double[rows - 2, rows - 2];
                //Заполняем матрицу коэффициентов за исключением первой и последней строки
                for (int j = 1; j < rows - 3; j++)
                {
                    mas[j, j - 1] = K1;
                    mas[j, j] = K2;
                    mas[j, j + 1] = K3;
                }
                //Заполняем первую и последнюю строку матрицы 
                mas[0, 0] = K2;
                mas[0, 1] = K3;
                mas[rows - 3, rows - 4] = K1;
                mas[rows - 3, rows - 3] = K2;

                //создаем столбец свободных членов
                double[] matrixB = new double[cols - 2];
                //заполняем столбец свободных членов
                for (int j = 1; j < cols - 1; j++)
                {
                    matrixB[j - 1] = setka[i + 1, j] / dt;
                }
                //корректируем первый и последний элемент столбца свободных членов
                matrixB[0] -= K1 * setka[i, 0];
                matrixB[matrixB.Length - 1] -= K3 * setka[i, cols - 1];
                //решаем систему методом гаусса
                double[] solution = gauss(mas, matrixB);
                //дополняем временной слой недостающими значениями
                for (int j = 1; j < cols - 1; j++)
                {
                    setka[i, j] = solution[j - 1];
                }
            }
            
            Result res = new Result(setka);
            res.Show();
            res.Activate();

        }

        
        /// <summary>
        /// Явная схема
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Initialize();
            //явная схема
            //вычисляем коэффициент для упрощения расчетов
            double gamma = (A * A * dt) / (dx * dx);
            for (int i = Convert.ToInt32(nR.Text); i > 0; i--)
            {
                for (int j = 1; j < Convert.ToInt32(nI.Text); j++)
                {
                    double u = gamma * setka[i, j - 1] - (2.0 * gamma - 1) * setka[i, j] + gamma * setka[i, j + 1];
                    setka[i - 1, j] = u;
                }
            }
            Result res = new Result(setka);
            res.Show();
            res.Activate();
        }


        private double[] gauss(double[,] a, double[] b)
        {
            int n = b.Length;
            double[] x = new double[n];
            int k = 0, index = 0;
            while (k < n)
            {
                double max = Math.Abs(a[k, k]);
                index = k;
                for (int i = k + 1; i < n; i++)
                {
                    if (Math.Abs(a[i, k]) > max)
                    {
                        max = Math.Abs(a[i, k]);
                        index = i;
                    }
                }
                if (max < 0.000001)
                {
                    throw new Exception("No solution!");
                }
                for (int i = 0; i < n; i++)
                {
                    double temp = a[k, i];
                    a[k, i] = a[index, i];
                    a[index, i] = temp;
                }
                double tmp = b[k];
                b[k] = b[index];
                b[index] = tmp;
                for (int i = k; i < n; i++)
                {
                    double template = a[i, k];
                    if (Math.Abs(template) < 0.000001)
                    {
                        continue;
                    }
                    for (int j = 0; j < n; j++)
                    {
                        a[i, j] = a[i, j] / template;
                    }
                    b[i] = b[i] / template;
                    if (i == k)
                    {
                        continue;
                    }
                    for (int j = 0; j < n; j++)
                    {
                        a[i, j] = a[i, j] - a[k, j];
                    }
                    b[i] = b[i] - b[k];
                }
                k++;
            }
            for (k = n - 1; k >= 0; k--)
            {
                x[k] = b[k];
                for (int i = 0; i < k; i++)
                {
                    b[i] = b[i] - a[i, k] * x[k];
                }
            }
            return x;
        }
    }
}
