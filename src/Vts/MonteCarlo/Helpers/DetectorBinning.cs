using System;
using System.Linq;

namespace Vts.MonteCarlo.Helpers
{
    public class DetectorBinning
    {
        /// <summary>
        /// WhichBin determines which uniform bin "value" is in
        /// If data is beyond last bin, the last bin is returned
        /// If the data is smaller than first bin, the first bin is returned
        /// This assumes bins are contiguous
        /// </summary>
        /// <param name="value">value to be binned</param>
        /// <param name="binSize">bin size</param>
        /// <param name="numberOfBins">size of array</param>
        /// <param name="binStart">starting location of binning</param>
        public static int WhichBin(double value, int numberOfBins, double binSize, double binStart)
        {
            int bin = (int)Math.Floor((value - binStart) / binSize);
            if (bin > numberOfBins - 1)
                return numberOfBins - 1;
            else
                if (bin < 0)
                    return 0;
                else
                    return bin;
        }
        /// <summary>
        /// WhichBin determines which bin "value" is in
        /// If value not in any bin, -1 returned
        /// This allows for non-contiguous bins and nonuniformly spaced bins
        /// </summary>
        /// <param name="value">value to be binned</param>
        /// <param name="binSize">bin size</param>
        /// <param name="binCenters">list of bin centers</param>
        public static int WhichBin(double value, double binSize, double[] binCenters)
        {
            for (int i = 0; i < binCenters.Count(); i++)
			{
                if ((value > binCenters[i] - binSize / 2) && (value < binCenters[i] + binSize / 2))
                    return i;
            }
            return -1; // for now
        }

        public static double GetTimeDelay(double pathlength, double n)
        {
            return pathlength / (GlobalConstants.C / n);
        }

        public static double GetRho(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }
    }
}