using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yapaySinir
{
    internal class NetworkConfig
    {
        public int RowNumber { get; } = 7;
        public int ColumnNumber { get; } = 5;
        public string[] Letters { get; } = { "A", "B", "C", "D", "E" };
        public double LearningRate { get; set; } = 0.1;
        public int IterationNumber { get; set; } = 20;
    }
}
