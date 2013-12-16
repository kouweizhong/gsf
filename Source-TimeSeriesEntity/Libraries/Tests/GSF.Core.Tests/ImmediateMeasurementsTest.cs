﻿#region [ Code Modification History ]
/*
 * 04/23/2012 Denis Kholine
 *   Generated original version of source code.
 *
 * 06/04/2012 Denis Kholine
 *   Update collections comparison unit tests.
 */
#endregion

#region  [ UIUC NCSA Open Source License ]
/*
Copyright © <2012> <University of Illinois>
All rights reserved.

Developed by: <ITI>
<University of Illinois>
<http://www.iti.illinois.edu/>
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal with the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
• Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimers.
• Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimers in the documentation and/or other materials provided with the distribution.
• Neither the names of <Name of Development Group, Name of Institution>, nor the names of its contributors may be used to endorse or promote products derived from this Software without specific prior written permission.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE CONTRIBUTORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS WITH THE SOFTWARE.
*/
#endregion

#region [ Using ]
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GSF.TimeSeries;
using GSF.Units;
using GSF;
#endregion

namespace TimeSeriesFramework.UnitTests
{
    /// <summary>
    /// This is a test class for ImmediateMeasurementsTest and is intended to represents the absolute latest measurement values received by a ConcentratorBase implementation.
    ///</summary>
    [TestClass()]
    public class ImmediateMeasurementsTest
    {
        #region [ Delegates ]
        public Ticks tf_RealTimeFunction()
        {
            return new Ticks(DateTime.UtcNow.Ticks);
        }

        #endregion [ Delegates ]

        #region [ Class Wrappers ]
        private class ConcentratorBaseWrapper : ConcentratorBase
        {
            public ConcentratorBaseWrapper()
                : base()
            { }

            protected override void PublishFrame(IFrame frame, int index)
            { }
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Gets or sets the allowed past time deviation tolerance, in seconds (can be subsecond).
        /// </summary>
        /// <remarks>
        /// <para>Defines the time sensitivity to past measurement timestamps.</para>
        /// <para>The number of seconds allowed before assuming a measurement timestamp is too old.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">LagTime must be greater than zero, but it can be less than one.</exception>
        public double LagTime;

        /// <summary>
        /// Gets or sets the allowed future time deviation tolerance, in seconds (can be subsecond).
        /// </summary>
        /// <remarks>
        /// <para>Defines the time sensitivity to future measurement timestamps.</para>
        /// <para>The number of seconds allowed before assuming a measurement timestamp is too advanced.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">LeadTime must be greater than zero, but it can be less than one.</exception>
        public double LeadTime;

        /// <summary>
        /// Returns the maximum value of all measurements.
        /// </summary>
        /// <remarks>This is only useful if all measurements represent the same type of measurement.</remarks>
        public double Maximum;

        /// <summary>
        /// Measurement
        /// </summary>
        public IMeasurement measurement;

        /// <summary>
        /// Returns collection of measurement ID's.
        /// </summary>
        public ICollection<Guid> MeasurementIDs;

        /// <summary>
        /// Returns the minimum value of all measurements.
        /// </summary>
        /// <remarks>This is only useful if all measurements represent the same type of measurement.</remarks>
        public double Minimum;

        /// <summary>
        /// Gets or sets function to return real-time.
        /// </summary>
        public Func<Ticks> RealTimeFunction;

        ///<summary>
        ///Returns ID collection for measurement tags.
        ///</summary>
        public ICollection<string> Tags;

        /// <summary>
        /// Ticks
        /// </summary>
        public Ticks ticks;

        /// <summary>
        /// measurement value
        /// </summary>
        public double value;

        /// <summary>
        /// expected value
        /// </summary>
        private bool expected;

        /// <summary>
        /// Guid
        /// </summary>
        private Guid id;

        /// <summary>
        /// ConcentratorBase
        /// </summary>
        private ConcentratorBaseWrapper parent;

        /// <summary>
        /// Measurement tag
        /// </summary>
        private String Tag;

        /// <summary>
        /// tagged measurements
        /// </summary>
        private DataTable taggedMeasurements;

        /// <summary>
        /// Target to test
        /// </summary>
        private ImmediateMeasurements target;

        /// <summary>
        /// We retrieve adjusted measurement values within time tolerance of concentrator real-time.
        /// </summary>
        /// <param name="id">A <see cref="Guid"/> representing the measurement ID.</param>
        /// <returns>A <see cref="Double"/> representing the adjusted measurement value.</returns>
        public double this[Guid id] { get { return this[id]; } }

