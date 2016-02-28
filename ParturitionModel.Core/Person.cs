namespace ParturitionModel.Core
{
    internal sealed class Person
    {
        private readonly Environment _environment;

        public Person(Environment environment)
        {
            _environment = environment;

            BirthYear = _environment.Year;
        }

        public Sex Sex { get; set; }

        public int BirthYear { get; set; }

        public int Age
        {
            get { return _environment.Year - BirthYear; }
        }

        public double DeathFactor { get; set; }
    }
}