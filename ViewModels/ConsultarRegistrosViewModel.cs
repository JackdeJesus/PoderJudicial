using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PoderJudicial.Data;
using PoderJudicial.Models;
using PoderJudicial.Views;

namespace PoderJudicial.ViewModels
{
    public class ConsultarRegistrosViewModel : BaseViewModel
    {
        
        private List<Audiencia> _listaCompleta = new List<Audiencia>();
        private DispatcherTimer _reloj;

        // ──────────────────────────────────────────
        //  PROPIEDADES
        // ──────────────────────────────────────────
        private ObservableCollection<Audiencia> _audiencias;
        public ObservableCollection<Audiencia> Audiencias
        {
            get => _audiencias;
            set { _audiencias = value; OnPropertyChanged(); }
        }

        private List<string> _sugerencias;
        public List<string> Sugerencias
        {
            get => _sugerencias;
            set { _sugerencias = value; OnPropertyChanged(); }
        }

        private string _textoBusqueda = "";
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                Filtrar();
                ActualizarSugerencias();
            }
        }

        private string _totalRegistros;
        public string TotalRegistros
        {
            get => _totalRegistros;
            set { _totalRegistros = value; OnPropertyChanged(); }
        }

        private string _hora;
        public string Hora
        {
            get => _hora;
            set { _hora = value; OnPropertyChanged(); }
        }

        private string _fecha;
        public string Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(); }
        }

        // ──────────────────────────────────────────
        //  COMANDOS
        // ──────────────────────────────────────────
        public ICommand VerCommand { get; }
        public ICommand EditarCommand { get; }
        public ICommand EliminarCommand { get; }

        // ──────────────────────────────────────────
        //  CONSTRUCTOR
        // ──────────────────────────────────────────
        public ConsultarRegistrosViewModel()
        {
            VerCommand = new RelayCommand(EjecutarVer);
            EditarCommand = new RelayCommand(EjecutarEditar);
            EliminarCommand = new RelayCommand(EjecutarEliminar);

            IniciarReloj();
            CargarDatos();
        }

        // ──────────────────────────────────────────
        //  RELOJ
        // ──────────────────────────────────────────
        private void IniciarReloj()
        {
            _reloj = new DispatcherTimer();
            _reloj.Interval = TimeSpan.FromSeconds(1);
            _reloj.Tick += (s, e) => ActualizarHora();
            _reloj.Start();
            ActualizarHora();
        }

        private void ActualizarHora()
        {
            Hora = DateTime.Now.ToString("hh:mm tt");
            Fecha = DateTime.Now.ToString("dddd, dd MMMM yyyy");
        }

        // ──────────────────────────────────────────
        //  DATOS
        // ──────────────────────────────────────────
        private void CargarDatos()
        {
            try
            {
                AudienciaData data = new AudienciaData();
                _listaCompleta = data.ObtenerAudiencias();

                Audiencias = new ObservableCollection<Audiencia>(_listaCompleta.Take(5));
                TotalRegistros = $"{_listaCompleta.Count} registro(s) en total";

                CargarSugerencias();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        private void CargarSugerencias()
        {
            Sugerencias = _listaCompleta
                .SelectMany(x => new[] { x.NoCausa, x.NUC, x.Imputado, x.FechaAudiencia })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();
        }

        // ──────────────────────────────────────────
        //  FILTRADO Y SUGERENCIAS
        // ──────────────────────────────────────────
        private void Filtrar()
        {
            string texto = _textoBusqueda.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                Audiencias = new ObservableCollection<Audiencia>(_listaCompleta.Take(5));
                TotalRegistros = $"{_listaCompleta.Count} registro(s) en total";
                return;
            }

            var filtrados = _listaCompleta.Where(a =>
                   (a.NoCausa != null && a.NoCausa.ToLower().Contains(texto))
                || (a.NUC != null && a.NUC.ToLower().Contains(texto))
                || (a.Imputado != null && a.Imputado.ToLower().Contains(texto))
                || (a.FechaAudiencia != null && a.FechaAudiencia.ToLower().Contains(texto))
            ).ToList();

            Audiencias = new ObservableCollection<Audiencia>(filtrados);
            TotalRegistros = $"{filtrados.Count} registro(s) encontrado(s)";
        }

        private void ActualizarSugerencias()
        {
            string texto = _textoBusqueda.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                Sugerencias = new List<string>();
                return;
            }

            Sugerencias = _listaCompleta
                .SelectMany(x => new[] { x.NoCausa, x.NUC, x.Imputado, x.FechaAudiencia })
                .Where(x => !string.IsNullOrWhiteSpace(x) && x.ToLower().Contains(texto))
                .Distinct()
                .Take(10)
                .ToList();
        }

        // ──────────────────────────────────────────
        //  ACCIONES
        // ──────────────────────────────────────────
        private void EjecutarVer(object param)
        {
            if (param is Audiencia audiencia)
            {
                VerDetalleRegistro ventana = new VerDetalleRegistro();
                ventana.ShowDialog();
            }
        }

        private void EjecutarEditar(object param)
        {
            if (param is Audiencia audiencia)
            {
                EditarRegistro ventana = new EditarRegistro();
                ventana.ShowDialog();
            }
        }

        private void EjecutarEliminar(object param)
        {
            if (param is Audiencia audiencia)
            {
                var resultado = MessageBox.Show(
                    $"¿Eliminar el registro {audiencia.NoCausa}?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultado == MessageBoxResult.Yes)
                {
                    _listaCompleta.Remove(audiencia);
                    Filtrar();
                }
            }
        }
    }
}