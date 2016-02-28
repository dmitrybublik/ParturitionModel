using System;

namespace ParturitionModel.Core
{
    public sealed class SimulationEventArgs : EventArgs
    {
        public int NaturalDeathCount { get; set; }

        public int CurrentYear { get; set; }

        public int ChildDeath { get; set; }

        public int ChildBorn { get; set; }
    }
}