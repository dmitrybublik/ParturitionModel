using System;
using System.Collections.Generic;

namespace ParturitionModel.Core
{
    public sealed class SimulationEventArgs : EventArgs
    {
        private readonly Dictionary<int, BornInfo> _bornInfos = 
            new Dictionary<int, BornInfo>();

        public int NaturalDeathCount { get; set; }

        public int CurrentYear { get; set; }

        public IDictionary<int, BornInfo> BornInfos
        {
            get { return _bornInfos; }
        }
    }
}