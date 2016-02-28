namespace ParturitionModel.App
{
    public sealed class DataViewModel
    {
        public int Year { get; set; }

        public int Population { get; set; }

        public int ChildDeath { get; set; }

        public int ChildBorn { get; set; }

        public double ChildDeathPercents
        {
            get
            {
                if (ChildBorn == 0)
                {
                    return 0.0;
                }

                return (double) ChildDeath/ChildBorn;
            }
        }
    }
}