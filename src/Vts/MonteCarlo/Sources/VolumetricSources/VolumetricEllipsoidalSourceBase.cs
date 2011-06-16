﻿using System;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources.SourceProfiles;

namespace Vts.MonteCarlo.Sources
{
    public abstract class VolumetricEllipsoidalSourceBase : ISource
    {
        protected ISourceProfile _sourceProfile;
        protected Direction _newDirectionOfPrincipalSourceAxis;
        protected Position _translationFromOrigin;
        protected SourceFlags _rotationAndTranslationFlags;
        protected double _aParameter;
        protected double _bParameter;
        protected double _cParameter;
        protected int _initialTissueRegionIndex;

        protected VolumetricEllipsoidalSourceBase(
            double aParameter,
            double bParameter,
            double cParameter,            
            ISourceProfile sourceProfile,
            Direction newDirectionOfPrincipalSourceAxis,
            Position translationFromOrigin,
            int initialTissueRegionIndex)
        {
            _rotationAndTranslationFlags = new SourceFlags(
               newDirectionOfPrincipalSourceAxis != SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
               translationFromOrigin != SourceDefaults.DefaultPosition.Clone(),
               false);

            _aParameter = aParameter;
            _bParameter = bParameter;
            _cParameter = cParameter;
            _sourceProfile = sourceProfile;
            _newDirectionOfPrincipalSourceAxis = newDirectionOfPrincipalSourceAxis.Clone();
            _translationFromOrigin = translationFromOrigin.Clone();
            _initialTissueRegionIndex = initialTissueRegionIndex;
        }

        public Photon GetNextPhoton(ITissue tissue)
        {
            //Source starts from anywhere in the ellipsoid
            Position finalPosition = GetFinalPositionFromProfileType(_sourceProfile, _aParameter, _bParameter, _cParameter, Rng);

            // sample angular distribution
            Direction finalDirection = GetFinalDirection();

            //Find the relevent polar and azimuthal pair for the direction
            PolarAzimuthalAngles _rotationalAnglesOfPrincipalSourceAxis = SourceToolbox.GetPolarAzimuthalPairFromDirection(_newDirectionOfPrincipalSourceAxis);

            //Rotation and translation
            SourceToolbox.UpdateDirectionPositionAfterGivenFlags(
                ref finalPosition,
                ref finalDirection,
                _rotationalAnglesOfPrincipalSourceAxis,
                _translationFromOrigin,
                _rotationAndTranslationFlags);

            var photon = new Photon(finalPosition, finalDirection, tissue, _initialTissueRegionIndex, Rng);

            return photon;
        }

        protected abstract Direction GetFinalDirection(); // position may or may not be needed

        private static Position GetFinalPositionFromProfileType(ISourceProfile sourceProfile, double aParameter, double bParameter, double cParameter, Random rng)
        {
            Position finalPosition = null;
            switch (sourceProfile.ProfileType)
            {
                case SourceProfileType.Flat:
                    // var flatProfile = sourceProfile as FlatSourceProfile;
                    SourceToolbox.GetPositionInAnEllipsoidRandomFlat(
                        SourceDefaults.DefaultPosition.Clone(),
                        2 * aParameter,
                        2 * bParameter,
                        2 * cParameter,
                        rng);
                    break;
                case SourceProfileType.Gaussian:
                    var gaussianProfile = sourceProfile as GaussianSourceProfile;
                    finalPosition = SourceToolbox.GetPositionInAnEllipsoidRandomGaussian(
                        SourceDefaults.DefaultPosition.Clone(),
                        2 * aParameter,
                        2 * bParameter,
                        2 * cParameter,
                        gaussianProfile.BeamDiaFWHM,
                        rng);
                    break;
            }
            return finalPosition;
        }

        #region Random number generator code (copy-paste into all sources)
        /// <summary>
        /// The random number generator used to create photons. If not assigned externally,
        /// a Mersenne Twister (MathNet.Numerics.Random.MersenneTwister) will be created with
        /// a seed of zero.
        /// </summary>
        public Random Rng
        {
            get
            {
                if (_rng == null)
                {
                    _rng = new MathNet.Numerics.Random.MersenneTwister(0);
                }
                return _rng;
            }
            set { _rng = value; }
        }
        private Random _rng;
        #endregion
    }
}