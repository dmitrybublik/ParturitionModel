using System;
using System.Collections.Generic;
using System.Linq;
using ParturitionModel.Core;

namespace ParturitionModel.App
{
    public sealed class DataViewModel
    {
        public int Year { get; set; }

        public int Population { get; set; }

        public int ChildDeathTotal
        {
            get { return BornInfos.Sum(x => x.DeathCount); }
        }

        public int ChildBornTotal
        {
            get { return BornInfos.Sum(x => x.BornCount); }
        }

        public IEnumerable<BornInfo> BornInfos  { get; set; }
    }
}