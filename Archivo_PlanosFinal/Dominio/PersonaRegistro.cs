namespace Dominio

{

    public class PersonaRegistro

    {

        public int Codigo { get; set; }

        public string Nombre1 { get; set; }

        public string Apellido1 { get; set; }

        public string Ciudad { get; set; }

        public string Tel { get; set; }

        public decimal Valor { get; set; }

        public PersonaRegistro() { }

        public PersonaRegistro(int cod, string nom, string ape, string ciu, string tel, decimal val)

        {

            Codigo = cod;

            Nombre1 = nom;

            Apellido1 = ape;

            Ciudad = ciu;

            Tel = tel;

            Valor = val;

        }

    }

}
