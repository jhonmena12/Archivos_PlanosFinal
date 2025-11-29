using System.Globalization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
 
namespace Dominio
{
    public static class GestorArchivos
    {
        static string RutaBase => Directory.GetCurrentDirectory();
        static string RU = Path.Combine(RutaBase, "usuarios.txt");
        static string RP = Path.Combine(RutaBase, "personas.txt");
        static string RL = Path.Combine(RutaBase, "auditoria.log");
        static string RCSV = Path.Combine(RutaBase, "reporte_ciudades.csv");

        public static void CrearSiNoExisten()
        {
            if (!File.Exists(RU))
            {
                List<string> usuarios = new();
                for (int i = 1; i <= 15; i++)
                    usuarios.Add($"{i};clave{i};1");
                File.WriteAllLines(RU, usuarios);
            }

            if (!File.Exists(RP))
                File.WriteAllText(RP, "");

            if (!File.Exists(RL))
                File.WriteAllText(RL, "");
        }

        public static List<Credencial> ObtenerUsuarios()
        {
            CrearSiNoExisten();
            var lista = new List<Credencial>();

            foreach (var linea in File.ReadAllLines(RU))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                var d = linea.Split(';');
                if (d.Length < 3) continue;

                bool estado = d[2] == "1";
                lista.Add(new Credencial(d[0], d[1], estado));
            }

            return lista;
        }

        public static void SalvarUsuarios(List<Credencial> u)
        {
            File.WriteAllLines(RU, u.Select(t => $"{t.Usuario};{t.Clave};{(t.Habilitado ? 1 : 0)}"));
        }


        public static List<PersonaRegistro> ObtenerPersonas()
        {
            CrearSiNoExisten();
            var lista = new List<PersonaRegistro>();

            foreach (var linea in File.ReadAllLines(RP))
            {
                if (string.IsNullOrWhiteSpace(linea)) continue;

                var d = linea.Split(';');
                if (d.Length < 6) continue;

                if (!int.TryParse(d[0], out int cod)) continue;
                if (!decimal.TryParse(d[5], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                    val = 0;

                lista.Add(new PersonaRegistro(
                    cod,
                    d[1],
                    d[2],
                    d[3],
                    d[4],
                    val
                ));
            }

            return lista;
        }

        public static void GuardarPersonas(List<PersonaRegistro> personas)
        {
            File.WriteAllLines(RP, personas.Select(p =>
                $"{p.Codigo};{p.Nombre1};{p.Apellido1};{p.Ciudad};{p.Tel};{p.Valor.ToString(CultureInfo.InvariantCulture)}"
            ));
        }

        public static void Registrar(string usuario, string evento)
        {
            File.AppendAllLines(RL, new[] {
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {usuario} | {evento}"
            });
        }

        
        public static string ExportarCSV(List<PersonaRegistro> lista)
        {
            var grupos = lista.GroupBy(x => x.Ciudad).OrderBy(x => x.Key);

            using var sw = new StreamWriter(RCSV, false);
            sw.WriteLine("Ciudad;Codigo;Nombre;Apellido;Telefono;Saldo");

            foreach (var g in grupos)
            {
                foreach (var p in g)
                {
                    sw.WriteLine($"{g.Key};{p.Codigo};{p.Nombre1};{p.Apellido1};{p.Tel};{p.Valor}");
                }
                sw.WriteLine();
            }

            return RCSV;
        }
    }
}