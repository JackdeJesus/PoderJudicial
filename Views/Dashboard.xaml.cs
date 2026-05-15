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
using PoderJudicial.Views;

namespace PoderJudicial.Views
{
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();

            // BOTÓN INICIAL ACTIVO
            ActivarBoton(BtnConsultar);

            // PÁGINA INICIAL
            MainFrame.Navigate(new ConsultarRegistros());
        }

        // ACTIVAR BOTÓN
        private void ActivarBoton(Button botonActivo)
        {
            // TODOS LOS BOTONES
            Button[] botones =
            {
                BtnConsultar,
                BtnNuevo,
                BtnReportes,
                BtnConfig
            };

            // DESACTIVAR TODOS
            foreach (Button btn in botones)
            {
                btn.Background =
                    System.Windows.Media.Brushes.Transparent;

                btn.Foreground =
                    new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)
                        System.Windows.Media.ColorConverter.ConvertFromString("#8B92A5"));
            }

            // ACTIVAR SOLO UNO
            botonActivo.Background =
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString("#2A3147"));

            botonActivo.Foreground =
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString("#2ECC8F"));
        }

        // CONSULTAR
        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnConsultar);

            MainFrame.Navigate(new ConsultarRegistros());
        }

        // NUEVO
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnNuevo);

            MainFrame.Navigate(new NuevoRegistro());
        }

        // REPORTES
        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnReportes);

            // MainFrame.Navigate(new Reportes());
        }

        // CONFIG
        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnConfig);

            // MainFrame.Navigate(new Configuracion());
        }
    }
}