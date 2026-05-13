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


namespace PoderJudicial.Views
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            
            MainFrame.Navigate(new PoderJudicial.Views.ConsultarRegistros());

        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PoderJudicial.Views.ConsultarRegistros());

        }

        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

