using System;
using System.Windows;
using System.Windows.Controls;
using PoderJudicial.Views;
using System.Windows.Media.Imaging;


namespace PoderJudicial.Views
{
    /// <summary>
    /// </summary>
    public partial class Login : Window
    {
        bool mostrando = false;

        public Login()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, RoutedEventArgs e)
        {

            Dashboard dashboard = new Dashboard();

            dashboard.Show();

            this.Close();
        }

        // mostrar y ocultar contraseña
        private void btnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (!mostrando)
            {
                //mostrar contraseña
                passVisible.Text = passOculta.Password;

                passVisible.Visibility = Visibility.Visible;
                passOculta.Visibility = Visibility.Collapsed;

                imgOjo.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Resources/eye.png"));

                mostrando = true;
            }
            else
            {
                //ocultar contraseña
                passOculta.Password = passVisible.Text;

                passVisible.Visibility = Visibility.Collapsed;
                passOculta.Visibility = Visibility.Visible;

                imgOjo.Source = new BitmapImage(
                    new Uri("pack://application:,,,/Resources/eyeClose.png"));

                mostrando = false;
            }

            ActualizarPlaceholderPassword();
        }

        //cuadrito letra usuario
        private void txtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPlaceholderUsuario.Visibility =
                string.IsNullOrWhiteSpace(txtUsuario.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        //cuadrito letra password visible
        private void passVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarPlaceholderPassword();
        }

        // cuadrito letra  PasswordBox
        private void passOculta_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ActualizarPlaceholderPassword();
        }

        // Método reutilizable
        private void ActualizarPlaceholderPassword()
        {
            string textoPassword = mostrando
                ? passVisible.Text
                : passOculta.Password;

            txtPlaceholderPassword.Visibility =
                string.IsNullOrWhiteSpace(textoPassword)
                ? Visibility.Visible
                : Visibility.Hidden;
        }
    }
}