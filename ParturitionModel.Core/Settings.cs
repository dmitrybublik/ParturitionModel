namespace ParturitionModel.Core
{
    internal sealed class PregoInfo
    {
        public int Age { get; set; }

        public int Order { get; set; }

        public double Factor { get; set; }
    }

    internal sealed class Settings
    {
        private static readonly int[] _birthYears = { 17, 21, 25, 29, 33 };
        private static readonly double[] _birthDeathFactor = { 1.0, 0.7, 0.5, 0.7, 1.0 };

        private PregoInfo[] _pregoInfos;

        public Settings()
        {
            _pregoInfos = new PregoInfo[LifeLimit + 1];
            for (int i = 0; i < _birthYears.Length; ++i)
            {
                var age = _birthYears[i];
                _pregoInfos[age] = new PregoInfo
                {
                    Age = age,
                    Factor = _birthDeathFactor[i],
                    Order = i + 1
                };
            }
        }

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
            get { return 0.5; }
        }

        public double MutationFactor
        {
            get { return 0.01; }
        }

        public int NubilityManAge
        {
            get { return 16; }
        }

        public PregoInfo GetPregoInfo(int age)
        {
            return _pregoInfos[age];
        }
    }
}