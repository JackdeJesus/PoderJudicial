using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PoderJudicial.Helpers
{
    public sealed class CatalogoPersonasData
    {
        public List<string> Entregan { get; set; } = new();
        public List<string> Reciben { get; set; } = new();
    }

    public static class PersonaCatalogoService
    {
        private static readonly JsonSerializerOptions OpcionesJson =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

        public static CatalogoPersonasData Cargar()
        {
            RutasInformes.CrearEstructura();

            string ruta =
                RutasInformes.ObtenerRutaCatalogoPersonas();

            if (!File.Exists(ruta))
                return new CatalogoPersonasData();

            string json = File.ReadAllText(ruta);

            if (string.IsNullOrWhiteSpace(json))
                return new CatalogoPersonasData();

            CatalogoPersonasData? datos =
                JsonSerializer.Deserialize<CatalogoPersonasData>(
                    json,
                    OpcionesJson);

            return datos ?? new CatalogoPersonasData();
        }

        public static void Guardar(
            IEnumerable<string> entregan,
            IEnumerable<string> reciben)
        {
            RutasInformes.CrearEstructura();

            var datos = new CatalogoPersonasData
            {
                Entregan = PrepararLista(entregan),
                Reciben = PrepararLista(reciben)
            };

            string json =
                JsonSerializer.Serialize(
                    datos,
                    OpcionesJson);

            string ruta =
                RutasInformes.ObtenerRutaCatalogoPersonas();

            File.WriteAllText(ruta, json);
        }

        private static List<string> PrepararLista(
            IEnumerable<string> valores)
        {
            return (valores ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }
    }
}