        #endregion

        #region [ Context ]
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region
        /// <summary>
        /// A test for AddTaggedMeasurement
        /// Associates a new measurement ID with a tag, creating the new tag if needed.
        /// Allows you to define "grouped" points so you can aggregate certain measurements.
        ///</summary>
        [TestMethod()]
        public void AddTaggedMeasurementTest()
        {
            target.AddTaggedMeasurement(Tag, id);
            expected = (target.Tags.Count > 0);
            Assert.IsTrue(expected);
        }

        /// <summary>
        /// A test for CalculateAverage
        ///</summary>
        [TestMethod()]
        public void CalculateAverageTest()
        {
            int count = 0;
            double actual = target.CalculateAverage(ref count);
            expected = (actual == 102F);
            Assert.IsTrue(expected);
        }

        /// <summary>
        ///A test for CalculateTagAverage
        ///</summary>
        [TestMethod()]
        public void CalculateTagAverageTest()
        {
            int count = 0;
            double actual = target.CalculateTagAverage(Tag, ref count);
            expected = (actual == 102F);
            Assert.IsTrue(expected);
        }

        /// <summary>
        /// A test for ClearMeasurementCache
        /// Clears the existing measurement cache.
        ///</summary>
        [TestMethod()]
        public void ClearMeasurementCacheTest()
        {
            target.ClearMeasurementCache();
            expected = (target.MeasurementIDs.Count == 0);
            Assert.IsTrue(expected);
        }

        /// <summary>
        /// A test for DefineTaggedMeasurements
        /// Defines tagged measurements from a data table.
        /// Expects String based tag field to be aliased
        /// as "Tag" and Guid based measurement ID field to be aliased as "ID".
        ///</summary>
        [TestMethod()]
        public void DefineTaggedMeasurementsTest()
        {
            this.taggedMeasurements = new DataTable("Measurements");
            this.taggedMeasurements.Columns.Add("Tag");
            this.taggedMeasurements.Columns.Add("ID");
            this.taggedMeasurements.Rows.Add(new object[2] { measurement.TagName, measurement.ID });

            target.DefineTaggedMeasurements(this.taggedMeasurements);
            expected = (target.Measurement(this.id).ID == this.id);
            Assert.IsTrue(expected);
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            try
            {
                target.Dispose();
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.IsTrue(false);
            }
        }

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        public Ticks f_RealTimeFunction()
        {
            return DateTime.UtcNow.Ticks;
        }

        /// <summary>
        ///A test for IEnumerable<TemporalMeasurement>.GetEnumerator
        ///</summary>
        [TestMethod()]
        public void GetEnumeratorTest()
        {
            IEnumerable<TemporalMeasurement> target = new ImmediateMeasurements();
            Assert.IsNotNull(target.GetEnumerator());
        }

        /// <summary>
        ///A test for IEnumerable.GetEnumerator
        ///</summary>
        [TestMethod()]
        public void GetEnumeratorTest1()
        {
            IEnumerable target = new ImmediateMeasurements();
            Assert.IsNotNull(target.GetEnumerator());
        }

