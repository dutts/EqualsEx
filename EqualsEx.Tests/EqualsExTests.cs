using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

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

#region Test Helper
        /// <summary>
        /// Performs basic testing on the returned result object to reduce repeated code below, asserts on MatchType and contains
        /// a mapping between property name and an action to perform on that property's result object (usually a recursive call to 
        /// TestResultObject)
        /// </summary>
        /// <param name="result">The result object to test</param>
        /// <param name="expectedMatchType">The expected match type</param>
        /// <param name="propertyTests">A mapping of property name against an Action to carry out against that property</param>
        private void TestResultObject(EqualsExResult result, MatchType expectedMatchType, IDictionary<string, Action> propertyTests)
        {
            Assert.AreEqual(expectedMatchType, result.ComparisonResult);
            if (propertyTests == null)
                Assert.IsNull(result.Properties);
            else
            {
                foreach(var propertyTest in propertyTests)
                {
                    var propertyName = propertyTest.Key;
                    var propertyTestAction = propertyTest.Value;
                    Assert.IsTrue(result.Properties.ContainsKey(propertyName));
                    propertyTestAction();
                }
            }
        }
#endregion

        [TestMethod]
        public void SameObject__ExpectReferenceMatch()
        {
            var obj = new ObjectWithNoProperties();
            var result = obj.EqualsEx(obj);
            TestResultObject(result, MatchType.ReferenceMatch, null);
        }

        [TestMethod]
        public void TwoDifferentObjects__ExpectNoMatch()
        {
            var obj1 = new ObjectWithNoProperties();
            var obj2 = new ObjectWithNoProperties();
            var result = obj1.EqualsEx(obj2);
            TestResultObject(result, MatchType.NoMatch, null);
        }

        [TestMethod]
        public void TwoDifferentObjectsWithSameValueProperty__ExpectNoMatchNestedValueMatch()
        {
            var obj1 = new ObjectWithSingleValueType { ValueType = 1 };
            var obj2 = new ObjectWithSingleValueType { ValueType = 1 };
            var result = obj1.EqualsEx(obj2);

            TestResultObject(result, MatchType.NoMatch, new Dictionary<string, Action>()
            {
                { "ValueType",() => { TestResultObject(result.Properties["ValueType"], MatchType.ValueMatch, null); } }
            });
        }

        [TestMethod]
        public void TwoDifferentObjectsWithDifferentValueProperties__ExpectNoMatchNestedNoMatch()
        {
            var obj1 = new ObjectWithSingleValueType { ValueType = 1 };
            var obj2 = new ObjectWithSingleValueType { ValueType = 2 };
            var result = obj1.EqualsEx(obj2);

            TestResultObject(result, MatchType.NoMatch, new Dictionary<string, Action>()
            {
                { "ValueType",() => { TestResultObject(result.Properties["ValueType"], MatchType.NoMatch, null); } }
            });
        }

        [TestMethod]
        public void TwoDifferentObjectsWithSameReferenceProperty__ExpectNoMatchNestedReferenceMatch()
        {
            var sharedObject = new object();
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = sharedObject };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = sharedObject };
            var result = obj1.EqualsEx(obj2);
            
            TestResultObject(result, MatchType.NoMatch, new Dictionary<string, Action>()
            {
                { "ReferenceType",() => { TestResultObject(result.Properties["ReferenceType"], MatchType.ReferenceMatch, null); } }
            });
        }

        [TestMethod]
        public void TwoDifferentObjectsWithDifferentReferenceProperty__ExpectNoMatchNestedNoMatch()
        {
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = new object() };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = new object() };
            var result = obj1.EqualsEx(obj2);

            TestResultObject(result, MatchType.NoMatch, new Dictionary<string, Action>()
            {
                { "ReferenceType",() => { TestResultObject(result.Properties["ReferenceType"], MatchType.NoMatch, null); } }
            });
        }

        [TestMethod]
        public void TwoDifferentObjectsWithDifferentReferencePropertyContainingSameValueProperty__ExpectNoMatchNestedNoMatchNestedValueMatch()
        {
            var childObj1 = new ObjectWithSingleValueType() { ValueType = 1 };
            var childObj2 = new ObjectWithSingleValueType() { ValueType = 1 };
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = childObj1 };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = childObj2 };
            var result = obj1.EqualsEx(obj2);

            TestResultObject(result, MatchType.NoMatch, new Dictionary<string, Action>()
            {
                { "ReferenceType",() => { TestResultObject(result.Properties["ReferenceType"], MatchType.NoMatch, new Dictionary<string, Action>()
                    {
                        { "ValueType", () => { TestResultObject(result.Properties["ReferenceType"].Properties["ValueType"], MatchType.ValueMatch, null); } }
                    });
                }}
            });
        }

        [TestMethod]
        public void TwoDifferentObjectsWithDifferentReferencePropertyContainingSameReferenceProperty__ExpectNoMatchNestedNoMatchNestedReferenceMatch()
        {
            var valueTypeWrappingObject = new ObjectWithSingleValueType() { ValueType = 1 };
            var childObj1 = new ObjectWithSingleReferenceType() { ReferenceType = valueTypeWrappingObject };
            var childObj2 = new ObjectWithSingleReferenceType() { ReferenceType = valueTypeWrappingObject };
            var obj1 = new ObjectWithSingleReferenceType { ReferenceType = childObj1 };
            var obj2 = new ObjectWithSingleReferenceType { ReferenceType = childObj2 };
            var result = obj1.EqualsEx(obj2);
         
            TestResultObject(result, MatchType.NoMatch, new Dictionary<string, Action>()
            {
                { "ReferenceType",() => { TestResultObject(result.Properties["ReferenceType"], MatchType.NoMatch, new Dictionary<string, Action>()
                    {
                        { "ReferenceType", () => { TestResultObject(result.Properties["ReferenceType"].Properties["ReferenceType"], MatchType.ReferenceMatch, null); } }
                    });
                }}
            });
        }
    }
}
