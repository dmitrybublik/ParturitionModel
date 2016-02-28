namespace ParturitionModel.Core
{
    public sealed class BornInfo
    {
        public int Order { get; set; }

        public int MotherAge { get; set; }

        public int BornCount { get; set; }

        public int DeathCount { get; set; }

        public int TotalCount
        {
            get { return BornCount + DeathCount; }
        }

        public double Factor
        {
            get
            {
                var totalCount = TotalCount;

                if (totalCount == 0)
                {
                    return 0;
                }

                return (double) BornCount/totalCount;
            }
        }
    }
}