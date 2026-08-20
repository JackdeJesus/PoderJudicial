﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PoderJudicial.Data;
using PoderJudicial.Helpers;

namespace PoderJudicial.Views
{
    public partial class Login : Window
    {
        bool mostrando = false;

        public Login()
        {
            InitializeComponent();
        }

        private async void btnIngresar_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string password = mostrando ? passVisible.Text : passOculta.Password;

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Ingresa usuario y contraseña.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var repo = new UserRepository();
            bool acceso = repo.Login(usuario, password);

            if (acceso)
            {
                SesionActual.Usuario = usuario;

                // El login usa SQLite (UserRepository), no depende de Access.
                // Antes de abrir Dashboard se valida la BD configurable.
                if (!GarantizarBaseDeDatosConfigurada())
                    return;

                // El respaldo se ejecuta de forma asíncrona y con manejo
                // interno de errores. Si la red, permisos o Access impiden
                // respaldar, la aplicación continúa normalmente.
                await RespaldoBaseDatosService.VerificarYCrearRespaldoAsync();

                Dashboard dashboard = new Dashboard(usuario);
                dashboard.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.",
                    "Acceso denegado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool GarantizarBaseDeDatosConfigurada()
        {
            if (!Conexion.EstaConfigurada)
            {
                var ventana =
                    new ConfiguracionBaseDatos(
                        permiteCancelar: false);

                return ventana.ShowDialog() == true;
            }

            string error =
                Conexion.ProbarConexion(Conexion.RutaBD);

            if (error == null)
                return true;

            var ventanaError =
                new ConfiguracionBaseDatos(
                    mensajeError:
                        $"No se pudo conectar a la base de datos configurada:\n" +
                        $"{Conexion.RutaBD}\n\n{error}",
                    permiteCancelar: false);

            return ventanaError.ShowDialog() == true;
        }

        private void btnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (!mostrando)
            {
                passVisible.Text = passOculta.Password;
                passVisible.Visibility = Visibility.Visible;
                passOculta.Visibility = Visibility.Collapsed;
                imgOjo.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/Resources/eye.png"));
                mostrando = true;
            }
            else
            {
                passOculta.Password = passVisible.Text;
                passVisible.Visibility = Visibility.Collapsed;
                passOculta.Visibility = Visibility.Visible;
                imgOjo.Source = new BitmapImage(
                    new Uri(
                        "pack://application:,,,/Resources/eyeClose.png"));
                mostrando = false;
            }

            ActualizarPlaceholderPassword();
        }

        private void txtUsuario_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            txtPlaceholderUsuario.Visibility =
                string.IsNullOrWhiteSpace(txtUsuario.Text)
                    ? Visibility.Visible
                    : Visibility.Hidden;
        }

        private void passVisible_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            ActualizarPlaceholderPassword();
        }

        private void passOculta_PasswordChanged(
            object sender,
            RoutedEventArgs e)
        {
            ActualizarPlaceholderPassword();
        }

        private void ActualizarPlaceholderPassword()
        {
            string textoPassword =
                mostrando
                    ? passVisible.Text
                    : passOculta.Password;

            txtPlaceholderPassword.Visibility =
                string.IsNullOrWhiteSpace(textoPassword)
                    ? Visibility.Visible
                    : Visibility.Hidden;
        }

        private void txtRegistrate_MouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            var ventana = new CrearUsuario();
            ventana.ShowDialog();
        }
    }
}
