using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EqualsEx.Tests
{
    [TestClass]
    public class EqualsExTests
    {

#region Test Objects
        internal class ObjectWithNoProperties {}

        internal class ObjectWithSingleValueType
        {
            public int ValueType { get; set; }
        }

        internal class ObjectWithSingleReferenceType
        {
            public object ReferenceType { get; set; }
        }
#endregion

        [TestMethod]
        public void SameObject()
        {
            var obj = new ObjectWithNoProperties();
            var result = obj.EqualsEx(obj);
            Assert.AreEqual(MatchType.ReferenceMatch, result.ComparisonResult);
            Assert.IsNull(result.Properties);
        }

        [TestMethod]
        public void TwoObjects()
        {
            var obj1 = new ObjectWithNoProperties();
            var obj2 = new ObjectWithNoProperties();
            var result = obj1.EqualsEx(obj2);
            Assert.AreEqual(MatchType.NoMatch, result.ComparisonResult);
            Assert.IsNull(result.Properties);
        }

        [TestMethod]
        public void EqualsExWithTwoObjectsWithSameValueProperty__True()
        {
            var obj1 = new ObjectWithSingleValueType { ValueType = 1 };
            var obj2 = new ObjectWithSingleValueType { ValueType = 1 };
            var result = obj1.EqualsEx(obj2);
            Assert.AreEqual(MatchType.NoMatch, result.ComparisonResult);
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("ValueType"));
            var childResult = result.Properties["ValueType"];
            Assert.AreEqual(MatchType.ValueMatch, childResult.ComparisonResult);
        }

        [TestMethod]
        public void EqualsExWithTwoObjectsWithDifferentValueProperty__False()
        {
            var obj1 = new ObjectWithSingleValueType { ValueType = 1 };
            var obj2 = new ObjectWithSingleValueType { ValueType = 2 };
            var result = obj1.EqualsEx(obj2);
            Assert.AreEqual(MatchType.NoMatch, result.ComparisonResult);
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("ValueType"));
            var childResult = result.Properties["ValueType"];
            Assert.AreEqual(MatchType.NoMatch, childResult.ComparisonResult);
        }

        [TestMethod]
        public void EqualsExWithTwoObjectsWithSameReferenceProperty__True()
        {
            var sharedObject = new object();
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = sharedObject };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = sharedObject };
            var result = obj1.EqualsEx(obj2);
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("ReferenceType"));
            var childResult = result.Properties["ReferenceType"];
            Assert.AreEqual(MatchType.ReferenceMatch, childResult.ComparisonResult);
            Assert.IsNull(childResult.Properties);
        }

        [TestMethod]
        public void EqualsExWithTwoObjectsWithDifferentReferenceProperty__False()
        {
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = new object() };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = new object() };
            var result = obj1.EqualsEx(obj2);
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("ReferenceType"));
            var childResult = result.Properties["ReferenceType"];
            Assert.AreEqual(MatchType.NoMatch, childResult.ComparisonResult);
            Assert.IsNull(childResult.Properties);
        }

        [TestMethod]
        public void EqualsExWithTwoObjectsWithDifferentReferencePropertyContainingSameValueProperty()
        {
            var childObj1 = new ObjectWithSingleValueType() { ValueType = 1 };
            var childObj2 = new ObjectWithSingleValueType() { ValueType = 1 };
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = childObj1 };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = childObj2 };
           
            var result = obj1.EqualsEx(obj2);
            
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("ReferenceType"));

            var childResult = result.Properties["ReferenceType"];
            Assert.AreEqual(MatchType.NoMatch, childResult.ComparisonResult);
            Assert.IsNotNull(childResult.Properties);
            Assert.AreEqual(1, childResult.Properties.Count);
            Assert.IsTrue(childResult.Properties.ContainsKey("ValueType"));

            var childChildResult = childResult.Properties["ValueType"];
            Assert.AreEqual(MatchType.ValueMatch, childChildResult.ComparisonResult);
            Assert.IsNull(childChildResult.Properties);
        }

        [TestMethod]
        public void EqualsExWithTwoObjectsWithDifferentReferencePropertyContainingSameReferenceProperty()
        {
            var valueTypeWrappingObject = new ObjectWithSingleValueType() { ValueType = 1 };
            var childObj1 = new ObjectWithSingleReferenceType() { ReferenceType = valueTypeWrappingObject };
            var childObj2 = new ObjectWithSingleReferenceType() { ReferenceType = valueTypeWrappingObject };
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = childObj1 };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = childObj2 };
           
            var result = obj1.EqualsEx(obj2);
            
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("ReferenceType"));

            var childResult = result.Properties["ReferenceType"];
            Assert.AreEqual(MatchType.NoMatch, childResult.ComparisonResult);
            Assert.IsNotNull(childResult.Properties);
            Assert.AreEqual(1, childResult.Properties.Count);
            Assert.IsTrue(childResult.Properties.ContainsKey("ReferenceType"));

            var childChildResult = childResult.Properties["ReferenceType"];
            Assert.AreEqual(MatchType.ReferenceMatch, childChildResult.ComparisonResult);
            Assert.IsNull(childChildResult.Properties);
        }
    }
}