        /// <summary>
        /// A test for ImmediateMeasurements Constructor
        /// Represents the absolute latest measurement values received by a ConcentratorBase
        /// implementation.
        ///</summary>
        [TestMethod()]
        public void ImmediateMeasurementsConstructorTest()
        {
            Assert.IsInstanceOfType(target, typeof(ImmediateMeasurements));
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ImmediateMeasurements Constructor
        ///</summary>
        [TestMethod()]
        public void ImmediateMeasurementsConstructorTest1()
        {
            Assert.IsNotNull(target);
            Assert.IsInstanceOfType(target, typeof(ImmediateMeasurements));
        }

        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void ItemTest()
        {
            target.Measurement(this.id).Value = this.value;
            double actual;
            actual = target.Measurement(this.id).Value;
            expected = (actual == this.value);
            Assert.IsTrue(expected);
        }

        /// <summary>
        ///A test for LagTime
        ///</summary>
        [TestMethod()]
        public void LagTimeTest()
        {
            Assert.AreEqual(this.LagTime, target.LagTime);
        }

        /// <summary>
        ///A test for LeadTime
        ///</summary>
        [TestMethod()]
        public void LeadTimeTest()
        {
            Assert.AreEqual(this.LeadTime, target.LeadTime);
        }

        /// <summary>
        /// A test for Maximum
        ///</summary>
        [TestMethod()]
        public void MaximumTest()
        {
            double actual;
            actual = target.Maximum;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// A test for MeasurementIDs
        ///</summary>
        [TestMethod()]
        public void MeasurementIDsTest()
        {
            expected = true;
            foreach (Guid ID in target.MeasurementIDs)
            {
                if (ID != this.id)
                {
                    expected = false;
                }
            }
            Assert.IsTrue(expected);
        }

        /// <summary>
        /// A test for Measurement
        ///</summary>
        [TestMethod()]
        public void MeasurementTest()
        {
            expected = (target.Measurement(id).ID == id);
            Assert.IsTrue(expected);
        }

        /// <summary>
        ///A test for Measurement
        ///</summary>
        [TestMethod()]
        public void MeasurementTest1()
        {
            IMeasurement measurement = new Measurement();
            TemporalMeasurement expected = new TemporalMeasurement(measurement, LagTime, LeadTime);
            TemporalMeasurement actual;
            actual = target.Measurement(measurement);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Minimum
        ///</summary>
        [TestMethod()]
        public void MinimumTest()
        {
            double actual;
            actual = target.Minimum;
            expected = (actual > 0);
            Assert.IsTrue(expected);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            this.target.Dispose();
            this.Tags.Clear();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            this.parent = new ConcentratorBaseWrapper();
            this.ticks = Ticks.Parse(DateTime.UtcNow.Ticks.ToString());
            this.measurement = new Measurement();
            this.measurement.TagName = "UnitTest";
            this.measurement.Value = 102F;
            this.measurement.Timestamp = this.ticks;
            this.measurement.ID = Guid.Parse("9cdd4506-9490-11e1-922f-0024e856a96e");
            this.value = this.measurement.Value;
            this.id = this.measurement.ID;
            this.Tag = this.measurement.TagName;
            this.ticks = this.measurement.Timestamp;
            this.Tags = new List<String>();
            this.Tags.Add(this.measurement.TagName);
            this.target = new ImmediateMeasurements(parent);
            this.target.LagTime = this.ticks - 60 * Ticks.PerSecond;
            this.target.LeadTime = this.ticks + 60 * Ticks.PerSecond;
            this.LagTime = this.target.LagTime;
            this.LeadTime = this.target.LeadTime;
            this.target.RealTimeFunction = f_RealTimeFunction;
            this.RealTimeFunction = this.target.RealTimeFunction;
            this.target.Measurement(measurement);
            this.target.AddTaggedMeasurement(this.Tag, this.id);
            this.MeasurementIDs = target.MeasurementIDs;
            this.target.UpdateMeasurementValue(measurement);
            this.expected = false;
        }

        #endregion

        #region [ Methods ]
        /// <summary>
        ///A test for RealTimeFunction
        ///</summary>
        [TestMethod()]
        public void RealTimeFunctionTest()
        {
            Func<Ticks> expected = tf_RealTimeFunction;
            Func<Ticks> actual;
            target.RealTimeFunction = expected;
            actual = target.RealTimeFunction;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TaggedMeasurementKeys
        ///</summary>
        [TestMethod()]
        public void TaggedMeasurementKeysTest()
        {
            ReadOnlyCollection<Guid> actual;
            actual = target.TaggedMeasurementKeys(Tag);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// A test for TagMaximum
        /// Returns the maximum value of all measurements associated with the specified tag.
        ///</summary>
        [TestMethod()]
        public void TagMaximumTest()
        {
            double expected = 102;
            double actual;
            actual = target.TagMaximum(Tag);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TagMinimum
        ///</summary>
        [TestMethod()]
        public void TagMinimumTest()
        {
            double expected = 102F;
            double actual = target.TagMinimum(Tag);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Tags
        ///</summary>
        [TestMethod()]
        public void TagsTest()
        {
            expected = true;
            foreach (string Tag in target.Tags)
            {
                if (Tag != this.Tag)
                {
                    expected = false;
                }
            }
            Assert.IsTrue(expected);
        }

        /// <summary>
        ///A test for UpdateMeasurementValue
        ///</summary>
        [TestMethod()]
        public void UpdateMeasurementValueTest()
        {
            IMeasurement newMeasurement = new Measurement();
            newMeasurement.Value = this.value;
            newMeasurement.ID = this.id;
            newMeasurement.Timestamp = new Ticks(DateTime.UtcNow.Ticks);

            target.UpdateMeasurementValue(newMeasurement);

            expected = (target.Measurement(this.id).Value == this.value);
            Assert.IsTrue(expected);
        }

        #endregion
    }
}