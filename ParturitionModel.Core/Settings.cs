namespace ParturitionModel.Core
{
    internal sealed class Settings
    {
        /// <summary>
        /// Initial population size.
        /// </summary>
        public int PopulationSize
        {
            get { return 100000; }
        }

        /// <summary>
        /// Initial men factor in population.
        /// </summary>
        public double SexFactor
        {
            get { return 0.5; }
        }

        /// <summary>
        /// Child men factor in population.
        /// </summary>
        public double ChildSexFactor
        {
            get { return 0.5; }
        }

        /// <summary>
        /// The maximum life span (in years).
        /// </summary>
        public int LifeLimit
        {
            get { return 70; }
        }

        /// <summary>
        /// Time of doubling the number of population (in years).
        /// </summary>
        public int DoublingPopulationTime
        {
            get { return 200; }
        }

        /// <summary>
        /// Simulation time (in years).
        /// </summary>
        public int SimulationTime
        {
            get { return 1000; }
        }

        /// <summary>
        /// The probability of death at first birth.
        /// </summary>
        public double DeathProbability
        {
            get { return 0.25; }
        }

        public double MutationFactor
        {
            get { return 0.01; }
        }

        public int[] BirthYears
        {
            get { return new[] {17, 21, 25, 29, 33}; }
        }

        public double[] BirthDeathFactor
        {
            get { return new[] {1.0, 0.7, 0.5, 0.7, 1.0}; }
        }

        public int NubilityManAge
        {
            get { return 16; }
        }
    }
}