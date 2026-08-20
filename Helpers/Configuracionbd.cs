﻿using System;
using System.IO;
using System.Text.Json;

namespace PoderJudicial.Helpers
{
    /// <summary>
    /// Configuración persistente de la aplicación.
    /// La ruta de Access sigue siendo la única fuente de verdad para la BD.
    /// Los parámetros de respaldo viven en este mismo archivo para evitar
    /// configuraciones duplicadas.
    /// </summary>
    public class ConfiguracionBD
    {
        public string Proveedor { get; set; } = "Access";
        public string RutaArchivo { get; set; } = "";

        // Respaldos automáticos
        public bool RespaldosAutomaticos { get; set; } = true;
        public int FrecuenciaRespaldoDias { get; set; } = 15;
        public int MaximoRespaldos { get; set; } = 6;

        /// <summary>
        /// Ruta opcional para los respaldos. Vacía = subcarpeta "Respaldos"
        /// dentro de la carpeta que contiene la base de datos.
        /// </summary>
        public string CarpetaRespaldos { get; set; } = "";

        private static readonly string CarpetaConfig =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PoderJudicial");

        private static readonly string RutaConfig =
            Path.Combine(CarpetaConfig, "config.json");

        public static ConfiguracionBD Cargar()
        {
            try
            {
                if (!File.Exists(RutaConfig))
                    return null;

                string json = File.ReadAllText(RutaConfig);
                ConfiguracionBD config =
                    JsonSerializer.Deserialize<ConfiguracionBD>(json);

                if (string.IsNullOrWhiteSpace(config?.RutaArchivo))
                    return null;

                // Valores defensivos para configuraciones antiguas o
                // modificadas manualmente.
                if (config.FrecuenciaRespaldoDias < 1)
                    config.FrecuenciaRespaldoDias = 15;

                if (config.MaximoRespaldos < 1)
                    config.MaximoRespaldos = 6;

                return config;
            }
            catch
            {
                // Archivo corrupto o ilegible: se trata igual que
                // "no configurado" en vez de tumbar la aplicación.
                return null;
            }
        }

        /// <summary>
        /// Guarda esta configuración en disco, creando la carpeta si hace falta.
        /// </summary>
        public void Guardar()
        {
            Directory.CreateDirectory(CarpetaConfig);

            string json = JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(RutaConfig, json);
        }
    }
}
