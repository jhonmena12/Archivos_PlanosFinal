using System;

using System.Linq;

using Dominio;

using System.Globalization;

namespace AppTaller

{

    class Inicio

    {

        static string usuario = "";

        static void Main()

        {

            GestorArchivos.CrearSiNoExisten();

            Console.WriteLine("=== SISTEMA TALLER 6  ===\n");

            if (!IniciarSesion())

            {

                Console.WriteLine("No fue posible continuar.");

                return;

            }

            EjecutarMenu();

        }

        
        static bool IniciarSesion()

        {

            var lista = GestorArchivos.ObtenerUsuarios();

            int intentos = 0;

            Credencial u = null;

            while (intentos < 3)

            {

                Console.Write("Usuario: ");

                var us = Console.ReadLine();

                Console.Write("Clave: ");

                var cl = Console.ReadLine();

                u = lista.FirstOrDefault(x => x.Usuario == us);

                if (u != null)

                {

                    if (!u.Habilitado)

                    {

                        Console.WriteLine("Usuario bloqueado.");

                        return false;

                    }

                    if (u.Clave == cl)

                    {

                        usuario = us;

                        GestorArchivos.Registrar(usuario, "Inicio exitoso");

                        return true;

                    }

                }

                intentos++;

                Console.WriteLine("Datos incorrectos.");

            }

            if (u != null)

            {

                u.Habilitado = false;

                GestorArchivos.SalvarUsuarios(lista);

                GestorArchivos.Registrar(u.Usuario, "Bloqueado por intentos fallidos");

            }

            return false;

        }

        

        static void EjecutarMenu()

        {

            int op = 0;

            do

            {

                Console.WriteLine("\n--- MENU PRINCIPAL ---");

                Console.WriteLine("1) Registrar persona");

                Console.WriteLine("2) Modificar persona");

                Console.WriteLine("3) Quitar persona");

                Console.WriteLine("4) Ver listado");

                Console.WriteLine("5) Informe por ciudad");

                Console.WriteLine("6) Exportar CSV");

                Console.WriteLine("7) Salir");

                Console.Write("Opción: ");

                int.TryParse(Console.ReadLine(), out op);

                switch (op)

                {

                    case 1: Registrar(); break;

                    case 2: Modificar(); break;

                    case 3: Quitar(); break;

                    case 4: Listado(); break;

                    case 5: Informe(); break;

                    case 6: Exportar(); break;

                }

            } while (op != 7);

        }

        
        static void Registrar()

        {

            var lista = GestorArchivos.ObtenerPersonas();

            Console.Write("Código: ");

            if (!int.TryParse(Console.ReadLine(), out int c) || lista.Any(x => x.Codigo == c))

            {

                Console.WriteLine("Código inválido o duplicado.");

                return;

            }

            Console.Write("Nombre: ");

            string n = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(n)) return;

            Console.Write("Apellido: ");

            string a = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(a)) return;

            Console.Write("Ciudad: ");

            string ciu = Console.ReadLine();

            Console.Write("Teléfono: ");

            string tel = Console.ReadLine();

            if (!tel.All(char.IsDigit)) return;

            Console.Write("Saldo: ");

            decimal sal;

            if (!decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out sal) || sal < 0)

                return;

            lista.Add(new PersonaRegistro(c, n, a, ciu, tel, sal));

            GestorArchivos.GuardarPersonas(lista);

            GestorArchivos.Registrar(usuario, $"Agrega persona {c}");

            Console.WriteLine("✓ Registrado.");

        }

        
        static void Modificar()

        {

            var lista = GestorArchivos.ObtenerPersonas();

            Console.Write("Código modificar: ");

            int.TryParse(Console.ReadLine(), out int cod);

            var p = lista.FirstOrDefault(x => x.Codigo == cod);

            if (p == null) { Console.WriteLine("No existe."); return; }

            Console.Write($"Nombre ({p.Nombre1}): ");

            var nn = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nn)) p.Nombre1 = nn;

            Console.Write($"Apellido ({p.Apellido1}): ");

            var na = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(na)) p.Apellido1 = na;

            Console.Write($"Ciudad ({p.Ciudad}): ");

            var nc = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nc)) p.Ciudad = nc;

            Console.Write($"Teléfono ({p.Tel}): ");

            var nt = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(nt) && nt.All(char.IsDigit)) p.Tel = nt;

            Console.Write($"Saldo ({p.Valor}): ");

            var ns = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(ns))

            {

                if (decimal.TryParse(ns, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal nuevo))

                    p.Valor = nuevo;

            }

            GestorArchivos.GuardarPersonas(lista);

            GestorArchivos.Registrar(usuario, $"Modifica persona {cod}");

            Console.WriteLine("✓ Modificada.");

        }

       

        static void Quitar()

        {

            var lista = GestorArchivos.ObtenerPersonas();

            Console.Write("Código a quitar: ");

            int.TryParse(Console.ReadLine(), out int cod);

            var p = lista.FirstOrDefault(x => x.Codigo == cod);

            if (p == null) { Console.WriteLine("No existe."); return; }

            Console.Write("¿Eliminar? (s/n): ");

            if (Console.ReadLine().ToLower() == "s")

            {

                lista.Remove(p);

                GestorArchivos.GuardarPersonas(lista);

                GestorArchivos.Registrar(usuario, $"Elimina persona {cod}");

                Console.WriteLine("✓ Eliminado.");

            }

        }

        

        static void Listado()

        {

            var lista = GestorArchivos.ObtenerPersonas();

            Console.WriteLine("\n=== LISTADO ===");

            foreach (var p in lista)

            {

                Console.WriteLine($"{p.Codigo} | {p.Nombre1} {p.Apellido1} | {p.Ciudad} | {p.Tel} | {p.Valor:N2}");

            }

        }

      

        static void Informe()

        {

            var lista = GestorArchivos.ObtenerPersonas();

            var grupos = lista.GroupBy(x => x.Ciudad).OrderBy(x => x.Key);

            decimal total = 0;

            Console.WriteLine("\n=== INFORME POR CIUDAD ===\n");

            foreach (var g in grupos)

            {

                Console.WriteLine($"Ciudad: {g.Key}\n");

                Console.WriteLine("COD\tNombre\t\tApellido\tSaldo");

                Console.WriteLine("—\t—-------------\t—------------\t—----------");

                decimal sub = 0;

                foreach (var p in g)

                {

                    Console.WriteLine($"{p.Codigo}\t{p.Nombre1}\t\t{p.Apellido1}\t\t{p.Valor:N2}");

                    sub += p.Valor;

                }

                Console.WriteLine("\t\t\t\t=======");

                Console.WriteLine($"Subtotal {g.Key}\t\t\t{sub:N2}\n");

                total += sub;

            }

            Console.WriteLine("\t\t\t\t=======");

            Console.WriteLine($"TOTAL GENERAL:\t\t\t{total:N2}");

        }

       

        static void Exportar()

        {

            var lista = GestorArchivos.ObtenerPersonas();

            string ruta = GestorArchivos.ExportarCSV(lista);

            Console.WriteLine($"Archivo listo en: {ruta}");

        }

    }

}

