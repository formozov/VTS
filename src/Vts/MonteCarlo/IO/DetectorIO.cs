﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vts.IO;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.IO
{
    public static class DetectorIO
    {
        /// <summary>
        /// Writes Detector xml for scalar detectors, writes Detector xml and 
        /// binary for 1D and larger detectors.  Detector.Name is used for filename.
        /// </summary>
        /// <param name="detector"></param>
        /// <param name="folderPath"></param>
        public static void WriteDetectorToFile(IDetector detector, string folderPath)
        {
            try
            {
                string filePath = folderPath + @"/" + detector.Name;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (detector is IDetector<double>)
                {
                    var d = detector as IDetector<double>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                }
                if (detector is IDetector<double[]>)
                {
                    var d = detector as IDetector<double[]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
                }
                if (detector is IDetector<double[,]>)
                {
                    var d = detector as IDetector<double[,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath, false);
                }
                if (detector is IDetector<double[, ,]>)
                {
                    var d = detector as IDetector<double[, ,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
                }
                if (detector is IDetector<double[, , ,]>)
                {
                    var d = detector as IDetector<double[, , ,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
                }
                if (detector is IDetector<double[, , , ,]>)
                {
                    var d = detector as IDetector<double[, , , ,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<double>(d.Mean, filePath);
                }
                if (detector is IDetector<Complex>)
                {
                    var d = detector as IDetector<Complex>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                }
                if (detector is IDetector<Complex[]>)
                {
                    var d = detector as IDetector<Complex[]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
                }
                if (detector is IDetector<Complex[,]>)
                {
                    var d = detector as IDetector<Complex[,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
                }
                if (detector is IDetector<Complex[, ,]>)
                {
                    var d = detector as IDetector<Complex[, ,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
                }
                if (detector is IDetector<Complex[, , ,]>)
                {
                    var d = detector as IDetector<Complex[, , ,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
                }
                if (detector is IDetector<Complex[, , , ,]>)
                {
                    var d = detector as IDetector<Complex[, , , ,]>;
                    FileIO.WriteToXML(d, filePath + ".xml");
                    FileIO.WriteArrayToBinary<Complex>(d.Mean, filePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem writing detector information to file.\n\nDetails:\n\n" + e + "\n");
            }
        }
        /// <summary>
        /// Reads Detector from File with given fileName.
        /// </summary>
        /// <param name="tallyType"></param>
        /// <param name="fileName"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static IDetector ReadDetectorFromFile(TallyType tallyType, string fileName, string folderPath)
        {
            try
            {
                string filePath = folderPath + @"/" + fileName;
                switch (tallyType)
                {
                    // "0D" detectors
                    case TallyType.RDiffuse:
                        return FileIO.ReadFromXML<RDiffuseDetector>(filePath + ".xml");

                    case TallyType.TDiffuse:
                        return FileIO.ReadFromXML<TDiffuseDetector>(filePath + ".xml");

                    case TallyType.ATotal:
                        return FileIO.ReadFromXML<ATotalDetector>(filePath + ".xml");

                    // "1D" detectors
                    case TallyType.ROfRho:
                        var rOfRhoDetector = FileIO.ReadFromXML<ROfRhoDetector>(filePath + ".xml");
                        rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                        return rOfRhoDetector;

                    case TallyType.pMCMuaMusROfRho:
                        var pMuaMusInROfRhoDetector = FileIO.ReadFromXML<pMCMuaMusROfRhoDetector>(filePath + ".xml");
                        pMuaMusInROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                        return pMuaMusInROfRhoDetector;

                    case TallyType.TOfRho:
                        var tOfRhoDetector = FileIO.ReadFromXML<TOfRhoDetector>(filePath + ".xml");
                        tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                        return tOfRhoDetector;

                    case TallyType.ROfAngle:
                        var rOfAngleDetector = FileIO.ReadFromXML<ROfAngleDetector>(filePath + ".xml");
                        rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                        return rOfAngleDetector;

                    case TallyType.TOfAngle:
                        var tOfAngleDetector = FileIO.ReadFromXML<TOfAngleDetector>(filePath + ".xml");
                        tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinary<double>(filePath);
                        return tOfAngleDetector;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
                        var rOfRhoAndTimeDetector = FileIO.ReadFromXML<ROfRhoAndTimeDetector>(filePath + ".xml");
                        var dims = new int[] {rOfRhoAndTimeDetector.Rho.Count, rOfRhoAndTimeDetector.Time.Count};
                        rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath, dims);
                        return rOfRhoAndTimeDetector;

                    case TallyType.pMCMuaMusROfRhoAndTime:
                        var pMuaMusInROfRhoAndTimeDetector =
                            FileIO.ReadFromXML<pMCMuaMusROfRhoAndTimeDetector>(filePath + ".xml");
                        pMuaMusInROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return pMuaMusInROfRhoAndTimeDetector;

                    case TallyType.ROfRhoAndAngle:
                        var rOfRhoAndAngleDetector = FileIO.ReadFromXML<ROfRhoAndAngleDetector>(filePath + ".xml");
                        rOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return rOfRhoAndAngleDetector;

                    case TallyType.TOfRhoAndAngle:
                        var tOfRhoAndAngleDetector = FileIO.ReadFromXML<TOfRhoAndAngleDetector>(filePath + ".xml");
                        tOfRhoAndAngleDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return tOfRhoAndAngleDetector;

                    case TallyType.ROfRhoAndOmega:
                        var rOfRhoAndOmegaDetector = FileIO.ReadFromXML<ROfRhoAndOmegaDetector>(filePath + ".xml");
                        rOfRhoAndOmegaDetector.Mean = (Complex[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return rOfRhoAndOmegaDetector;

                    case TallyType.ROfXAndY:
                        var rOfXAndYDetector = FileIO.ReadFromXML<ROfXAndYDetector>(filePath + ".xml");
                        rOfXAndYDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return rOfXAndYDetector;

                    case TallyType.FluenceOfRhoAndZ:
                        var fluenceOfRhoAndZDetector = FileIO.ReadFromXML<FluenceOfRhoAndZDetector>(filePath + ".xml");
                        fluenceOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return fluenceOfRhoAndZDetector;

                    case TallyType.AOfRhoAndZ:
                        var aOfRhoAndZDetector = FileIO.ReadFromXML<AOfRhoAndZDetector>(filePath + ".xml");
                        aOfRhoAndZDetector.Mean = (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return aOfRhoAndZDetector;

                    case TallyType.MomentumTransferOfRhoAndZ:
                        var momentumTransferOfRhoAndZDetector =
                            FileIO.ReadFromXML<MomentumTransferOfRhoAndZDetector>(filePath + ".xml");
                        momentumTransferOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return momentumTransferOfRhoAndZDetector;

                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
                        var fluenceOfRhoAndZAndTimeDetector =
                            FileIO.ReadFromXML<FluenceOfRhoAndZAndTimeDetector>(filePath + ".xml");
                        fluenceOfRhoAndZAndTimeDetector.Mean = (double[, ,])FileIO.ReadArrayFromBinary<double>(filePath);
                        return fluenceOfRhoAndZAndTimeDetector;

                    default:
                        throw new ArgumentOutOfRangeException("tallyType");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
        /// <summary>
        /// Reads Detector from file with default fileName (TallyType.ToString).
        /// </summary>
        /// <param name="tallyType"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static IDetector ReadDetectorFromFile(TallyType tallyType, string folderPath)
        {
            return ReadDetectorFromFile(tallyType, tallyType.ToString(), folderPath);
        }
        /// <summary>
        /// Reads Detector from a file in resources using given fileName.
        /// </summary>
        /// <param name="tallyType"></param>
        /// <param name="fileName"></param>
        /// <param name="folderPath"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string fileName, string folderPath, string projectName)
        {
            try
            {
                string filePath = folderPath + fileName;
                switch (tallyType)
                {
                    // "0D" detectors
                    case TallyType.RDiffuse:
                        return FileIO.ReadFromXMLInResources<RDiffuseDetector>(filePath + ".xml", projectName);

                    case TallyType.TDiffuse:
                        return FileIO.ReadFromXMLInResources<TDiffuseDetector>(filePath + ".xml", projectName);

                    case TallyType.ATotal:
                        return FileIO.ReadFromXMLInResources<ATotalDetector>(filePath + ".xml", projectName);

                    // "1D" detectors
                    case TallyType.ROfRho:
                        var rOfRhoDetector = FileIO.ReadFromXMLInResources<ROfRhoDetector>(filePath + ".xml", projectName);
                        var rOfRhoDetectorDims = new int[] { rOfRhoDetector.Rho.Count };
                        rOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoDetectorDims);
                        return rOfRhoDetector;

                    case TallyType.pMCMuaMusROfRho:
                        var pMuaMusROfRhoDetector = FileIO.ReadFromXMLInResources<pMCMuaMusROfRhoDetector>(filePath + ".xml", projectName);
                        var pMCROfRhoDetectorDims = new int[] { pMuaMusROfRhoDetector.Rho.Count };
                        pMuaMusROfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, pMCROfRhoDetectorDims);
                        return pMuaMusROfRhoDetector;

                    case TallyType.TOfRho:
                        var tOfRhoDetector = FileIO.ReadFromXMLInResources<TOfRhoDetector>(filePath + ".xml", projectName);
                        tOfRhoDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName);
                        return tOfRhoDetector;

                    case TallyType.ROfAngle:
                        var rOfAngleDetector = FileIO.ReadFromXMLInResources<ROfAngleDetector>(filePath + ".xml", projectName);
                        var rOfAngleDetectorDims = new int[] { rOfAngleDetector.Angle.Count };
                        rOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfAngleDetectorDims);
                        return rOfAngleDetector;

                    case TallyType.TOfAngle:
                        var tOfAngleDetector = FileIO.ReadFromXMLInResources<TOfAngleDetector>(filePath + ".xml", projectName);
                        var tOfAngleDetectorDims = new int[] { tOfAngleDetector.Angle.Count };
                        tOfAngleDetector.Mean = (double[])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, tOfAngleDetectorDims);
                        return tOfAngleDetector;

                    // "2D" detectors
                    case TallyType.ROfRhoAndTime:
                        var rOfRhoAndTimeDetector = FileIO.ReadFromXMLInResources<ROfRhoAndTimeDetector>(filePath + ".xml", projectName);
                        var rOfRhoAndTimeDetectorDims = new int[] { rOfRhoAndTimeDetector.Rho.Count, rOfRhoAndTimeDetector.Time.Count };
                        rOfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndTimeDetectorDims);
                        return rOfRhoAndTimeDetector;

                    case TallyType.pMCMuaMusROfRhoAndTime:
                        var pMuaMusROfRhoAndTimeDetector = FileIO.ReadFromXMLInResources<pMCMuaMusROfRhoAndTimeDetector>(filePath + ".xml", projectName);
                        var pMCROfRhoAndTimeDetectorDims = new int[] { pMuaMusROfRhoAndTimeDetector.Rho.Count, pMuaMusROfRhoAndTimeDetector.Time.Count };
                        pMuaMusROfRhoAndTimeDetector.Mean = (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, pMCROfRhoAndTimeDetectorDims);
                        return pMuaMusROfRhoAndTimeDetector;

                    case TallyType.ROfRhoAndAngle:
                        var rOfRhoAndAngleDetector =
                            FileIO.ReadFromXMLInResources<ROfRhoAndAngleDetector>(filePath + ".xml", projectName);
                        var rOfRhoAndAngleDetectorDims = new int[] { rOfRhoAndAngleDetector.Rho.Count, rOfRhoAndAngleDetector.Angle.Count };
                        rOfRhoAndAngleDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndAngleDetectorDims);
                        return rOfRhoAndAngleDetector;

                    case TallyType.TOfRhoAndAngle:
                        var tOfRhoAndAngleDetector =
                            FileIO.ReadFromXMLInResources<TOfRhoAndAngleDetector>(filePath + ".xml", projectName);
                        var tOfRhoAndAngleDetectorDims = new int[] { tOfRhoAndAngleDetector.Rho.Count, tOfRhoAndAngleDetector.Angle.Count };
                        tOfRhoAndAngleDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, tOfRhoAndAngleDetectorDims);
                        return tOfRhoAndAngleDetector;

                    case TallyType.ROfRhoAndOmega:
                        var rOfRhoAndOmegaDetector =
                            FileIO.ReadFromXMLInResources<ROfRhoAndOmegaDetector>(filePath + ".xml", projectName);
                        var rOfRhoAndOmegaDetectorDims = new int[] { rOfRhoAndOmegaDetector.Rho.Count, rOfRhoAndOmegaDetector.Omega.Count };
                        rOfRhoAndOmegaDetector.Mean =
                            (Complex[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfRhoAndOmegaDetectorDims);
                        return rOfRhoAndOmegaDetector;

                    case TallyType.ROfXAndY:
                        var rOfXAndYDetector = FileIO.ReadFromXMLInResources<ROfXAndYDetector>(filePath + ".xml",
                                                                                               projectName);
                        var rOfXAndYDetectorDims = new int[] { rOfXAndYDetector.X.Count, rOfXAndYDetector.Y.Count };
                        rOfXAndYDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, rOfXAndYDetectorDims);
                        return rOfXAndYDetector;

                    case TallyType.FluenceOfRhoAndZ:
                        var fluenceOfRhoAndZDetector =
                            FileIO.ReadFromXMLInResources<FluenceOfRhoAndZDetector>(filePath + ".xml", projectName);
                        var fluenceOfRhoAndZDetectorDims = new int[] { fluenceOfRhoAndZDetector.Rho.Count, fluenceOfRhoAndZDetector.Z.Count };
                        fluenceOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfRhoAndZDetectorDims);
                        return fluenceOfRhoAndZDetector;

                    case TallyType.AOfRhoAndZ:
                        var aOfRhoAndZDetector = FileIO.ReadFromXMLInResources<AOfRhoAndZDetector>(filePath + ".xml",
                                                                                                   projectName);
                        var aOfRhoAndZDetectorDims = new int[] { aOfRhoAndZDetector.Rho.Count, aOfRhoAndZDetector.Z.Count };
                        aOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, aOfRhoAndZDetectorDims);
                        return aOfRhoAndZDetector;

                    case TallyType.MomentumTransferOfRhoAndZ:
                        var momentumTransferOfRhoAndZDetector =
                            FileIO.ReadFromXMLInResources<MomentumTransferOfRhoAndZDetector>(filePath + ".xml",
                                                                                             projectName);
                        var momentumTransferOfRhoAndZDetectorDims = 
                            new int[] { momentumTransferOfRhoAndZDetector.Rho.Count, momentumTransferOfRhoAndZDetector.Z.Count };
                        momentumTransferOfRhoAndZDetector.Mean =
                            (double[,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, momentumTransferOfRhoAndZDetectorDims);
                        return momentumTransferOfRhoAndZDetector;

                    // "3D" detectors
                    case TallyType.FluenceOfRhoAndZAndTime:
                        var fluenceOfRhoAndZAndTimeDetector =
                            FileIO.ReadFromXMLInResources<FluenceOfRhoAndZAndTimeDetector>(filePath + ".xml",
                                                                                           projectName);
                        var fluenceOfRhoAndZAndTimeDetectorDims = 
                            new int[] { fluenceOfRhoAndZAndTimeDetector.Rho.Count, 
                                        fluenceOfRhoAndZAndTimeDetector.Z.Count,
                                        fluenceOfRhoAndZAndTimeDetector.Time.Count };
                        fluenceOfRhoAndZAndTimeDetector.Mean =
                            (double[, ,])FileIO.ReadArrayFromBinaryInResources<double>(filePath, projectName, fluenceOfRhoAndZAndTimeDetectorDims);
                        return fluenceOfRhoAndZAndTimeDetector;

                    default:
                        throw new ArgumentOutOfRangeException("tallyType");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem reading detector information from resource file.\n\nDetails:\n\n" + e + "\n");
            }

            return null;
        }
        /// <summary>
        /// Reads Detector from file in resources using default name (TallyType.ToString).
        /// </summary>
        /// <param name="tallyType"></param>
        /// <param name="folderPath"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public static IDetector ReadDetectorFromFileInResources(TallyType tallyType, string folderPath, string projectName)
        {
            return ReadDetectorFromFileInResources(tallyType, tallyType.ToString(), folderPath, projectName);
        }
    }
}
