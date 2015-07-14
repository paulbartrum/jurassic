using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    /// <summary>
    /// Test the global Date object.
    /// </summary>
    [TestClass]
    public class DateTests
    {
        [TestMethod]
        public void Constructor()
        {
            // Warm up the engine.
            TestUtils.Evaluate("new Date().valueOf()");

            // new Date() returns the current date - this test assumes the running time is less than 30ms.
            Assert.AreEqual((double)ToJSDate(DateTime.Now), (double)TestUtils.Evaluate("new Date().valueOf()"), 30);

            // new Date(milliseconds)
            Assert.AreEqual(0, TestUtils.Evaluate("new Date(0).valueOf()"));
            Assert.AreEqual(100, TestUtils.Evaluate("new Date(100).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(NaN).valueOf()"));
            Assert.AreEqual(1, TestUtils.Evaluate("new Date(true).valueOf()"));
            Assert.AreEqual(6, TestUtils.Evaluate("new Date(6.6).valueOf()"));
            Assert.AreEqual(-6, TestUtils.Evaluate("new Date(-6.6).valueOf()"));

            // new Date(dateStr)
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("new Date('5 Jan 2010').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), TestUtils.Evaluate("new Date('Sat, 24 Apr 2010').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), TestUtils.Evaluate("new Date(' Sat , 24  Apr  2010 ').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(1989, 12, 31)), TestUtils.Evaluate("new Date('31 Dec 89').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 15, 30, 01)), TestUtils.Evaluate("new Date('24 Apr 2010 15:30:01').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 3, 30, 01)), TestUtils.Evaluate("new Date('24 Apr 2010 3:30:01').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 2, 3, 1)), TestUtils.Evaluate("new Date('24 Apr 2010 2:3:1').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 UT').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 GMT').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 EST').valueOf()")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 3, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 EDT').valueOf()")); // -4
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 CST').valueOf()")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 CDT').valueOf()")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 MST').valueOf()")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 MDT').valueOf()")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 7, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 PST').valueOf()")); // -8
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 PDT').valueOf()")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 Z').valueOf()"));   // GMT
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 A').valueOf()"));   // -1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 11, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 M').valueOf()"));   // -12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 N').valueOf()"));   // +1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 Y').valueOf()"));   // +12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 +1100').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 29, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 +0030').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 29, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 +30').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 29, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 +90').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 GMT+1100').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:59 GMT+1100 (Zone Name)').valueOf()"));

            // new Date(year, month, [day], [hour], [minute], [second], [millisecond])
            // Note: month is 0-11 is javascript but 1-12 in .NET.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1)), TestUtils.Evaluate("new Date(2010, 0).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("new Date(2010, 0, 5).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 0, 0)), TestUtils.Evaluate("new Date(2010, 0, 5, 12).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 0)), TestUtils.Evaluate("new Date(2010, 0, 5, 12, 32).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45)), TestUtils.Evaluate("new Date(2010, 0, 5, 12, 32, 45).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45, 123)), TestUtils.Evaluate("new Date(2010, 0, 5, 12, 32, 45, 123).valueOf()"));
            
            // Test overflow.
            Assert.AreEqual(ToJSDate(new DateTime(2009, 12, 1)), TestUtils.Evaluate("new Date(2010, -1).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 1, 1)), TestUtils.Evaluate("new Date(2010, 12).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 1)), TestUtils.Evaluate("new Date(2010, 3, 1).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 11, 23, 0, 0)), TestUtils.Evaluate("new Date(2010, 2, 12, -1).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 1, 3, 0)), TestUtils.Evaluate("new Date(2010, 2, 12, 0, 63).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 0, 1, 3)), TestUtils.Evaluate("new Date(2010, 2, 12, 0, 0, 63).valueOf()"));

            if (TestUtils.Engine != JSEngine.JScript)
            {
                // Date() returns the current date as a string - this test assumes the running time is less than 1s.
                var str = (string)TestUtils.Evaluate("Date()");
                var formatString = "ddd MMM dd yyyy HH:mm:ss";
                Assert.IsTrue(str.StartsWith(DateTime.Now.ToString(formatString)) || str.StartsWith(DateTime.Now.AddSeconds(1).ToString(formatString)),
                    string.Format("Expected: {0} Was: {1}", DateTime.Now.ToString(formatString), str));

                // Any arguments provided are ignored.
                str = (string)TestUtils.Evaluate("Date(2009)");
                Assert.IsTrue(str.StartsWith(DateTime.Now.ToString("ddd MMM dd yyyy HH:mm:ss")) ||
                    str.StartsWith(DateTime.Now.AddSeconds(1).ToString("ddd MMM dd yyyy HH:mm:ss")));
            }

            // toString and valueOf.
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("function Date() { [native code] }", TestUtils.Evaluate("Date.toString()"));
            Assert.AreEqual(true, TestUtils.Evaluate("Date.valueOf() === Date"));

            // Undefined dates.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(undefined, 1, 1).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, 2, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, 2, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, 2, 1, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, 2, 1, 1, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(NaN).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, NaN).valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date(2010, 1, 2, NaN).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 2, 1, 1, 1, 1)), TestUtils.Evaluate("new Date(2010, 1, 2, 1, 1, 1, 1, undefined).valueOf()"));

            // length
            Assert.AreEqual(7, TestUtils.Evaluate("Date.length"));
        }

        [TestMethod]
        public void constructor()
        {
            Assert.AreEqual(true, TestUtils.Evaluate("new Date().constructor === Date"));
        }

        [TestMethod]
        public void prototype()
        {
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.prototype.valueOf()"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(true, TestUtils.Evaluate("Object.getPrototypeOf(new Date()) === Date.prototype"));
        }

        [TestMethod]
        public void now()
        {
            // JScript doesn't support Date.now
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            // This test assumes the running time is less than 30ms.
            Assert.AreEqual((double)ToJSDate(DateTime.Now), (double)TestUtils.Evaluate("Date.now()"), 30);
        }

        [TestMethod]
        public void parse()
        {
            if (TestUtils.Engine != JSEngine.JScript)
            {
                // ECMAScript format - date-only forms.
                Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05')"));

                // ECMAScript format - date and time forms.
                Assert.AreEqual(45240000, TestUtils.Evaluate("Date.parse('1970-01-01T12:34')"));
                Assert.AreEqual(ToJSDate(new DateTime(1970, 1, 1, 12, 34, 56, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('1970-01-01T12:34:56')"));
                Assert.AreEqual(ToJSDate(new DateTime(1970, 1, 1, 12, 34, 56, 123, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('1970-01-01T12:34:56.123')"));
                Assert.AreEqual(45240000, TestUtils.Evaluate("Date.parse('1970-01-01T12:34Z')"));
                Assert.AreEqual(45296000, TestUtils.Evaluate("Date.parse('1970-01-01T12:34:56Z')"));
                Assert.AreEqual(45296123, TestUtils.Evaluate("Date.parse('1970-01-01T12:34:56.123Z')"));
                Assert.AreEqual(77640000, TestUtils.Evaluate("Date.parse('1970-01-01T12:34-09:00')"));
                Assert.AreEqual(77696000, TestUtils.Evaluate("Date.parse('1970-01-01T12:34:56-09:00')"));
                Assert.AreEqual(77696123, TestUtils.Evaluate("Date.parse('1970-01-01T12:34:56.123-09:00')"));
                Assert.AreEqual(ToJSDate(new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('1970-01-01T24:00')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1, 12, 34, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010T12:34')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 1, 12, 34, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02T12:34')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34:56Z')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 12, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34:56.012Z')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 100, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34:56.1Z')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 120, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34:56.12Z')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 123, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34:56.1234567890123456789Z')"));
                Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 3, 34, 56, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('2010-02-05T12:34:56+09:00')"));
            }

            // Unstructured forms.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('5 Jan 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('Jan 5 2010')"));
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('1 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('Tue Jan 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('Wed Jan 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), TestUtils.Evaluate("Date.parse('Sat, 24 Apr 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), TestUtils.Evaluate("Date.parse(' Sat , 24  Apr  2010 ')"));
            Assert.AreEqual(ToJSDate(new DateTime(1989, 12, 31)), TestUtils.Evaluate("Date.parse('31 Dec 89')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('2010 Jan 5')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('1/5/2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('1-5-2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), TestUtils.Evaluate("Date.parse('5 January 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 15, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 15:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 3, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 3:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 2, 3, 1)), TestUtils.Evaluate("Date.parse('24 Apr 2010 2:3:1')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 15, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 15:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 12:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 2, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 2:30:01 am')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 14, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 2:30:01 PM')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 0, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 12:30:01 AM')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 0, 30, 01)), TestUtils.Evaluate("Date.parse('24 Apr 2010 0:30:01 AM')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 30, 0)), TestUtils.Evaluate("Date.parse('24 Apr 2010 12:30  pm')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 UT')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 GMT')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 EST')")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 3, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 EDT')")); // -4
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 CST')")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 CDT')")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 MST')")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 MDT')")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 7, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 PST')")); // -8
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 PDT')")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 Z')"));   // GMT
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 A')"));   // -1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 11, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 M')"));   // -12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 N')"));   // +1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 Y')"));   // +12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 +1100')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 29, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 +0030')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 00, 29, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 -30')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 29, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 +90')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 GMT+1100')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), TestUtils.Evaluate("Date.parse('24 Apr 2010 23:59:59 GMT+1100 (Zone Name)')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 3)), TestUtils.Evaluate("Date.parse('31 Feb 2010')"));

            // Invalid ECMAScript dates.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('2010-0-2')"));                          // month out of range
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('2010-2-29')"));                         // day out of range
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T24:01')"));                  // 24:00 is the last valid time.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T24:00:01')"));               // 24:00 is the last valid time.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T24:00:00.001')"));           // 24:00 is the last valid time.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T12:60')"));                  // 00-59 minutes.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T12:34:60')"));               // 00-59 seconds.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T12')"));                     // no minutes.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T5:34')"));                   // hours must be 2 digits.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T05:3')"));                   // minutes must be 2 digits.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T05:34:2')"));                // seconds must be 2 digits.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('1970-01-01T05:34:22.')"));              // milliseconds must have at least one digit.

            // Time-only forms should not be supported (see addendum).
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('T12:34Z')"));

            // Invalid unstructured dates.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('5 Jan')"));                         // no year
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('Jan 2010')"));                      // no day
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('5 2010')"));                        // no day
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('24 Apr 2010 15 : 30 : 01')"));  // spaces between time components
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('24 Apr 2010 15')"));                // extraneous number
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('24 Apr 2010 15:30:01.123')"));      // milliseconds not supported
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('24 Apr 2010 hello')"));             // extraneous text
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('24 Apr 2010 13:30:01 AM')"));       // 12 hour clock goes from 0-12.
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("Date.parse('24 Apr 2010 13:30:01 PM')"));       // 12 hour clock goes from 0-12.
        }

        [TestMethod]
        public void UTC()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 0)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 0, 5)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 0, 5, 12)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 0, 5, 12, 32)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 0, 5, 12, 32, 45)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45, 123, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 0, 5, 12, 32, 45, 123)"));

            // Test overflow.
            Assert.AreEqual(ToJSDate(new DateTime(2009, 12, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, -1)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 12)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 3, 1)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 11, 23, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 2, 12, -1)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 1, 3, 0, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 2, 12, 0, 63)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 0, 1, 3, DateTimeKind.Utc)), TestUtils.Evaluate("Date.UTC(2010, 2, 12, 0, 0, 63)"));
        }

        [TestMethod]
        public void getDate()
        {
            Assert.AreEqual(24, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getDate()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getDate.length"));
        }

        [TestMethod]
        public void getDay()
        {
            Assert.AreEqual(6, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getDay()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getDay.length"));
        }

        [TestMethod]
        public void getFullYear()
        {
            Assert.AreEqual(2010, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getFullYear()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getFullYear.length"));
        }

        [TestMethod]
        public void getHours()
        {
            Assert.AreEqual(23, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getHours()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getHours.length"));
        }

        [TestMethod]
        public void getMilliseconds()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getMilliseconds()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getMilliseconds.length"));
        }

        [TestMethod]
        public void getMinutes()
        {
            Assert.AreEqual(59, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getMinutes()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getMinutes.length"));
        }

        [TestMethod]
        public void getMonth()
        {
            Assert.AreEqual(3, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getMonth()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getMonth.length"));
        }

        [TestMethod]
        public void getSeconds()
        {
            Assert.AreEqual(57, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getSeconds()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getSeconds.length"));
        }

        [TestMethod]
        public void getTime()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getTime()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getTime.length"));
        }

        [TestMethod]
        public void getTimezoneOffset()
        {
            var offsetInMinutes = (int)-TimeZoneInfo.Local.GetUtcOffset(new DateTime(2010, 4, 24, 23, 59, 57)).TotalMinutes;
            Assert.AreEqual(offsetInMinutes, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getTimezoneOffset()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getTimezoneOffset.length"));
        }

        [TestMethod]
        public void getUTCDate()
        {
            Assert.AreEqual(24, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCDate()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCDate.length"));
        }

        [TestMethod]
        public void getUTCDay()
        {
            Assert.AreEqual(6, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCDay()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCDay.length"));
        }

        [TestMethod]
        public void getUTCFullYear()
        {
            Assert.AreEqual(2010, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCFullYear()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCFullYear.length"));
        }

        [TestMethod]
        public void getUTCHours()
        {
            Assert.AreEqual(23, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCHours()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCHours.length"));
        }

        [TestMethod]
        public void getUTCMilliseconds()
        {
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCMilliseconds()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCMilliseconds.length"));
        }

        [TestMethod]
        public void getUTCMinutes()
        {
            Assert.AreEqual(59, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCMinutes()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCMinutes.length"));
        }

        [TestMethod]
        public void getUTCMonth()
        {
            Assert.AreEqual(3, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCMonth()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCMonth.length"));
        }

        [TestMethod]
        public void getUTCSeconds()
        {
            Assert.AreEqual(57, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCSeconds()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getUTCSeconds.length"));
        }

        [TestMethod]
        public void getYear()
        {
            // Note: JScript purposefully disobeys the spec as part of Y2K readiness.
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? 2010 : 110, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getYear()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').getYear.length"));
        }



        [TestMethod]
        public void setDate()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 11, 23, 59, 57)), TestUtils.Evaluate("x.setDate(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57)), TestUtils.Evaluate("x.setDate(35)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setDate(NaN)"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.setDate.length"));
        }

        [TestMethod]
        public void setFullYear()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.setFullYear(2001)"));
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(90, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.setFullYear(90)"));
            Assert.AreEqual(ToJSDate(new DateTime(2002, 12, 5, 23, 59, 57)), TestUtils.Evaluate("x.setFullYear(2002, 11, 5)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setFullYear(NaN)"));
            Assert.AreEqual(3, TestUtils.Evaluate("x.setFullYear.length"));
        }

        [TestMethod]
        public void setHours()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 57)), TestUtils.Evaluate("x.setHours(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57)), TestUtils.Evaluate("x.setHours(34)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 4, 3, 2)), TestUtils.Evaluate("x.setHours(5, 4, 3, 2)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setHours(NaN)"));
            Assert.AreEqual(4, TestUtils.Evaluate("x.setHours.length"));
        }

        [TestMethod]
        public void setMilliseconds()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57, 11)), TestUtils.Evaluate("x.setMilliseconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5)), TestUtils.Evaluate("x.setMilliseconds(1005)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setMilliseconds(NaN)"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.setMilliseconds.length"));
        }

        [TestMethod]
        public void setMinutes()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 11, 57)), TestUtils.Evaluate("x.setMinutes(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 57)), TestUtils.Evaluate("x.setMinutes(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 57)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 4, 3)), TestUtils.Evaluate("x.setMinutes(5, 4, 3)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setMinutes(NaN)"));
            Assert.AreEqual(3, TestUtils.Evaluate("x.setMinutes.length"));
        }

        [TestMethod]
        public void setMonth()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 12, 24, 23, 59, 57)), TestUtils.Evaluate("x.setMonth(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57)), TestUtils.Evaluate("x.setMonth(13)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 6, 4, 23, 59, 57)), TestUtils.Evaluate("x.setMonth(5, 4)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setMonth(NaN)"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.setMonth.length"));
        }

        [TestMethod]
        public void setSeconds()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 11)), TestUtils.Evaluate("x.setSeconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5)), TestUtils.Evaluate("x.setSeconds(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 7, 4)), TestUtils.Evaluate("x.setSeconds(7, 4)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setSeconds(NaN)"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.setSeconds.length"));
        }

        [TestMethod]
        public void setTime()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual((int)ToJSDate(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("x.setTime(0)"));
            Assert.AreEqual((int)ToJSDate(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setTime(NaN)"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.setTime.length"));
        }

        [TestMethod]
        public void setUTCDate()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 11, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCDate(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCDate(35)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setUTCDate(NaN)"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.setUTCDate.length"));
        }

        [TestMethod]
        public void setUTCFullYear()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCFullYear(2001)"));
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(90, 4, 24, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCFullYear(90)"));
            Assert.AreEqual(ToJSDate(new DateTime(2002, 2, 2, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCFullYear(2002, 1, 2)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setUTCFullYear(NaN)"));
            Assert.AreEqual(3, TestUtils.Evaluate("x.setUTCFullYear.length"));
        }

        [TestMethod]
        public void setUTCHours()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCHours(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCHours(34)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 4, 3, 2, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCHours(5, 4, 3, 2)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setUTCHours(NaN)"));
            Assert.AreEqual(4, TestUtils.Evaluate("x.setUTCHours.length"));
        }

        [TestMethod]
        public void setUTCMilliseconds()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57, 11, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCMilliseconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCMilliseconds(1005)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setUTCMilliseconds(NaN)"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.setUTCMilliseconds.length"));
        }

        [TestMethod]
        public void setUTCMinutes()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 11, 57, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 57, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 5, 4, 3, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(5, 4, 3)"));

            // Negative minutes are before the hour.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 55, 57, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(-5)"));

            // Fractional minutes are ignored.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 13, 57, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(13.9)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 55, 57, DateTimeKind.Utc)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(-5.9)"));

            Assert.AreEqual(double.NaN, TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(NaN)"));
            Assert.AreEqual(3, TestUtils.Evaluate("new Date().setUTCMinutes.length"));
        }

        [TestMethod]
        public void setUTCMonth()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 12, 24, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCMonth(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCMonth(13)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 6, 4, 23, 59, 57, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCMonth(5, 4)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setUTCMonth(NaN)"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.setUTCMonth.length"));
        }

        [TestMethod]
        public void setUTCSeconds()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 11, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCSeconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCSeconds(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5, DateTimeKind.Utc)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 4, 3, DateTimeKind.Utc)), TestUtils.Evaluate("x.setUTCSeconds(4, 3)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setUTCSeconds(NaN)"));
            Assert.AreEqual(2, TestUtils.Evaluate("x.setUTCSeconds.length"));
        }

        [TestMethod]
        public void setYear()
        {
            TestUtils.Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(1999, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.setYear(99)"));
            Assert.AreEqual(ToJSDate(new DateTime(105, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.setYear(105)"));
            Assert.AreEqual(ToJSDate(new DateTime(105, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 4, 24, 23, 59, 57)), TestUtils.Evaluate("x.setYear(2011)"));
            Assert.AreEqual(double.NaN, TestUtils.Evaluate("x.setYear(NaN)"));
            Assert.AreEqual(1, TestUtils.Evaluate("x.setYear.length"));
        }

        [TestMethod]
        public void toDateString()
        {
            Assert.AreEqual("Sat Apr 24 2010", TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').toDateString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toDateString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toDateString.length"));
        }

        [TestMethod]
        public void toGMTString()
        {
            Assert.AreEqual("Sat, 24 Apr 2010 23:59:57 GMT", TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toGMTString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toGMTString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toGMTString.length"));
        }

        [TestMethod]
        public void toISOString()
        {
            // JScript does not support Date.toISOString
            if (TestUtils.Engine == JSEngine.JScript)
                return;

            Assert.AreEqual("1970-01-01T00:00:00.012Z", TestUtils.Evaluate("new Date(12).toISOString()"));
            Assert.AreEqual("2010-04-24T23:59:57.000Z", TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toISOString()"));
            Assert.AreEqual("RangeError", TestUtils.EvaluateExceptionType("new Date(NaN).toISOString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toISOString.length"));
        }

        [TestMethod]
        public void toJSON()
        {
            // Note: the spec says the milliseconds field is optional.  Firefox and Chrome include it but IE doesn't.
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "1970-01-01T00:00:00Z" : "1970-01-01T00:00:00.012Z", TestUtils.Evaluate("new Date(12).toJSON()"));
            Assert.AreEqual(TestUtils.Engine == JSEngine.JScript ? "2010-04-24T23:59:57Z" : "2010-04-24T23:59:57.000Z", TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toJSON()"));
            Assert.AreEqual(Jurassic.Null.Value, TestUtils.Evaluate("new Date(NaN).toJSON()"));
            Assert.AreEqual(1, TestUtils.Evaluate("new Date().toJSON.length"));

            // toJSON is generic.
            Assert.AreEqual("abc", TestUtils.Evaluate("x = {toISOString: function() { return 'abc'; }, f: new Date().toJSON }; x.f()"));
        }

        [TestMethod]
        public void toLocaleDateString()
        {
            Assert.AreEqual(new DateTime(2010, 4, 24).ToLongDateString(), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').toLocaleDateString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toLocaleDateString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toLocaleDateString.length"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            Assert.AreEqual(new DateTime(2010, 4, 24, 23, 59, 57).ToString("F"), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').toLocaleString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toLocaleString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toLocaleString.length"));
        }

        [TestMethod]
        public void toLocaleTimeString()
        {
            Assert.AreEqual(new DateTime(2010, 4, 24, 23, 59, 57).ToLongTimeString(), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').toLocaleTimeString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toLocaleTimeString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toLocaleTimeString.length"));
        }

        [TestMethod]
        public void toString()
        {
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("Sat Apr 24 2010 23:59:57 " + GetTimezoneString(DateTime.Parse("24 Apr 2010 23:59:57")), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').toString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toString.length"));
        }

        [TestMethod]
        public void toTimeString()
        {
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("23:59:57 " + GetTimezoneString(DateTime.Parse("24 Apr 2010 23:59:57")), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').toTimeString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toTimeString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toTimeString.length"));
        }

        [TestMethod]
        public void toUTCString()
        {
            if (TestUtils.Engine != JSEngine.JScript)
                Assert.AreEqual("Sat, 24 Apr 2010 23:59:57 GMT", TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toUTCString()"));
            Assert.AreEqual("Invalid Date", TestUtils.Evaluate("new Date(NaN).toUTCString()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().toUTCString.length"));
        }

        [TestMethod]
        public void valueOf()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57)), TestUtils.Evaluate("new Date('24 Apr 2010 23:59:57').valueOf()"));
            Assert.AreEqual(0, TestUtils.Evaluate("new Date().valueOf.length"));
        }







        private static object ToJSDate(DateTime dateTime)
        {
            var result = dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            if ((double)(int)result == result)
                return (int)result;
            return result;
        }

        private static string GetTimezoneString(DateTime date)
        {
            var timeZoneInfo = TimeZoneInfo.Local;
            var offset = timeZoneInfo.GetUtcOffset(date);
            return string.Format("GMT{3}{0:d2}{1:d2} ({2})", offset.Hours, offset.Minutes,
                timeZoneInfo.IsDaylightSavingTime(date) ? timeZoneInfo.DaylightName : timeZoneInfo.StandardName, 
                offset.Hours > 0 ? "+" : "");
        }
    }
}
