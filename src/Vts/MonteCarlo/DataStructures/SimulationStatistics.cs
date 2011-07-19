using System;
using System.Collections.Generic;
using Vts.IO;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This class captures informational statistics of the MC simulation executed
    /// </summary>
    public class SimulationStatistics
    {
        public SimulationStatistics(
            long numberOfPhotonsOutTopOfTissue,
            long numberOfPhotonsOutBottomOfTissue,
            long numberOfPhotonsAbsorbed,
            long numberOfPhotonsKilledOverMaximumPathLength,
            long numberOfPhotonsKilledOverMaximumCollisions,
            long numberOfPhotonsKilledByRussianRoulette)
        {
            NumberOfPhotonsOutTopOfTissue = numberOfPhotonsOutTopOfTissue;
            NumberOfPhotonsOutBottomOfTissue = numberOfPhotonsOutBottomOfTissue;
            NumberOfPhotonsAbsorbed = numberOfPhotonsAbsorbed;
            NumberOfPhotonsKilledOverMaximumPathLength = numberOfPhotonsKilledOverMaximumPathLength;
            NumberOfPhotonsKilledOverMaximumCollisions = numberOfPhotonsKilledOverMaximumCollisions;
            NumberOfPhotonsKilledByRussianRoulette = numberOfPhotonsKilledByRussianRoulette;
        }

        public SimulationStatistics() : this(0, 0, 0, 0, 0, 0) { }

        public long NumberOfPhotonsOutTopOfTissue { get; set; }
        public long NumberOfPhotonsOutBottomOfTissue { get; set; }
        public long NumberOfPhotonsAbsorbed { get; set; }
        public long NumberOfPhotonsKilledOverMaximumPathLength { get; set; }
        public long NumberOfPhotonsKilledOverMaximumCollisions { get; set; }
        public long NumberOfPhotonsKilledByRussianRoulette { get; set; }

        public void ToFile(string filename)
        {
            FileIO.WriteToXML(this, filename);
        }
        public static SimulationStatistics FromFile(string filename)
        {
            return FileIO.ReadFromXML<SimulationStatistics>(filename);
        }
    }
}