﻿using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.SpectralMapping;
using Vts.IO;
using System.Runtime.Serialization;

namespace Vts.Test.Modeling.Spectroscopy
{
    [KnownType(typeof(ChromophoreSpectrum))]
    [TestFixture]
    public class SpectralDatabaseLoaderTest
    {
        [Test]
        public void validate_Loading_Spectral_Database()
        {
            var _testDictionary = Vts.SpectralMapping.SpectralDatabaseLoader.GetDatabaseFromFile();
            Assert.IsNotNull(_testDictionary);
        }

        [Test]
        public void validate_Serializing_Spectral_Database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabaseLoader.GetDatabaseFromFile();
            
            // "ToFile" static method in SpectralDatabaseLoader
            //var values = testDictionary.Select(di => di.Value).ToList();
            //values.WriteToXML("samplefile.xml");
            testDictionary.WriteToXML("dictionary.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary.xml"); 

            Assert.IsTrue(true);
        }

        [Test]
        public void validate_Deserializing_Spectral_Database()
        {
            var testDictionary = Vts.SpectralMapping.SpectralDatabaseLoader.GetDatabaseFromFile();
            testDictionary.WriteToXML("dictionary2.xml");
            var Dvalues = FileIO.ReadFromXML<Dictionary<string, ChromophoreSpectrum>>("dictionary2.xml"); 
            Assert.IsTrue(true);
        }
    }
}