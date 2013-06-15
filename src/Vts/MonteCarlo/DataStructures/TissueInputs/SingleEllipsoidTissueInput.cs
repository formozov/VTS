using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Implements ITissueInput.  Defines input to SingleEllipsoidTissue class.
    /// </summary>
    [KnownType(typeof(EllipsoidRegion))]
    [KnownType(typeof(LayerRegion))]
    [KnownType(typeof(OpticalProperties))]
    [KnownType(typeof(HenyeyGreensteinPhaseFunctionInput))]
    [KnownType(typeof(LookupTablePhaseFunctionInput))]
    [KnownType(typeof(BidirectionalPhaseFunctionInput))]
    public class SingleEllipsoidTissueInput : ITissueInput
    {
        private ITissueRegion _ellipsoidRegion;
        private ITissueRegion[] _layerRegions;

        /// <summary>
        /// allows definition of single ellipsoid tissue
        /// </summary>
        /// <param name="ellipsoidRegion">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleEllipsoidTissueInput(
            ITissueRegion ellipsoidRegion, 
            ITissueRegion[] layerRegions)
        {
            _ellipsoidRegion = ellipsoidRegion;
            _layerRegions = layerRegions;
            RegionPhaseFunctionInputs = new Dictionary<string, IPhaseFunctionInput>();
        }

        /// <summary>
        /// allows definition of single ellipsoid tissue & sets the phase function inputs
        /// </summary>
        /// <param name="ellipsoidRegion">ellipsoid region specification</param>
        /// <param name="layerRegions">tissue layer specification</param>
        public SingleEllipsoidTissueInput(
            ITissueRegion ellipsoidRegion,
            ITissueRegion[] layerRegions, 
            IDictionary<string, IPhaseFunctionInput>regionPhaseFunctionInputs)
        {
            _ellipsoidRegion = ellipsoidRegion;
            _layerRegions = layerRegions;
            RegionPhaseFunctionInputs = regionPhaseFunctionInputs;
        }

        /// <summary>
        /// SingleEllipsoidTissueInput default constructor provides homogeneous tissue with single ellipsoid
        /// with radius 0.5mm and center (0,0,1)
        /// </summary>
        public SingleEllipsoidTissueInput()
            : this(
                new EllipsoidRegion(
                    new Position(0, 0, 1), 
                    0.5, 
                    0.5, 
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey1"
                ),
                new ITissueRegion[] 
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey1"),
                    new LayerRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1")
                })
        {
            if (!RegionPhaseFunctionInputs.ContainsKey("HenyeyGreensteinKey1"))
                RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
        }
        /// <summary>
        /// Dictionary that contains all the phase function inputs
        /// </summary>
        public IDictionary<string, IPhaseFunctionInput> RegionPhaseFunctionInputs { get; set; }
        /// <summary>
        /// tissue type
        /// </summary>
        [IgnoreDataMember]
        public TissueType TissueType { get { return TissueType.SingleEllipsoid; } }
        /// <summary>
        /// regions of tissue (layers and ellipsoid)
        /// </summary>
        [IgnoreDataMember]
        public ITissueRegion[] Regions { get { return _layerRegions.Concat(_ellipsoidRegion).ToArray(); } }
        /// <summary>
        /// tissue ellipsoid region
        /// </summary>
        public ITissueRegion EllipsoidRegion { get { return _ellipsoidRegion; } set { _ellipsoidRegion = value; } }
        /// <summary>
        /// tissue layer regions
        /// </summary>
        public ITissueRegion[] LayerRegions { get { return _layerRegions; } set { _layerRegions = value; } }
       
    }
}
