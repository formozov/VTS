﻿using NUnit.Framework;
using System;
using Vts.Modeling.ForwardSolvers;

namespace Vts.Test.Modeling.ForwardSolvers
{
    [TestFixture]
    public class pMCForwardSolverTests
    {
        /// <summary>
        /// Setup for the SolverFactory tests.
        /// </summary>
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Test solver construction 
        /// </summary>
        [Test]
        public void constructor_returns_valid_solver()
        {
            var fs = new pMCForwardSolver();
        }

        /// <summary>
        /// Tear down for the NurbsGenerator tests.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
        }
    }
}