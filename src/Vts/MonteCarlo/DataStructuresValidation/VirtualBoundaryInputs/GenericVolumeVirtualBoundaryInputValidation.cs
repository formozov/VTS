using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the DetectorInputs in this class are volume type detectors. 
    /// </summary>
    /// <param name="layers"></param>
    public class GenericVolumeVirtualBoundaryInputValidation
    {
        public static ValidationResult ValidateInput(IVirtualBoundaryInput vbInput)
        {
            foreach (var detectorInput in vbInput.DetectorInputs)
	        {
                if (!detectorInput.TallyType.IsVolumeTally())
                {
                    return new ValidationResult(
                        false,
                        "VolumeVirtualBoundaryInput: detector input is not a volume type",
                        "Make sure IList<IDetectorInput> only contains volume type tallies");
                }
                if (vbInput.VirtualBoundaryType.IsGenericVolumeVirtualBoundary() &&
                    !detectorInput.TallyType.IsVolumeTally())
                {
                    return new ValidationResult(
                        false,
                        "VolumeVirtualBoundaryInput: detector input is not consistent with virtual boundary type",
                        "Make sure virtual boundary type matches type of detector input");
                }
            }

            return new ValidationResult(
                true,
                "detector inputs must match type of virtual boundary input");
        }
    }
}