using System;
using System.Collections.Generic;

namespace LegendPeak.Platforms
{
    public class PlatformRangeInfo
    {
        public int heightRange;
        public int prefabRangeStart;
        public int prefabRangeEnd;

        public PlatformRangeInfo()
        {
        }

        public PlatformRangeInfo(int heightRange, int prefabRangeStart, int prefabRangeEnd)
        {
            this.heightRange = heightRange;
            this.prefabRangeStart = prefabRangeStart;
            this.prefabRangeEnd = prefabRangeEnd;
        }
    }

}

