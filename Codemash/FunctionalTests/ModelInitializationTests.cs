﻿using System;
using System.Linq;
using Codemash.Api.Data.Repositories.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FunctionalTests
{
    [TestClass]
    public class ModelInitializationTests
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void test_that_all_tables_are_empty()
        {
            try
            {
                using (var context = new CodemashContext())
                {
                    Assert.AreEqual(0, context.SessionChanges.Count());
                    Assert.AreEqual(0, context.Sessions.Count());
                    Assert.AreEqual(0, context.Speakers.Count());
                    Assert.AreEqual(0, context.SpeakerChanges.Count());
                    Assert.IsTrue(true);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail("Exception Occured");
            }
        }
    }
}
