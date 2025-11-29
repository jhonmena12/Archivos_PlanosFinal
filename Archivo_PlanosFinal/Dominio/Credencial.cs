namespace Dominio

{

    public class Credencial

    {

        public string Usuario { get; set; }

        public string Clave { get; set; }

        public bool Habilitado { get; set; }

        public Credencial() { }

        public Credencial(string u, string c, bool h)

        {

            Usuario = u;

            Clave = c;

            Habilitado = h;

        }

    }

}
