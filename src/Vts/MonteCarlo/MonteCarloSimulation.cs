using System;
using System.Linq;
using System.Collections.Generic;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using System.IO;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for Monte Carlo simulation. 
    /// </summary>
    public class MonteCarloSimulation
    {
        public const double COS90D = 1.0E-6;
        public const double COSZERO = (1.0 - 1e-12);

        private ISource _source;
        private ITissue _tissue;
        private DetectorController _detectorController;
        private VirtualBoundaryController _virtualBoundaryController;
        private long _numberOfPhotons;
        private SimulationStatistics _simulationStatistics;

        protected SimulationInput _input;
        private Random _rng;

        private string _outputPath;

        public MonteCarloSimulation(SimulationInput input)
        {
            _outputPath = "";

            // all field/property defaults should be set here
            _input = input;

            var result = SimulationInputValidation.ValidateInput(_input);
            if (result.IsValid == false)
            {
                throw new ArgumentException(result.ValidationRule + (!string.IsNullOrEmpty(result.Remarks) ? "; " + result.Remarks : ""));
            }

            _numberOfPhotons = input.N;

            WRITE_DATABASES = input.Options.WriteDatabases; // modified ckh 4/9/11
            ABSORPTION_WEIGHTING = input.Options.AbsorptionWeightingType; // CKH add 12/14/09
            TRACK_STATISTICS = input.Options.TrackStatistics;
            if (TRACK_STATISTICS)
                _simulationStatistics = new SimulationStatistics();

            _rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                input.Options.RandomNumberGeneratorType, input.Options.Seed);

            this.SimulationIndex = input.Options.SimulationIndex;

            _tissue = TissueFactory.GetTissue(input.TissueInput, input.Options.AbsorptionWeightingType, input.Options.PhaseFunctionType);
            _source = SourceFactory.GetSource(input.SourceInput, _tissue, _rng);

            // instantiate detector controller for the detectors that apply to each virtual boundary 
            var detectors = DetectorFactory.GetDetectors(input.DetectorInputs, _tissue, input.Options.TallySecondMoment);
            _detectorController = DetectorControllerFactory.GetStandardDetectorController(detectors);

            _virtualBoundaryController = VirtualBoundaryControllerFactory.GetVirtualBoundaryController(
                _detectorController.Detectors, _tissue);
        }

        /// <summary>
        /// Default constructor to allow quick-and-easy simulation
        /// </summary>
        public MonteCarloSimulation() : this(new SimulationInput()) { }

        // private properties
        private int SimulationIndex { get; set; }

        // public properties
        private IList<DatabaseType> WRITE_DATABASES { get; set; }  // modified ckh 4/9/11
        private AbsorptionWeightingType ABSORPTION_WEIGHTING { get; set; }
        public PhaseFunctionType PHASE_FUNCTION { get; set; }
        private bool TRACK_STATISTICS { get; set; }

        public Output Results { get; private set; }

        public Output Run(string outputPath)
        {
            _outputPath = outputPath;
            return Run();
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        /// <returns></returns>
        public Output Run()
        {
            DisplayIntro();

            ExecuteMCLoop();

            Results = new Output(_input, _detectorController.Detectors);

            ReportResults();

            return Results;
        }

        /// <summary>
        /// Executes the Monte Carlo Loop
        /// </summary>
        protected virtual void ExecuteMCLoop()
        {
            PhotonDatabaseWriter terminationWriter = null;
            CollisionInfoDatabaseWriter collisionWriter = null;

            try
            {
                if (WRITE_DATABASES != null)
                {
                    if (WRITE_DATABASES.Contains(DatabaseType.PhotonExitDataPoints))
                    {
                        terminationWriter = new PhotonDatabaseWriter(
                            Path.Combine(_outputPath, _input.OutputName, "photonExitDatabase"));
                    }
                    if (WRITE_DATABASES.Contains(DatabaseType.CollisionInfo))
                    {
                        collisionWriter = new CollisionInfoDatabaseWriter(
                            Path.Combine(_outputPath, _input.OutputName, "collisionInfoDatabase"), _tissue.Regions.Count());
                    }
                }

                for (long n = 1; n <= _numberOfPhotons; n++)
                {
                    // todo: bug - num photons is assumed to be over 10 :)
                    if (n % (_numberOfPhotons / 10) == 0)
                    {
                        DisplayStatus(n, _numberOfPhotons);
                    }

                    var photon = _source.GetNextPhoton(_tissue);

                    do
                    { /* begin do while  */
                        photon.SetStepSize(); // only calls rng if SLeft == 0.0

                        //bool hitBoundary = photon.Move(distance);
                        BoundaryHitType hitType = Move(photon); // in-line?

                        // for each "hit" virtual boundary, tally respective detectors. 
                        if (hitType == BoundaryHitType.Virtual)
                        {
                            _virtualBoundaryController.TallyToTerminationDetectors(photon.DP);
                        }

                        // kill photon for various reasons, including possible VB crossings
                        photon.TestDeath(_virtualBoundaryController);

                        // check if virtual boundary 
                        if (hitType == BoundaryHitType.Virtual)
                        {
                            continue;
                        }

                        if (hitType == BoundaryHitType.Tissue)
                        {
                            photon.CrossRegionOrReflect();
                            continue;
                        }

                        photon.Absorb(); // can be added to TestDeath?
                        if (!photon.DP.StateFlag.Has(PhotonStateType.Absorbed))
                        {
                            photon.Scatter();
                        }

                    } while (photon.DP.StateFlag.Has(PhotonStateType.Alive)); /* end do while */

                    //_detectorController.TerminationTally(photon.DP);

                    if (terminationWriter != null)
                    {
                        //dc: how to check if detector contains DP  ckh: check is on reading side, may need to fix
                        terminationWriter.Write(photon.DP);
                    }
                    if (collisionWriter != null)
                    {
                        collisionWriter.Write(photon.History.SubRegionInfoList);
                    }

                    _virtualBoundaryController.TallyToHistoryDetectors(photon.History);

                    if (TRACK_STATISTICS)
                    {
                        _simulationStatistics.TrackStatistics(photon.History);
                    }

                } /* end of for n loop */
            }
            finally
            {
                if (terminationWriter != null) terminationWriter.Dispose();
                if (collisionWriter != null) collisionWriter.Dispose();
            }

            // normalize all detectors by the total number of photons (each tally records it's own "local" count as well)
            _detectorController.NormalizeDetectors(_numberOfPhotons);
            if (TRACK_STATISTICS)
            {
                _simulationStatistics.ToFile("statistics");
            }
        }

        private BoundaryHitType Move(Photon photon)
        {
            bool willHitTissueBoundary = false;

            // get distance to any tissue boundary
            var tissueDistance = _tissue.GetDistanceToBoundary(photon);
            // get distance to any VB

            double vbDistance = double.PositiveInfinity;
            var vb =_virtualBoundaryController.GetClosestVirtualBoundary(photon.DP, out vbDistance);

            if (tissueDistance < vbDistance) // determine if will hit tissue boundary first
            {
                var hitTissueBoundary = photon.Move(tissueDistance);
                return hitTissueBoundary ? BoundaryHitType.Tissue : BoundaryHitType.None;
            }
            else // otherwise, move to the closest virtual boundary
            {
                // if both tissueDistance and vbDistance are both infinity, then photon dead
                if (vbDistance == double.PositiveInfinity)
                {
                    photon.DP.StateFlag = photon.DP.StateFlag.Remove(PhotonStateType.Alive);
                    return BoundaryHitType.Virtual; // set to virtual so fall out of loop
                }
                else
                {
                    var hitVirtualBoundary = photon.Move(vbDistance);
                    photon.DP.StateFlag = photon.DP.StateFlag.Add(vb.PhotonStateType); // add pseudo-collision for vb
                    return hitVirtualBoundary ? BoundaryHitType.Virtual : BoundaryHitType.None;
                }
            }
        }

        public void ReportResults()
        {
            // write out how many photons written to each detector
            for (int i = 0; i < _detectorController.Detectors.Count; ++i)  
                Console.WriteLine(SimulationIndex + ": detector named {0} -> {1} photons written",
                    _detectorController.Detectors[i].TallyType, _detectorController.Detectors[i].TallyCount);
        }

        /********************************************************/
        void DisplayIntro()
        {
            var header = _input.OutputName + "(" + SimulationIndex + ")";
            string intro = "\n" +
                header + ":                                                  \n" +
                header + ":      Monte Carlo Simulation of Light Propagation \n" +
                header + ":              in a multi-region tissue            \n" +
                header + ":                                                  \n" +
                header + ":         written by the Virtual Photonics Team    \n" +
                header + ":              Beckman Laser Institute             \n" +
                header + ":";
            Console.WriteLine(intro);
        }

        /*****************************************************************/
        void DisplayStatus(long n, long num_phot)
        {
            var header = _input.OutputName + "(" + SimulationIndex + ")";
            /* fraction of photons completed */
            double frac = 100 * n / num_phot;
            Console.WriteLine(header + ": " + frac + " percent complete, " + DateTime.Now);
        }
    }
}
