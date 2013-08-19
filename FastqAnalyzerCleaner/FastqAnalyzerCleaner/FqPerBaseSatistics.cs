using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
namespace FastqAnalyzerCleaner
{
    [Serializable]
    [ProtoContract]
    public class FqPerBaseSatistics
    {
        [ProtoMember(1)]
        public int UpperThreshold { get; set; }
        [ProtoMember(2)]
        public int ThirdQuartile { get; set; }
        [ProtoMember(3)]
        public int Median { get; set; }
        [ProtoMember(4)]
        public double Mean { get; set; }
        [ProtoMember(5)]
        public int FirstQuartile { get; set; }
        [ProtoMember(6)]
        public int LowerThreshold { get; set; }
        [ProtoMember(7)]
        public int BaseCount { get; set; }
        [ProtoIgnore]
        private List<int> baseQualities;
        [ProtoIgnore]
        private int qualitiesTotal = 0;

        public FqPerBaseSatistics()
        {
            baseQualities = new List<int>();
            Mean = 0;
            Median = 0;
            ThirdQuartile = 0;
            FirstQuartile = 0;
            LowerThreshold = 0;
            UpperThreshold = 0;
        }

        public void addBaseQuality(int quality)
        {
            baseQualities.Add(quality);
            qualitiesTotal += quality;
            
        }

        public void reconcileBaseSatistics()
        {
            baseQualities.Sort();
            BaseCount = baseQualities.Count;
            Mean = (double) qualitiesTotal / (double) BaseCount;
            Median = (int) baseQualities[(BaseCount / 2)];
            ThirdQuartile = (int)baseQualities[((BaseCount / 4) * 3)];
            FirstQuartile = (int)baseQualities[((BaseCount / 4))];
            LowerThreshold = (int)baseQualities[0];
            UpperThreshold = (int) baseQualities[BaseCount - 1];

            baseQualities = null;
        }

        public int GetPerBaseCount()
        {
            return baseQualities.Count();
        }

    }
}
