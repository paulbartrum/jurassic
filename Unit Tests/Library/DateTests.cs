using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the global Date object.
    /// </summary>
    [TestClass]
    public class DateTests : TestBase
    {
        [TestMethod]
        public void Constructor()
        {
            // Warm up the engine.
            Evaluate("new Date().valueOf()");

            // new Date() returns the current date - this test assumes the running time is less than 30ms.
            Assert.AreEqual((double)ToJSDate(DateTime.Now), (double)Evaluate("new Date().valueOf()"), 30);

            // new Date(milliseconds)
            Assert.AreEqual(0, Evaluate("new Date(0).valueOf()"));
            Assert.AreEqual(100, Evaluate("new Date(100).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(NaN).valueOf()"));
            Assert.AreEqual(1, Evaluate("new Date(true).valueOf()"));
            Assert.AreEqual(6, Evaluate("new Date(6.6).valueOf()"));
            Assert.AreEqual(-6, Evaluate("new Date(-6.6).valueOf()"));

            // new Date(dateStr)
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("new Date('5 Jan 2010').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), Evaluate("new Date('Sat, 24 Apr 2010').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), Evaluate("new Date(' Sat , 24  Apr  2010 ').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(1989, 12, 31)), Evaluate("new Date('31 Dec 89').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 15, 30, 01)), Evaluate("new Date('24 Apr 2010 15:30:01').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 3, 30, 01)), Evaluate("new Date('24 Apr 2010 3:30:01').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 2, 3, 1)), Evaluate("new Date('24 Apr 2010 2:3:1').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 UT').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 GMT').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 EST').valueOf()")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 3, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 EDT').valueOf()")); // -4
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 CST').valueOf()")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 CDT').valueOf()")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 MST').valueOf()")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 MDT').valueOf()")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 7, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 PST').valueOf()")); // -8
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 PDT').valueOf()")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 Z').valueOf()"));   // GMT
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 A').valueOf()"));   // -1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 11, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 M').valueOf()"));   // -12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 N').valueOf()"));   // +1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 Y').valueOf()"));   // +12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 +1100').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 29, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 +0030').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 29, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 +30').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 29, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 +90').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 GMT+1100').valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:59 GMT+1100 (Zone Name)').valueOf()"));

            // This date doesn't actually exist.
            Assert.AreEqual(ToJSDate(new DateTime(2011, 3, 1, 0, 0, 0)), Evaluate("new Date('29 Feb 2011').valueOf()"));

            // Dates before 1970 should work.
            Assert.AreEqual(-31585736580000d - TimeZoneInfo.Local.GetUtcOffset(new DateTime(969, 01, 01, 8, 17, 0, DateTimeKind.Local)).TotalMilliseconds,
                Evaluate("new Date(969, 01, 01, 8, 17, 0).valueOf()"));
            Assert.AreEqual(true, Evaluate("new Date(new Date(969, 01, 01, 8, 17, 0)).getTime() == new Date(969, 01, 01, 8, 17, 0).getTime()"));

            // new Date(year, month, [day], [hour], [minute], [second], [millisecond])
            // Note: month is 0-11 is javascript but 1-12 in .NET.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1)), Evaluate("new Date(2010, 0).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("new Date(2010, 0, 5).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 0, 0)), Evaluate("new Date(2010, 0, 5, 12).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 0)), Evaluate("new Date(2010, 0, 5, 12, 32).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45)), Evaluate("new Date(2010, 0, 5, 12, 32, 45).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45, 123)), Evaluate("new Date(2010, 0, 5, 12, 32, 45, 123).valueOf()"));
            
            // Test overflow.
            Assert.AreEqual(ToJSDate(new DateTime(2009, 12, 1)), Evaluate("new Date(2010, -1).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 1, 1)), Evaluate("new Date(2010, 12).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 1)), Evaluate("new Date(2010, 3, 1).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 11, 23, 0, 0)), Evaluate("new Date(2010, 2, 12, -1).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 1, 3, 0)), Evaluate("new Date(2010, 2, 12, 0, 63).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 0, 1, 3)), Evaluate("new Date(2010, 2, 12, 0, 0, 63).valueOf()"));

            // Date() returns the current date as a string - this test assumes the running time is less than 1s.
            var str = (string)Evaluate("Date()");
                var formatString = "ddd MMM dd yyyy HH:mm:ss";
                Assert.IsTrue(str.StartsWith(DateTime.Now.ToString(formatString, CultureInfo.InvariantCulture)) || str.StartsWith(DateTime.Now.AddSeconds(1).ToString(formatString)),
                    string.Format("Expected: {0} Was: {1}", DateTime.Now.ToString(formatString), str));

            // Any arguments provided are ignored.
            str = (string)Evaluate("Date(2009)");
                Assert.IsTrue(str.StartsWith(DateTime.Now.ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture)) ||
                    str.StartsWith(DateTime.Now.AddSeconds(1).ToString("ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture)));

            // toString and valueOf.
            Assert.AreEqual("function Date() { [native code] }", Evaluate("Date.toString()"));
            Assert.AreEqual(true, Evaluate("Date.valueOf() === Date"));

            // Undefined dates.
            Assert.AreEqual(double.NaN, Evaluate("new Date(undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(undefined, 1, 1).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, 2, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, 2, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, 2, 1, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, 2, 1, 1, 1, undefined).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(NaN).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, NaN).valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("new Date(2010, 1, 2, NaN).valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 2, 1, 1, 1, 1)), Evaluate("new Date(2010, 1, 2, 1, 1, 1, 1, undefined).valueOf()"));

            // length
            Assert.AreEqual(7, Evaluate("Date.length"));
        }

        [TestMethod]
        public void constructor()
        {
            Assert.AreEqual(true, Evaluate("new Date().constructor === Date"));
        }

        [TestMethod]
        public void prototype()
        {
            Assert.AreEqual("TypeError", EvaluateExceptionType("Date.prototype.valueOf()"));
            Assert.AreEqual(true, Evaluate("Object.getPrototypeOf(new Date()) === Date.prototype"));
        }

        [TestMethod]
        public void now()
        {
            // This test assumes the running time is less than 30ms.
            Assert.AreEqual((double)ToJSDate(DateTime.Now), (double)Evaluate("Date.now()"), 30);
        }

        [TestMethod]
        public void parse()
        {
            // ECMAScript format - date-only forms.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.parse('2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05')"));

            // ECMAScript format - date and time forms.
            Assert.AreEqual(45240000, Evaluate("Date.parse('1970-01-01T12:34')"));
            Assert.AreEqual(ToJSDate(new DateTime(1970, 1, 1, 12, 34, 56, DateTimeKind.Utc)), Evaluate("Date.parse('1970-01-01T12:34:56')"));
            Assert.AreEqual(ToJSDate(new DateTime(1970, 1, 1, 12, 34, 56, 123, DateTimeKind.Utc)), Evaluate("Date.parse('1970-01-01T12:34:56.123')"));
            Assert.AreEqual(45240000, Evaluate("Date.parse('1970-01-01T12:34Z')"));
            Assert.AreEqual(45296000, Evaluate("Date.parse('1970-01-01T12:34:56Z')"));
            Assert.AreEqual(45296123, Evaluate("Date.parse('1970-01-01T12:34:56.123Z')"));
            Assert.AreEqual(77640000, Evaluate("Date.parse('1970-01-01T12:34-09:00')"));
            Assert.AreEqual(77696000, Evaluate("Date.parse('1970-01-01T12:34:56-09:00')"));
            Assert.AreEqual(77696123, Evaluate("Date.parse('1970-01-01T12:34:56.123-09:00')"));
            Assert.AreEqual(ToJSDate(new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.parse('1970-01-01T24:00')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1, 12, 34, 0, DateTimeKind.Utc)), Evaluate("Date.parse('2010T12:34')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 1, 12, 34, 0, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02T12:34')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 0, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34:56Z')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 12, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34:56.012Z')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 100, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34:56.1Z')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 120, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34:56.12Z')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 12, 34, 56, 123, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34:56.1234567890123456789Z')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 2, 5, 3, 34, 56, DateTimeKind.Utc)), Evaluate("Date.parse('2010-02-05T12:34:56+09:00')"));

            // Unstructured forms.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('5 Jan 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('Jan 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('1 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('Tue Jan 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('Wed Jan 5 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), Evaluate("Date.parse('Sat, 24 Apr 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24)), Evaluate("Date.parse(' Sat , 24  Apr  2010 ')"));
            Assert.AreEqual(ToJSDate(new DateTime(1989, 12, 31)), Evaluate("Date.parse('31 Dec 89')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('2010 Jan 5')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('1/5/2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('1-5-2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5)), Evaluate("Date.parse('5 January 2010')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 15, 30, 01)), Evaluate("Date.parse('24 Apr 2010 15:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 3, 30, 01)), Evaluate("Date.parse('24 Apr 2010 3:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 2, 3, 1)), Evaluate("Date.parse('24 Apr 2010 2:3:1')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 15, 30, 01)), Evaluate("Date.parse('24 Apr 2010 15:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 30, 01)), Evaluate("Date.parse('24 Apr 2010 12:30:01')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 2, 30, 01)), Evaluate("Date.parse('24 Apr 2010 2:30:01 am')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 14, 30, 01)), Evaluate("Date.parse('24 Apr 2010 2:30:01 PM')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 0, 30, 01)), Evaluate("Date.parse('24 Apr 2010 12:30:01 AM')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 0, 30, 01)), Evaluate("Date.parse('24 Apr 2010 0:30:01 AM')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 30, 0)), Evaluate("Date.parse('24 Apr 2010 12:30  pm')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 UT')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 GMT')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 EST')")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 3, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 EDT')")); // -4
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 CST')")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 4, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 CDT')")); // -5
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 MST')")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 MDT')")); // -6
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 7, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 PST')")); // -8
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 6, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 PDT')")); // -7
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 Z')"));   // GMT
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 A')"));   // -1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 11, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 M')"));   // -12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 N')"));   // +1
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 Y')"));   // +12
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 +1100')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 29, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 +0030')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 00, 29, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 -30')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 29, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 +90')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 GMT+1100')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 12, 59, 59, DateTimeKind.Utc)), Evaluate("Date.parse('24 Apr 2010 23:59:59 GMT+1100 (Zone Name)')"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 3)), Evaluate("Date.parse('31 Feb 2010')"));

            // Invalid ECMAScript dates.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('2010-0-2')"));                          // month out of range
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('2010-2-29')"));                         // day out of range
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T24:01')"));                  // 24:00 is the last valid time.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T24:00:01')"));               // 24:00 is the last valid time.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T24:00:00.001')"));           // 24:00 is the last valid time.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T12:60')"));                  // 00-59 minutes.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T12:34:60')"));               // 00-59 seconds.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T12')"));                     // no minutes.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T5:34')"));                   // hours must be 2 digits.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T05:3')"));                   // minutes must be 2 digits.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T05:34:2')"));                // seconds must be 2 digits.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('1970-01-01T05:34:22.')"));              // milliseconds must have at least one digit.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('2011-02-29T12:00:00.000Z')"));          // 29 Feb did not exist in 2011.

            // Time-only forms should not be supported (see addendum).
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('T12:34Z')"));

            // Invalid unstructured dates.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('5 Jan')"));                         // no year
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('Jan 2010')"));                      // no day
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('5 2010')"));                        // no day
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('24 Apr 2010 15 : 30 : 01')"));  // spaces between time components
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('24 Apr 2010 15')"));                // extraneous number
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('24 Apr 2010 15:30:01.123')"));      // milliseconds not supported
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('24 Apr 2010 hello')"));             // extraneous text
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('24 Apr 2010 13:30:01 AM')"));       // 12 hour clock goes from 0-12.
            Assert.AreEqual(double.NaN, Evaluate("Date.parse('24 Apr 2010 13:30:01 PM')"));       // 12 hour clock goes from 0-12.
        }

        [TestMethod]
        public void UTC()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 0)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 0, 5)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 0, 5, 12)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 0, 5, 12, 32)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 0, 5, 12, 32, 45)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 1, 5, 12, 32, 45, 123, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 0, 5, 12, 32, 45, 123)"));

            // Test overflow.
            Assert.AreEqual(ToJSDate(new DateTime(2009, 12, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, -1)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 1, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 12)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 3, 1)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 11, 23, 0, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 2, 12, -1)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 1, 3, 0, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 2, 12, 0, 63)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 3, 12, 0, 1, 3, DateTimeKind.Utc)), Evaluate("Date.UTC(2010, 2, 12, 0, 0, 63)"));
        }

        [TestMethod]
        public void getDate()
        {
            Assert.AreEqual(24, Evaluate("new Date('24 Apr 2010 23:59:57').getDate()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getDate.length"));
        }

        [TestMethod]
        public void getDay()
        {
            Assert.AreEqual(6, Evaluate("new Date('24 Apr 2010 23:59:57').getDay()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getDay.length"));
        }

        [TestMethod]
        public void getFullYear()
        {
            Assert.AreEqual(2010, Evaluate("new Date('24 Apr 2010 23:59:57').getFullYear()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getFullYear.length"));
        }

        [TestMethod]
        public void getHours()
        {
            Assert.AreEqual(23, Evaluate("new Date('24 Apr 2010 23:59:57').getHours()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getHours.length"));
        }

        [TestMethod]
        public void getMilliseconds()
        {
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getMilliseconds()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getMilliseconds.length"));
        }

        [TestMethod]
        public void getMinutes()
        {
            Assert.AreEqual(59, Evaluate("new Date('24 Apr 2010 23:59:57').getMinutes()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getMinutes.length"));
        }

        [TestMethod]
        public void getMonth()
        {
            Assert.AreEqual(3, Evaluate("new Date('24 Apr 2010 23:59:57').getMonth()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getMonth.length"));
        }

        [TestMethod]
        public void getSeconds()
        {
            Assert.AreEqual(57, Evaluate("new Date('24 Apr 2010 23:59:57').getSeconds()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getSeconds.length"));
        }

        [TestMethod]
        public void getTime()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57)), Evaluate("new Date('24 Apr 2010 23:59:57').getTime()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getTime.length"));
        }

        [TestMethod]
        public void getTimezoneOffset()
        {
            var offsetInMinutes = (int)-TimeZoneInfo.Local.GetUtcOffset(new DateTime(2010, 4, 24, 23, 59, 57)).TotalMinutes;
            Assert.AreEqual(offsetInMinutes, Evaluate("new Date('24 Apr 2010 23:59:57').getTimezoneOffset()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getTimezoneOffset.length"));
        }

        [TestMethod]
        public void getUTCDate()
        {
            Assert.AreEqual(24, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCDate()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCDate.length"));
        }

        [TestMethod]
        public void getUTCDay()
        {
            Assert.AreEqual(6, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCDay()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCDay.length"));
        }

        [TestMethod]
        public void getUTCFullYear()
        {
            Assert.AreEqual(2010, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCFullYear()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCFullYear.length"));
        }

        [TestMethod]
        public void getUTCHours()
        {
            Assert.AreEqual(23, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCHours()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCHours.length"));
        }

        [TestMethod]
        public void getUTCMilliseconds()
        {
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCMilliseconds()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCMilliseconds.length"));
        }

        [TestMethod]
        public void getUTCMinutes()
        {
            Assert.AreEqual(59, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCMinutes()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCMinutes.length"));
        }

        [TestMethod]
        public void getUTCMonth()
        {
            Assert.AreEqual(3, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCMonth()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCMonth.length"));
        }

        [TestMethod]
        public void getUTCSeconds()
        {
            Assert.AreEqual(57, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').getUTCSeconds()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getUTCSeconds.length"));
        }

        [TestMethod]
        public void getYear()
        {
            Assert.AreEqual(110, Evaluate("new Date('24 Apr 2010 23:59:57').getYear()"));
            Assert.AreEqual(0, Evaluate("new Date('24 Apr 2010 23:59:57').getYear.length"));
        }



        [TestMethod]
        public void setDate()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 11, 23, 59, 57)), Evaluate("x.setDate(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57)), Evaluate("x.setDate(35)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57)), Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("x.setDate(NaN)"));
            Assert.AreEqual(1, Evaluate("x.setDate.length"));
        }

        [TestMethod]
        public void setFullYear()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57)), Evaluate("x.setFullYear(2001)"));
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(90, 4, 24, 23, 59, 57)), Evaluate("x.setFullYear(90)"));
            Assert.AreEqual(ToJSDate(new DateTime(2002, 12, 5, 23, 59, 57)), Evaluate("x.setFullYear(2002, 11, 5)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setFullYear(NaN)"));
            Assert.AreEqual(3, Evaluate("x.setFullYear.length"));
        }

        [TestMethod]
        public void setHours()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 57)), Evaluate("x.setHours(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57)), Evaluate("x.setHours(34)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 4, 3, 2)), Evaluate("x.setHours(5, 4, 3, 2)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setHours(NaN)"));
            Assert.AreEqual(4, Evaluate("x.setHours.length"));
        }

        [TestMethod]
        public void setMilliseconds()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57, 11)), Evaluate("x.setMilliseconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5)), Evaluate("x.setMilliseconds(1005)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5)), Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("x.setMilliseconds(NaN)"));
            Assert.AreEqual(1, Evaluate("x.setMilliseconds.length"));
        }

        [TestMethod]
        public void setMinutes()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 11, 57)), Evaluate("x.setMinutes(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 57)), Evaluate("x.setMinutes(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 57)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 4, 3)), Evaluate("x.setMinutes(5, 4, 3)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setMinutes(NaN)"));
            Assert.AreEqual(3, Evaluate("x.setMinutes.length"));
        }

        [TestMethod]
        public void setMonth()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 12, 24, 23, 59, 57)), Evaluate("x.setMonth(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57)), Evaluate("x.setMonth(13)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 6, 4, 23, 59, 57)), Evaluate("x.setMonth(5, 4)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setMonth(NaN)"));
            Assert.AreEqual(2, Evaluate("x.setMonth.length"));
        }

        [TestMethod]
        public void setSeconds()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 11)), Evaluate("x.setSeconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5)), Evaluate("x.setSeconds(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 7, 4)), Evaluate("x.setSeconds(7, 4)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setSeconds(NaN)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 7, 0)), Evaluate("new Date('24 Apr 2010 23:59:57').setSeconds(7, null)"));
            Assert.AreEqual(double.NaN, Evaluate("new Date('24 Apr 2010 23:59:57').setSeconds(7, undefined)"));
            Assert.AreEqual(2, Evaluate("x.setSeconds.length"));
        }

        [TestMethod]
        public void setTime()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual((int)ToJSDate(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("x.setTime(0)"));
            Assert.AreEqual((int)ToJSDate(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("x.setTime(NaN)"));
            Assert.AreEqual(1, Evaluate("x.setTime.length"));
        }

        [TestMethod]
        public void setUTCDate()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 11, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCDate(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCDate(35)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 5, 5, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("x.setUTCDate(NaN)"));
            Assert.AreEqual(1, Evaluate("x.setUTCDate.length"));
        }

        [TestMethod]
        public void setUTCFullYear()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCFullYear(2001)"));
            Assert.AreEqual(ToJSDate(new DateTime(2001, 4, 24, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(90, 4, 24, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCFullYear(90)"));
            Assert.AreEqual(ToJSDate(new DateTime(2002, 2, 2, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCFullYear(2002, 1, 2)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setUTCFullYear(NaN)"));
            Assert.AreEqual(3, Evaluate("x.setUTCFullYear.length"));
        }

        [TestMethod]
        public void setUTCHours()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 11, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCHours(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCHours(34)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 10, 59, 57, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 5, 4, 3, 2, DateTimeKind.Utc)), Evaluate("x.setUTCHours(5, 4, 3, 2)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setUTCHours(NaN)"));
            Assert.AreEqual(4, Evaluate("x.setUTCHours.length"));
        }

        [TestMethod]
        public void setUTCMilliseconds()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57, 11, DateTimeKind.Utc)), Evaluate("x.setUTCMilliseconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5, DateTimeKind.Utc)), Evaluate("x.setUTCMilliseconds(1005)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 58, 5, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(double.NaN, Evaluate("x.setUTCMilliseconds(NaN)"));
            Assert.AreEqual(1, Evaluate("x.setUTCMilliseconds.length"));
        }

        [TestMethod]
        public void setUTCMinutes()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 11, 57, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 5, 57, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 5, 4, 3, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(5, 4, 3)"));

            // Negative minutes are before the hour.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 55, 57, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(-5)"));

            // Fractional minutes are ignored.
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 13, 57, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(13.9)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 22, 55, 57, DateTimeKind.Utc)), Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(-5.9)"));

            Assert.AreEqual(double.NaN, Evaluate("new Date('24 Apr 2010 23:59:57 GMT').setUTCMinutes(NaN)"));
            Assert.AreEqual(3, Evaluate("new Date().setUTCMinutes.length"));
        }

        [TestMethod]
        public void setUTCMonth()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 12, 24, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCMonth(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCMonth(13)"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 2, 24, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 6, 4, 23, 59, 57, DateTimeKind.Utc)), Evaluate("x.setUTCMonth(5, 4)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setUTCMonth(NaN)"));
            Assert.AreEqual(2, Evaluate("x.setUTCMonth.length"));
        }

        [TestMethod]
        public void setUTCSeconds()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57 GMT')");
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 11, DateTimeKind.Utc)), Evaluate("x.setUTCSeconds(11)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5, DateTimeKind.Utc)), Evaluate("x.setUTCSeconds(65)"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 5, DateTimeKind.Utc)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 25, 0, 0, 4, 3, DateTimeKind.Utc)), Evaluate("x.setUTCSeconds(4, 3)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setUTCSeconds(NaN)"));
            Assert.AreEqual(2, Evaluate("x.setUTCSeconds.length"));
        }

        [TestMethod]
        public void setYear()
        {
            Evaluate("var x = new Date('24 Apr 2010 23:59:57')");
            Assert.AreEqual(ToJSDate(new DateTime(1999, 4, 24, 23, 59, 57)), Evaluate("x.setYear(99)"));
            Assert.AreEqual(ToJSDate(new DateTime(105, 4, 24, 23, 59, 57)), Evaluate("x.setYear(105)"));
            Assert.AreEqual(ToJSDate(new DateTime(105, 4, 24, 23, 59, 57)), Evaluate("x.valueOf()"));
            Assert.AreEqual(ToJSDate(new DateTime(2011, 4, 24, 23, 59, 57)), Evaluate("x.setYear(2011)"));
            Assert.AreEqual(double.NaN, Evaluate("x.setYear(NaN)"));
            Assert.AreEqual(1, Evaluate("x.setYear.length"));
        }

        [TestMethod]
        public void toDateString()
        {
            Assert.AreEqual("Sat Apr 24 2010", Evaluate("new Date('24 Apr 2010 23:59:57').toDateString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toDateString()"));
            Assert.AreEqual(0, Evaluate("new Date().toDateString.length"));
        }

        [TestMethod]
        public void toGMTString()
        {
            Assert.AreEqual("Sat, 24 Apr 2010 23:59:57 GMT", Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toGMTString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toGMTString()"));
            Assert.AreEqual(0, Evaluate("new Date().toGMTString.length"));
        }

        [TestMethod]
        public void toISOString()
        {
            Assert.AreEqual("1970-01-01T00:00:00.012Z", Evaluate("new Date(12).toISOString()"));
            Assert.AreEqual("2010-04-24T23:59:57.000Z", Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toISOString()"));
            Assert.AreEqual("RangeError", EvaluateExceptionType("new Date(NaN).toISOString()"));
            Assert.AreEqual(0, Evaluate("new Date().toISOString.length"));
        }

        [TestMethod]
        public void toJSON()
        {
            Assert.AreEqual("1970-01-01T00:00:00.012Z", Evaluate("new Date(12).toJSON()"));
            Assert.AreEqual("2010-04-24T23:59:57.000Z", Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toJSON()"));
            Assert.AreEqual(Jurassic.Null.Value, Evaluate("new Date(NaN).toJSON()"));
            Assert.AreEqual(1, Evaluate("new Date().toJSON.length"));

            // toJSON is generic.
            Assert.AreEqual("abc", Evaluate("x = {toISOString: function() { return 'abc'; }, f: new Date().toJSON }; x.f()"));
        }

        [TestMethod]
        public void toLocaleDateString()
        {
            Assert.AreEqual(new DateTime(2010, 4, 24).ToLongDateString(), Evaluate("new Date('24 Apr 2010 23:59:57').toLocaleDateString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toLocaleDateString()"));
            Assert.AreEqual(0, Evaluate("new Date().toLocaleDateString.length"));
        }

        [TestMethod]
        public void toLocaleString()
        {
            Assert.AreEqual(new DateTime(2010, 4, 24, 23, 59, 57).ToString("F"), Evaluate("new Date('24 Apr 2010 23:59:57').toLocaleString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toLocaleString()"));
            Assert.AreEqual(0, Evaluate("new Date().toLocaleString.length"));
        }

        [TestMethod]
        public void toLocaleTimeString()
        {
            Assert.AreEqual(new DateTime(2010, 4, 24, 23, 59, 57).ToLongTimeString(), Evaluate("new Date('24 Apr 2010 23:59:57').toLocaleTimeString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toLocaleTimeString()"));
            Assert.AreEqual(0, Evaluate("new Date().toLocaleTimeString.length"));
        }

        [TestMethod]
        public void toString()
        {
            Assert.AreEqual("Sat Apr 24 2010 23:59:57 " + GetTimezoneString(DateTime.Parse("24 Apr 2010 23:59:57")), Evaluate("new Date('24 Apr 2010 23:59:57').toString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toString()"));
            Assert.AreEqual(0, Evaluate("new Date().toString.length"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date().toString.call(5)"));
        }

        [TestMethod]
        public void toTimeString()
        {
            Assert.AreEqual("23:59:57 " + GetTimezoneString(DateTime.Parse("24 Apr 2010 23:59:57")), Evaluate("new Date('24 Apr 2010 23:59:57').toTimeString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toTimeString()"));
            Assert.AreEqual(0, Evaluate("new Date().toTimeString.length"));
        }

        [TestMethod]
        public void toUTCString()
        {
            Assert.AreEqual("Sat, 24 Apr 2010 23:59:57 GMT", Evaluate("new Date('24 Apr 2010 23:59:57 GMT').toUTCString()"));
            Assert.AreEqual("Invalid Date", Evaluate("new Date(NaN).toUTCString()"));
            Assert.AreEqual(0, Evaluate("new Date().toUTCString.length"));
        }

        [TestMethod]
        public void valueOf()
        {
            Assert.AreEqual(ToJSDate(new DateTime(2010, 4, 24, 23, 59, 57)), Evaluate("new Date('24 Apr 2010 23:59:57').valueOf()"));
            double value = (double)Evaluate("new Date().valueOf()");
            Assert.AreEqual(0.0, value - Math.Floor(value));
            Assert.AreEqual(0, Evaluate("new Date().valueOf.length"));
        }

        [TestMethod]
        public void IsValid()
        {
            Assert.AreEqual(false, ((DateInstance)Evaluate("new Date(NaN)")).IsValid);
            Assert.AreEqual(false, ((DateInstance)Evaluate("new Date(undefined)")).IsValid);
            Assert.AreEqual(true, ((DateInstance)Evaluate("new Date()")).IsValid);
        }

        [TestMethod]
        public void DateConversion()
        {
            // Init Jurassic
            Evaluate("");

            // Simulate "new Date()" (which uses DateTime.Now) at 2016-01-01T11:59:59.9999999Z.
            var dateInstanceValueField = typeof(DateInstance).GetField("value", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            DateInstance specialNowDate1 = new DateInstance(jurassicScriptEngine.Date.InstancePrototype);
            dateInstanceValueField.SetValue(specialNowDate1, new DateTime(635872463999999999L, DateTimeKind.Utc));
            jurassicScriptEngine.SetGlobalValue("specialDate1", specialNowDate1);

            Assert.AreEqual(Evaluate("specialDate1.toUTCString()"), Evaluate("new Date(specialDate1.getTime()).toUTCString()"));
            Assert.AreEqual(1451649599999d, Convert.ToDouble(Evaluate("specialDate1.getTime()")));

            // Simulate "new Date" at 1969-12-31T23:59:59.9999999Z.
            DateInstance specialNowDate2 = new DateInstance(jurassicScriptEngine.Date.InstancePrototype);
            dateInstanceValueField.SetValue(specialNowDate2, new DateTime(621355967999999999L, DateTimeKind.Utc));
            jurassicScriptEngine.SetGlobalValue("specialDate2", specialNowDate2);

            Assert.AreEqual(-1, Evaluate("specialDate2.getTime()"));
        }



        private static object ToJSDate(DateTime dateTime)
        {
            // Once we target .NET 4.6, use DateTimeOffset.ToUnixTimeMilliseconds().
            double result = dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond -
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / TimeSpan.TicksPerMillisecond;
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
                offset.Hours >= 0 ? "+" : "");
        }
    }
}
