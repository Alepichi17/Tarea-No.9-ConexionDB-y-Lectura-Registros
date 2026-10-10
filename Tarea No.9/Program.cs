using Microsoft.Data.SqlClient;
namespace Tarea_No._9
{
    internal class Program
    {
        private const string CadenaConexion =
           @"Server=(localdb)\MSSQLLocalDB;" +
           @"Database=GastosDB;" +
           @"Trusted_Connection=True;" +
           @"TrustServerCertificate=True;";
        static void Main(string[] args)
        {
            string ruta = "Gastos.csv";
            string Opcion;
            List<Gasto> gasto = Cargar(ruta);
            int SiguienteId = 1;

            foreach (Gasto g in gasto)
            {
                if (g.id >= SiguienteId)
                {
                    SiguienteId = g.id + 1;
                }
            }
            do
            {
                Opcion = Menu();

                switch (Opcion)
                {
                    case "1":
                        AgregarGasto(gasto, ref SiguienteId);
                        break;
                    case "2":
                        ListarGastos(gasto);
                        break;
                    case "3":
                        BuscarCategoria(gasto);
                        break;
                    case "4":
                        ListarDesdeSql();
                        break;
                    case "5":
                        Guardar(gasto, ruta);
                        Console.WriteLine("Saliendo Del Programa");
                        break;
                    default:
                        Console.WriteLine("Opcion No Valida");
                        break;
                }
            }
            while (Opcion != "5");
        }

        public static string Menu()
        {
            Console.WriteLine();
            Console.WriteLine("_-_-_-Registro De Gastos_-_-_-_-");
            Console.WriteLine("Opcion 1: Agregar Gasto");
            Console.WriteLine("Opcion 2: Listar Gastos");
            Console.WriteLine("Opcion 3: Buscar Por Categoria");
            Console.WriteLine("Opcion 4: Listar Desde SQL");
            Console.WriteLine("Opcion 5: Salir");
            Console.Write("Elige Una Opcion: ");
            return Console.ReadLine();
        }

        public static void AgregarGasto(List<Gasto> g, ref int Id)
        {
            Console.WriteLine();
            Console.Write("Descripcion: ");
            string Descripcion = Console.ReadLine();
            Console.Write("Monto: ");
            bool Montook = decimal.TryParse(Console.ReadLine(), out decimal Monto);
            Console.Write("Categoria: ");
            string Categoria = Console.ReadLine();

            if (!Montook || Monto < 0 || string.IsNullOrWhiteSpace(Descripcion) || string.IsNullOrWhiteSpace(Categoria))
            {
                Console.WriteLine("Datos No Validos");
                return;
            }

            Gasto NuevoGasto = new Gasto()
            {
                id = Id,
                descripcion = Descripcion,
                monto = Monto,
                categoria = Categoria,
            };
            g.Add(NuevoGasto);
            Id++;
            Console.WriteLine("Gasto Agregado");
        }

        public static void ListarGastos(List<Gasto> g)
        {
            int Elementos = g.Count;
            decimal Total = 0;
            if (Elementos == 0)
            {
                Console.WriteLine("Sin Elementos");
            }
            foreach (Gasto Gasto in g)
            {
                Console.WriteLine(Gasto);
                Total += Gasto.monto;
            }
            Console.WriteLine($"Total: {Total}");
        }

        public static void BuscarCategoria(List<Gasto> g)
        {
            Console.Write("Categoria: ");
            string Buscar = Console.ReadLine().Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(Buscar))
            {
                Console.WriteLine("Intente de nuevo");
                return;
            }

            bool encontrado = false;
            foreach (Gasto Gasto in g)
            {
                if (Gasto.categoria.ToUpper().Contains(Buscar))
                {
                    Console.WriteLine(Gasto);
                    encontrado = true;
                }
            }
            if (!encontrado)
            {
                Console.WriteLine("No encontrado");
            }
        }

        private static void Guardar(List<Gasto> g, string ruta)
        {
            var lineas = new List<string>();

            foreach (Gasto Gasto in g)
            {
                lineas.Add($"{Gasto.id}; {Gasto.descripcion}; {Gasto.monto}; {Gasto.categoria}");
            }
            File.WriteAllLines(ruta, lineas);
            Console.WriteLine($"Gasto Guardados en {ruta} {(g.Count)} Gastos");
        }

        private static List<Gasto> Cargar(string ruta)
        {
            var g = new List<Gasto>();
            if (!File.Exists(ruta)) return g;

            foreach (string linea in File.ReadLines(ruta))
            {
                string[] campos = linea.Split(';');

                if (campos.Length != 4) continue;

                Gasto NuevoGasto = new Gasto()
                {
                    id = int.Parse(campos[0]),
                    descripcion = campos[1],
                    monto = decimal.Parse(campos[2]),
                    categoria = campos[3]
                };
                g.Add(NuevoGasto);
            }
            Console.WriteLine($"Cargados {g.Count} Productos desde {ruta}");
            return g;
        }

        private static void ListarDesdeSql()
        {
            using (SqlConnection conexion = new SqlConnection(CadenaConexion))
            {
                conexion.Open();
                Console.WriteLine("Conexion exitosa a InventarioDB");
                Console.WriteLine();

                string sql = "SELECT Id, Descripcion, Monto, Categoria FROM Gastos";
                using (SqlCommand comando = new SqlCommand(sql, conexion))
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    Console.WriteLine("Id   Descripcion          Monto     Categoria");
                    decimal Total = 0;
                    bool HayGastos = false;
                    while (lector.Read())
                    {
                        HayGastos = true;
                        int Id = lector.GetInt32(0);
                        string Descripcion = lector.GetString(1);
                        decimal Monto = lector.GetDecimal(2);
                        string Categoria = lector.GetString(3);
                        Console.WriteLine($"{Id,-3}  {Descripcion,-20} Q {Monto,7:N2} {Categoria,5}");
                        Total += Monto;
                    }
                    if(HayGastos)
                    {
                    Console.WriteLine();
                    Console.WriteLine($"Total Gastado: {Total}");
                    }
                    else
                    {
                        Console.WriteLine("No Hay Gastos");
                    }
                }
            }
        }
    }
}