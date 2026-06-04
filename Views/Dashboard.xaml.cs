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
using PoderJudicial.Helpers;

namespace PoderJudicial.Views
{
    public partial class Dashboard : Window
    {
        public Dashboard(string usuario)
        {
            InitializeComponent();
            ActivarBoton(BtnConsultar);
            MainFrame.Navigate(new ConsultarRegistros());

            // Footer usuario
            txtAvatar.Text = usuario.Substring(0, 1).ToUpper();
            txtNombreUsuario.Text = usuario;
        }
        public Frame FramePrincipal => MainFrame;

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

            // Desactivar todos
            foreach (Button btn in botones)
            {
                btn.Background =
                    System.Windows.Media.Brushes.Transparent;

                btn.Foreground =
                    new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)
                        System.Windows.Media.ColorConverter.ConvertFromString("#8B92A5"));
            }

            // activar solo uno
            botonActivo.Background =
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString("#2A3147"));

            botonActivo.Foreground =
                new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)
                    System.Windows.Media.ColorConverter.ConvertFromString("#2ECC8F"));
        }

        // Cosultar
        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnConsultar);

            MainFrame.Navigate(new ConsultarRegistros());
        }

        // Nuevo
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnNuevo);

            MainFrame.Navigate(new NuevoRegistro());
        }

        // REPORTES
        private void BtnReportes_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnReportes);

             MainFrame.Navigate(new ReportesView());
        }

        // Config
        

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            SesionActual.Usuario = string.Empty;
            Login login = new Login();
            login.Show();

            
            this.Close();
        }


        private void BtnConfig_Click(object sender, RoutedEventArgs e)
        {
            BtnConfig.ContextMenu.PlacementTarget = BtnConfig;
            BtnConfig.ContextMenu.IsOpen = true;
        }


        private void ModoClaro_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CambiarTema("Light");
        }

        private void ModoOscuro_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CambiarTema("Dark");
        }

        private void ModoDescanso_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.CambiarTema("EyeCare");
        }


    }
}