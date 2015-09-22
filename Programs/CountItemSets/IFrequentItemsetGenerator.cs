﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountItemSets
{
    public interface IFrequentItemsetGenerator
    {
        IDictionary<long, int> Level1 { get; }
        IDictionary<string, int> Level2 { get; }
        IDictionary<string, int> Level3 { get; }
        IDictionary<string, int> Level4 { get; }
        IDictionary<string, int> Level5 { get; }
    }
}
