using PoderJudicial.Data;
using PoderJudicial.Helpers;
using PoderJudicial.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

        private bool _submenuConsultasVisible = false;
        private Button _tablaSeleccionada = null;
        

        public Dashboard(string usuario)
        {
            InitializeComponent();

            MainFrame.Navigate(
    new ConsultarRegistros(
        TableDetector.TablaActual));

            ActivarBoton(BtnConsultar);

            CargarTablasBD();

            // Footer usuario
            txtAvatar.Text = usuario.Substring(0, 1).ToUpper();
            txtNombreUsuario.Text = usuario;
        }
        public Frame FramePrincipal => MainFrame;

        private void CargarTablasBD()
        {
            List<string> tablas = new();

            using (OleDbConnection conn =
                Conexion.ObtenerConexion())
            {
                conn.Open();

                DataTable schema =
                    conn.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string nombreTabla =
                        row["TABLE_NAME"].ToString();

                    // IGNORAR tablas del sistema
                    if (nombreTabla.StartsWith("MSys"))
                        continue;

                    // IGNORAR tablas temporales
                    if (nombreTabla.StartsWith("~"))
                        continue;

                    tablas.Add(nombreTabla);
                }
            }

            // Ordenar
            tablas = tablas
                .OrderByDescending(x => x)
                .ToList();

            PanelTablas.ItemsSource = tablas;
        }

        private void BtnTablaDinamica_Click(
    object sender,
    RoutedEventArgs e)
        {
            ActivarBoton(BtnConsultar);

            Button btn = (Button)sender;

            string nombreTabla =
                btn.Content.ToString();

            if (_tablaSeleccionada != null)
            {
                _tablaSeleccionada.Background =
                    Brushes.Transparent;

                _tablaSeleccionada.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(
                            "#B8C1D1"));
            }

            _tablaSeleccionada = btn;

            _tablaSeleccionada.Background =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        "#2A3147"));

            _tablaSeleccionada.Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        "#2ECC8F"));

            MainFrame.Navigate(
                new ConsultarRegistros(nombreTabla));
        }


        // ACTIVAR BOTÓN
        private void ActivarBoton(Button botonActivo)
        {
            // TODOS LOS BOTONES
            Button[] botones =
 {
    BtnConsultar,
    BtnNuevo,
    BtnCopias,
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
        private void BtnConsultar_Click(
    object sender,
    RoutedEventArgs e)
        {
            _submenuConsultasVisible =
                !_submenuConsultasVisible;

            PanelTablas.Visibility =
                _submenuConsultasVisible
                ? Visibility.Visible
                : Visibility.Collapsed;

            TxtFlechaConsultar.Text =
                _submenuConsultasVisible
                ? "▲"
                : "▼";
        }

        // Nuevo
        private void BtnNuevo_Click(object sender, RoutedEventArgs e)
        {
            ActivarBoton(BtnNuevo);

            if (_tablaSeleccionada != null)
            {
                _tablaSeleccionada.Background =
                    Brushes.Transparent;

                _tablaSeleccionada.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(
                            "#B8C1D1"));
            }

            MainFrame.Navigate(new NuevoRegistro());
        }
        // Copias
        private void BtnCopias_Click(
    object sender,
    RoutedEventArgs e)
        {
            ActivarBoton(BtnCopias);

            if (_tablaSeleccionada != null)
            {
                _tablaSeleccionada.Background =
                    Brushes.Transparent;

                _tablaSeleccionada.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(
                            "#B8C1D1"));

                _tablaSeleccionada = null;
            }

            MainFrame.Navigate(
                new RegistroCopias());
        }

        // REPORTES
        private void BtnReportes_Click(
    object sender,
    RoutedEventArgs e)
        {
            ActivarBoton(BtnReportes);

            if (_tablaSeleccionada != null)
            {
                _tablaSeleccionada.Background =
                    Brushes.Transparent;

                _tablaSeleccionada.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(
                            "#B8C1D1"));

                _tablaSeleccionada = null;
            }

            MainFrame.Navigate(
                new ReportesView());
        }

        // Config


        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            SesionActual.Usuario = string.Empty;
            Login login = new Login();
            login.Show();

            
            this.Close();
        }


        private void BtnConfig_Click(
    object sender,
    RoutedEventArgs e)
        {
            ActivarBoton(BtnConfig);

            if (_tablaSeleccionada != null)
            {
                _tablaSeleccionada.Background =
                    Brushes.Transparent;

                _tablaSeleccionada.Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(
                            "#B8C1D1"));

                _tablaSeleccionada = null;
            }

            BtnConfig.ContextMenu.PlacementTarget =
                BtnConfig;

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