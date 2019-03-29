using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jurassic.Library
{
    /// <summary>
    /// The prototype for the Date object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplayValue,nq}", Type = "{DebuggerDisplayType,nq}")]
    [DebuggerTypeProxy(typeof(ObjectInstanceDebugView))]
    public partial class DateInstance : ObjectInstance
    {
        /// <summary>
        /// The underlying DateTime value. Its Kind is always DateTimeKind.Utc.
        /// The value <c>null</c> represents an invalid date.
        /// </summary>
        private DateTime? value;



        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Date instance and initializes it to the current time.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        public DateInstance(ObjectInstance prototype)
            : this(prototype, GetUtcNow())
        {
        }

        /// <summary>
        /// Creates a new Date instance from the given date value.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </param>
        public DateInstance(ObjectInstance prototype, double value)
            : this(prototype, ToUtcDateTime(value))
        {
        }

        /// <summary>
        /// Creates a new Date instance from the given date string.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="dateStr"> A string representing a date, expressed in RFC 1123 format. </param>
        public DateInstance(ObjectInstance prototype, string dateStr)
            : this(prototype, DateParser.Parse(dateStr))
        {
        }

        /// <summary>
        /// Creates a new Date instance from various date components, expressed in local time.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <remarks>
        /// If any of the parameters are out of range, then the other values are modified accordingly.
        /// </remarks>
        public DateInstance(ObjectInstance prototype, int year, int month, int day = 1, int hour = 0,
            int minute = 0, int second = 0, int millisecond = 0)
            : this(prototype, ToUtcDateTime(prototype.Engine, year >= 0 && year < 100 ? year + 1900 : year, month, day, hour, minute, second, millisecond, DateTimeKind.Local))
        {
        }

        /// <summary>
        /// Creates a new Date instance from the given date.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="dateTime"> The date to set the instance value to. </param>
        private DateInstance(ObjectInstance prototype, DateTime? dateTime)
            : base(prototype)
        {
            this.value = ConvertTimeToUtc(prototype.Engine, dateTime);
        }

        /// <summary>
        /// Creates the Date prototype object.
        /// </summary>
        /// <param name="engine"> The script environment. </param>
        /// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
        internal static ObjectInstance CreatePrototype(ScriptEngine engine, DateConstructor constructor)
        {
            var result = engine.Object.Construct();
            var properties = GetDeclarativeProperties(engine);
            properties.Add(new PropertyNameAndValue("constructor", constructor, PropertyAttributes.NonEnumerable));
            result.FastSetProperties(properties);
            return result;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the date represented by this object in standard .NET DateTime format.
        /// </summary>
        /// <exception cref="InvalidOperationException">If <see cref="IsValid"/> returns <c>false</c>.</exception>
        public DateTime Value
        {
            get {
                if (!this.IsValid)
                    throw new InvalidOperationException("Invalid date.");

                return this.value.Value;
            }
        }

        /// <summary>
        /// Gets the date represented by this object as the number of milliseconds elapsed since
        /// January 1, 1970, 00:00:00 UTC.
        /// </summary>
        public double ValueInMilliseconds
        {
            get { return ToJSDate(this.value); }
        }

        /// <summary>
        /// Gets a value indicating whether the date instance is valid.  A date can be invalid if
        /// NaN is passed to any of the constructor parameters.
        /// </summary>
        public bool IsValid
        {
            get { return this.value.HasValue; }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayValue
        {
            get
            {
                return DateInstance.ToString(this);
            }
        }

        /// <summary>
        /// Gets value, that will be displayed in debugger watch window when this object is part of array, map, etc.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayShortValue
        {
            get { return this.DebuggerDisplayValue; }
        }

        /// <summary>
        /// Gets type, that will be displayed in debugger watch window.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override string DebuggerDisplayType
        {
            get { return "Date"; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________


        /// <summary>
        /// Returns the year component of this date, according to local time.
        /// </summary>
        /// <returns> The year component of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getFullYear")]
        public double GetFullYear()
        {
            return GetDateComponent(DateComponent.Year, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the year component of this date as an offset from 1900, according to local time.
        /// </summary>
        /// <returns> The year component of this date as an offset from 1900, according to local time. </returns>
        [JSInternalFunction(Deprecated = true, Name = "getYear")]
        public double GetYear()
        {
            return GetDateComponent(DateComponent.Year, DateTimeKind.Local) - 1900;
        }

        /// <summary>
        /// Returns the month component of this date, according to local time.
        /// </summary>
        /// <returns> The month component (0-11) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getMonth")]
        public double GetMonth()
        {
            return GetDateComponent(DateComponent.Month, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the day of the month component of this date, according to local time.
        /// </summary>
        /// <returns> The day of the month component (1-31) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getDate")]
        public double GetDate()
        {
            return GetDateComponent(DateComponent.Day, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the day of the week component of this date, according to local time.
        /// </summary>
        /// <returns> The day of the week component (0-6) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getDay")]
        public double GetDay()
        {
            return GetDateComponent(DateComponent.DayOfWeek, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the hour component of this date, according to local time.
        /// </summary>
        /// <returns> The hour component (0-23) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getHours")]
        public double GetHours()
        {
            return GetDateComponent(DateComponent.Hour, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the minute component of this date, according to local time.
        /// </summary>
        /// <returns> The minute component (0-59) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getMinutes")]
        public double GetMinutes()
        {
            return GetDateComponent(DateComponent.Minute, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the seconds component of this date, according to local time.
        /// </summary>
        /// <returns> The seconds component (0-59) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getSeconds")]
        public double GetSeconds()
        {
            return GetDateComponent(DateComponent.Second, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the millisecond component of this date, according to local time.
        /// </summary>
        /// <returns> The millisecond component (0-999) of this date, according to local time. </returns>
        [JSInternalFunction(Name = "getMilliseconds")]
        public double GetMilliseconds()
        {
            return GetDateComponent(DateComponent.Millisecond, DateTimeKind.Local);
        }

        /// <summary>
        /// Returns the number of milliseconds since January 1, 1970, 00:00:00 UTC.
        /// </summary>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </returns>
        [JSInternalFunction(Name = "getTime")]
        public double GetTime()
        {
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Returns the time-zone offset in minutes for the current locale.
        /// </summary>
        /// <returns> The time-zone offset in minutes for the current locale. </returns>
        [JSInternalFunction(Name = "getTimezoneOffset")]
        public double GetTimezoneOffset()
        {
            if (!IsValid)
                return double.NaN;
            return -(int)Engine.LocalTimeZone.GetUtcOffset(this.Value).TotalMinutes;
        }

        /// <summary>
        /// Returns the year component of this date, according to universal time.
        /// </summary>
        /// <returns> The year component of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCFullYear")]
        public double GetUTCFullYear()
        {
            return GetDateComponent(DateComponent.Year, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the month component of this date, according to universal time.
        /// </summary>
        /// <returns> The month component (0-11) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCMonth")]
        public double GetUTCMonth()
        {
            return GetDateComponent(DateComponent.Month, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the day of the month component of this date, according to universal time.
        /// </summary>
        /// <returns> The day of the month component (1-31) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCDate")]
        public double GetUTCDate()
        {
            return GetDateComponent(DateComponent.Day, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the day of the week component of this date, according to universal time.
        /// </summary>
        /// <returns> The day of the week component (0-6) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCDay")]
        public double GetUTCDay()
        {
            return GetDateComponent(DateComponent.DayOfWeek, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the hour component of this date, according to universal time.
        /// </summary>
        /// <returns> The hour component (0-23) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCHours")]
        public double GetUTCHours()
        {
            return GetDateComponent(DateComponent.Hour, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the minute component of this date, according to universal time.
        /// </summary>
        /// <returns> The minute component (0-59) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCMinutes")]
        public double GetUTCMinutes()
        {
            return GetDateComponent(DateComponent.Minute, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the seconds component of this date, according to universal time.
        /// </summary>
        /// <returns> The seconds component (0-59) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCSeconds")]
        public double GetUTCSeconds()
        {
            return GetDateComponent(DateComponent.Second, DateTimeKind.Utc);
        }

        /// <summary>
        /// Returns the millisecond component of this date, according to universal time.
        /// </summary>
        /// <returns> The millisecond component (0-999) of this date, according to universal time. </returns>
        [JSInternalFunction(Name = "getUTCMilliseconds")]
        public double GetUTCMilliseconds()
        {
            return GetDateComponent(DateComponent.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// Sets the full year (4 digits for 4-digit years) of this date, according to local time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setFullYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetFullYear(double year)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Local, year);
        }

        /// <summary>
        /// Sets the full year (4 digits for 4-digit years) of this date, according to local time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setFullYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetFullYear(double year, double month)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Local, year, month);
        }

        /// <summary>
        /// Sets the full year (4 digits for 4-digit years) of this date, according to local time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setFullYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetFullYear(double year, double month, double day)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Local, year, month, day);
        }

        /// <summary>
        /// Sets the year of this date, according to local time.
        /// </summary>
        /// <param name="year"> The year.  Numbers less than 100 will be assumed to be  </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Deprecated = true, Name = "setYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetYear(double year)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Local, year >= 0 && year < 100 ? year + 1900 : year);
        }

        /// <summary>
        /// Sets the month of this date, according to local time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setMonth", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetMonth(double month)
        {
            return SetDateComponents(DateComponent.Month, DateTimeKind.Local, month);
        }

        /// <summary>
        /// Sets the month of this date, according to local time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setMonth", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetMonth(double month, double day)
        {
            return SetDateComponents(DateComponent.Month, DateTimeKind.Local, month, day);
        }

        /// <summary>
        /// Sets the day of this date, according to local time.
        /// </summary>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setDate", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetDate(double day)
        {
            return SetDateComponents(DateComponent.Day, DateTimeKind.Local, day);
        }

        /// <summary>
        /// Sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetHours(double hour)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Local, hour);
        }

        /// <summary>
        /// Sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetHours(double hour, double minute)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Local, hour, minute);
        }

        /// <summary>
        /// Sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetHours(double hour, double minute, double second)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Local, hour, minute, second);
        }

        /// <summary>
        /// Sets the hours component of this date, according to local time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetHours(double hour, double minute, double second, double millisecond)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Local, hour, minute, second, millisecond);
        }

        /// <summary>
        /// Sets the minutes component of this date, according to local time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setMinutes", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetMinutes(double minute)
        {
            return SetDateComponents(DateComponent.Minute, DateTimeKind.Local, minute);
        }

        /// <summary>
        /// Sets the minutes component of this date, according to local time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setMinutes", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetMinutes(double minute, double second)
        {
            return SetDateComponents(DateComponent.Minute, DateTimeKind.Local, minute, second);
        }

        /// <summary>
        /// Sets the minutes component of this date, according to local time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setMinutes", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetMinutes(double minute, double second, double millisecond)
        {
            return SetDateComponents(DateComponent.Minute, DateTimeKind.Local, minute, second, millisecond);
        }

        /// <summary>
        /// Sets the seconds component of this date, according to local time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setSeconds", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetSeconds(double second)
        {
            return SetDateComponents(DateComponent.Second, DateTimeKind.Local, second);
        }

        /// <summary>
        /// Sets the seconds component of this date, according to local time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setSeconds", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetSeconds(double second, double millisecond)
        {
            return SetDateComponents(DateComponent.Second, DateTimeKind.Local, second, millisecond);
        }

        /// <summary>
        /// Sets the milliseconds component of this date, according to local time.
        /// </summary>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setMilliseconds", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetMilliseconds(double millisecond)
        {
            return SetDateComponents(DateComponent.Millisecond, DateTimeKind.Local, millisecond);
        }

        /// <summary>
        /// Sets the full year (4 digits for 4-digit years) of this date, according to universal time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCFullYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCFullYear(double year)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Utc, year);
        }

        /// <summary>
        /// Sets the full year (4 digits for 4-digit years) of this date, according to universal time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCFullYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCFullYear(double year, double month)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Utc, year, month);
        }

        /// <summary>
        /// Sets the full year (4 digits for 4-digit years) of this date, according to universal time.
        /// </summary>
        /// <param name="year"> The 4 digit year. </param>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCFullYear", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCFullYear(double year, double month, double day)
        {
            return SetDateComponents(DateComponent.Year, DateTimeKind.Utc, year, month, day);
        }

        /// <summary>
        /// Sets the month of this date, according to universal time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCMonth", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCMonth(double month)
        {
            return SetDateComponents(DateComponent.Month, DateTimeKind.Utc, month);
        }

        /// <summary>
        /// Sets the month of this date, according to universal time.
        /// </summary>
        /// <param name="month"> The month (0-11). </param>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCMonth", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCMonth(double month, double day)
        {
            return SetDateComponents(DateComponent.Month, DateTimeKind.Utc, month, day);
        }

        /// <summary>
        /// Sets the day of this date, according to universal time.
        /// </summary>
        /// <param name="day"> The day of the month (1-31). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCDate", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCDate(double day)
        {
            return SetDateComponents(DateComponent.Day, DateTimeKind.Utc, day);
        }

        /// <summary>
        /// Sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCHours(double hour)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour);
        }

        /// <summary>
        /// Sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCHours(double hour, double minute)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour, minute);
        }

        /// <summary>
        /// Sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCHours(double hour, double minute, double second)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour, minute, second);
        }

        /// <summary>
        /// Sets the hours component of this date, according to universal time.
        /// </summary>
        /// <param name="hour"> The number of hours since midnight (0-23). </param>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCHours", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCHours(double hour, double minute, double second, double millisecond)
        {
            return SetDateComponents(DateComponent.Hour, DateTimeKind.Utc, hour, minute, second, millisecond);
        }

        /// <summary>
        /// Sets the minutes component of this date, according to universal time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCMinutes", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCMinutes(double minute)
        {
            return SetDateComponents(DateComponent.Minute, DateTimeKind.Utc, minute);
        }

        /// <summary>
        /// Sets the minutes component of this date, according to universal time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCMinutes", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCMinutes(double minute, double second)
        {
            return SetDateComponents(DateComponent.Minute, DateTimeKind.Utc, minute, second);
        }

        /// <summary>
        /// Sets the minutes component of this date, according to universal time.
        /// </summary>
        /// <param name="minute"> The number of minutes since the hour (0-59). </param>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCMinutes", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCMinutes(double minute, double second, double millisecond)
        {
            return SetDateComponents(DateComponent.Minute, DateTimeKind.Utc, minute, second, millisecond);
        }

        /// <summary>
        /// Sets the seconds component of this date, according to universal time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCSeconds", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCSeconds(double second)
        {
            return SetDateComponents(DateComponent.Second, DateTimeKind.Utc, second);
        }

        /// <summary>
        /// Sets the seconds component of this date, according to universal time.
        /// </summary>
        /// <param name="second"> The number of seconds since the minute (0-59). </param>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCSeconds", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCSeconds(double second, double millisecond)
        {
            return SetDateComponents(DateComponent.Second, DateTimeKind.Utc, second, millisecond);
        }

        /// <summary>
        /// Sets the milliseconds component of this date, according to universal time.
        /// </summary>
        /// <param name="millisecond"> The number of milliseconds since the second (0-999). </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setUTCMilliseconds", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetUTCMilliseconds(double millisecond)
        {
            return SetDateComponents(DateComponent.Millisecond, DateTimeKind.Utc, millisecond);
        }

        /// <summary>
        /// Sets the date and time value of ths date.
        /// </summary>
        /// <param name="millisecond"> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        [JSInternalFunction(Name = "setTime", Flags = JSFunctionFlags.MutatesThisObject)]
        public double SetTime(double millisecond)
        {
            this.value = ToUtcDateTime(millisecond);
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Returns the date as a string.
        /// </summary>
        /// <returns> The date as a string. </returns>
        [JSInternalFunction(Name = "toDateString")]
        public string ToDateString()
        {
            if (!this.IsValid)
                return "Invalid Date";
            return ConvertTimeFromUtc(Engine, this.Value).Value.ToString("ddd MMM dd yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns the date as a string using GMT (Greenwich Mean Time).
        /// </summary>
        /// <returns> The date as a string. </returns>
        [JSInternalFunction(Deprecated = true, Name = "toGMTString")]
        public string ToGMTString()
        {
            if (!this.IsValid)
                return "Invalid Date";
            return ConvertTimeToUtc(Engine, this.Value).Value.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns the date as a string using GMT (Greenwich Mean Time).
        /// </summary>
        /// <returns> The date as a string. </returns>
        [JSInternalFunction(Name = "toISOString")]
        public string ToISOString()
        {
            if (!this.IsValid)
                throw new JavaScriptException(this.Engine, ErrorType.RangeError, "The date is invalid");
            return ConvertTimeToUtc(Engine, this.Value).Value.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Used by the JSON.stringify to transform objects prior to serialization.
        /// </summary>
        /// <param name="thisObject"> The object that is being operated on. </param>
        /// <param name="key"> Unused. </param>
        /// <returns> The date as a serializable string. </returns>
        [JSInternalFunction(Name = "toJSON", Flags = JSFunctionFlags.HasThisObject)]
        public static object ToJSON(ObjectInstance thisObject, string key)
        {
            var number = TypeConverter.ToPrimitive(thisObject, PrimitiveTypeHint.Number);
            if (number is double && (double.IsInfinity((double)number) || double.IsNaN((double)number)))
                return Null.Value;
            return thisObject.CallMemberFunction("toISOString");
        }

        /// <summary>
        /// Returns the date as a string using the current locale settings.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "toLocaleDateString")]
        public string ToLocaleDateString()
        {
            if (!this.IsValid)
                return "Invalid Date";
            return ConvertTimeFromUtc(Engine, this.Value).Value.ToString("D", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns the date and time as a string using the current locale settings.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "toLocaleString")]
        public new string ToLocaleString()
        {
            if (!this.IsValid)
                return "Invalid Date";
            return ConvertTimeFromUtc(Engine, this.Value).Value.ToString("F", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns the time as a string using the current locale settings.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "toLocaleTimeString")]
        public string ToLocaleTimeString()
        {
            if (!this.IsValid)
                return "Invalid Date";
            return ConvertTimeFromUtc(Engine, this.Value).Value.ToString("T", System.Globalization.DateTimeFormatInfo.CurrentInfo);
        }

        /// <summary>
        /// Returns a string representing the date and time.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="thisRef"> The object that is being operated on. </param>
        /// <returns> A string representing the date and time. </returns>
        [JSInternalFunction(Name = "toString", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject)]
        public static string ToString(ScriptEngine engine, object thisRef)
        {
            // As of ES6, this method is generic.
            if ((thisRef is DateInstance) == false)
                return "Invalid Date";

            var instance = (DateInstance)thisRef;
            if (!instance.IsValid)
                return "Invalid Date";

            var dateTime = ConvertTimeFromUtc(instance.Engine, instance.Value);
            return dateTime.Value.ToString("ddd MMM dd yyyy HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                ToTimeZoneString(engine, dateTime.Value);
        }

        /// <summary>
        /// Returns the time as a string.
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "toTimeString")]
        public string ToTimeString()
        {
            if (!this.IsValid)
                return "Invalid Date";

            var dateTime = ConvertTimeFromUtc(Engine, this.Value);
            return dateTime.Value.ToString("HH:mm:ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) +
                ToTimeZoneString(Engine, dateTime.Value);
        }

        /// <summary>
        /// Returns the date as a string using UTC (universal time).
        /// </summary>
        /// <returns></returns>
        [JSInternalFunction(Name = "toUTCString")]
        public string ToUTCString()
        {
            if (!this.IsValid)
                return "Invalid Date";
            return ConvertTimeToUtc(Engine, this.Value).Value.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Returns the primitive value of this object.
        /// </summary>
        /// <returns> The primitive value of this object. </returns>
        [JSInternalFunction(Name = "valueOf")]
        public new double ValueOf()
        {
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Returns a primitive value that represents the current object.  Used by the addition and
        /// equality operators.
        /// </summary>
        /// <param name="engine"> The current script environment. </param>
        /// <param name="thisObj"> The object to operate on. </param>
        /// <param name="hint"> Specifies the conversion behaviour.  Must be "default", "string" or "number". </param>
        /// <returns></returns>
        [JSInternalFunction(Name = "@@toPrimitive", Flags = JSFunctionFlags.HasEngineParameter | JSFunctionFlags.HasThisObject, RequiredArgumentCount = 1)]
        private static object ToPrimitive(ScriptEngine engine, ObjectInstance thisObj, string hint)
        {
            // This behaviour differs from the standard behaviour only in that the "default" hint
            // results in a conversion to a string, not a number.
            if (hint == "default" || hint == "string")
                return thisObj.GetPrimitiveValuePreES6(PrimitiveTypeHint.String);
            if (hint == "number")
                return thisObj.GetPrimitiveValuePreES6(PrimitiveTypeHint.Number);
            throw new JavaScriptException(engine, ErrorType.TypeError, "Invalid type hint.");
        }



        //     STATIC JAVASCRIPT METHODS (FROM DATECONSTRUCTOR)
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns the current date and time as the number of milliseconds elapsed since January 1,
        /// 1970, 00:00:00 UTC.
        /// </summary>
        /// <returns> The current date and time as the number of milliseconds elapsed since January 1,
        /// 1970, 00:00:00 UTC. </returns>
        public static double Now()
        {
            return ToJSDate(GetUtcNow());
        }

        /// <summary>
        /// Given the components of a UTC date, returns the number of milliseconds since January 1,
        /// 1970, 00:00:00 UTC to that date.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC to the given
        /// date. </returns>
        /// <remarks>
        /// This method differs from the Date constructor in two ways:
        /// 1. The date components are specified in UTC time rather than local time.
        /// 2. A number is returned instead of a Date instance.
        /// 
        /// If any of the parameters are out of range, then the other values are modified accordingly.
        /// </remarks>
        public static double UTC(ScriptEngine engine, int year, int month, int day = 1, int hour = 0,
            int minute = 0, int second = 0, int millisecond = 0)
        {
            return ToJSDate(ToUtcDateTime(engine, year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc));
        }

        /// <summary>
        /// Parses a string representation of a date, and returns the number of milliseconds since
        /// January 1, 1970, 00:00:00 UTC.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="dateStr"> A string representing a date, expressed in RFC 1123 format. </param>
        public static double Parse(ScriptEngine engine, string dateStr)
        {
            return ToJSDate(ConvertTimeToUtc(engine, DateParser.Parse(dateStr)));
        }



        //     PRIVATE IMPLEMENTATION METHODS
        //_________________________________________________________________________________________


        private enum DateComponent
        {
            Year = 0,
            Month = 1,
            Day = 2,
            Hour = 3,
            Minute = 4,
            Second = 5,
            Millisecond = 6,
            DayOfWeek,
        }

        /// <summary>
        /// Gets a single component of this date.
        /// </summary>
        /// <param name="component"> The date component to extract. </param>
        /// <param name="localOrUniversal"> Indicates whether to retrieve the component in local
        /// or universal time. </param>
        /// <returns> The date component value, or <c>NaN</c> if the date is invalid. </returns>
        private double GetDateComponent(DateComponent component, DateTimeKind localOrUniversal)
        {
            if (!this.IsValid)
                return double.NaN;

            DateTime value = this.Value;
            
            // Convert the date to local or universal time.
            switch (localOrUniversal)
            {
                case DateTimeKind.Local:
                    value = ConvertTimeFromUtc(Engine, value).Value;
                    break;                    
                case DateTimeKind.Utc:
                    value = ConvertTimeToUtc(Engine, value).Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("localOrUniversal");
            }

            // Extract the requested component.
            switch (component)
            {
                case DateComponent.Year:
                    return value.Year;
                case DateComponent.Month:
                    return value.Month - 1;    // Javascript month is 0-11.
                case DateComponent.Day:
                    return value.Day;
                case DateComponent.DayOfWeek:
                    return (double)value.DayOfWeek;
                case DateComponent.Hour:
                    return value.Hour;
                case DateComponent.Minute:
                    return value.Minute;
                case DateComponent.Second:
                    return value.Second;
                case DateComponent.Millisecond:
                    return value.Millisecond;
                default:
                    throw new ArgumentOutOfRangeException("component");
            }
        }

        /// <summary>
        /// Sets one or more components of this date.
        /// </summary>
        /// <param name="firstComponent"> The first date component to set. </param>
        /// <param name="localOrUniversal"> Indicates whether to set the component(s) in local
        /// or universal time. </param>
        /// <param name="componentValues"> One or more date component values. </param>
        /// <returns> The number of milliseconds elapsed since January 1, 1970, 00:00:00 UTC for
        /// the new date. </returns>
        private double SetDateComponents(DateComponent firstComponent, DateTimeKind localOrUniversal, params double[] componentValues)
        {
            // When operating on an invalid date, we only set the date if
            // the year is actually specified. This matches the behavior of
            // other JS engines.
            if (!this.IsValid && firstComponent != DateComponent.Year)
            {
                return this.ValueInMilliseconds;
            }

            DateTime value = this.value ?? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);            
            
            // Convert the date to local or universal time.
            switch (localOrUniversal)
            {
                case DateTimeKind.Local:
                    value = ConvertTimeFromUtc(Engine, value).Value;
                    break;
                case DateTimeKind.Utc:
                    value = ConvertTimeToUtc(Engine, value).Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("localOrUniversal");
            }

            // Get the current component values of the date.
            int[] allComponentValues = new int[7];
            allComponentValues[0] = value.Year;
            allComponentValues[1] = value.Month - 1;   // Javascript month is 0-11.
            allComponentValues[2] = value.Day;
            allComponentValues[3] = value.Hour;
            allComponentValues[4] = value.Minute;
            allComponentValues[5] = value.Second;
            allComponentValues[6] = value.Millisecond;

            // Overwrite the component values with the new ones that were passed in.
            for (int i = 0; i < componentValues.Length; i++)
            {
                double componentValue = componentValues[i];
                if (double.IsNaN(componentValue) || double.IsInfinity(componentValue))
                {
                    this.value = null;
                    return this.ValueInMilliseconds;
                }
                allComponentValues[(int)firstComponent + i] = (int)componentValue;
            }

            // Construct a new date.
            this.value = ToUtcDateTime(Engine, allComponentValues[0], allComponentValues[1], allComponentValues[2],
                allComponentValues[3], allComponentValues[4], allComponentValues[5], allComponentValues[6],
                localOrUniversal);

            // Return the date value.
            return this.ValueInMilliseconds;
        }

        /// <summary>
        /// Converts a .NET date into a javascript date.
        /// </summary>
        /// <param name="utcDateTime"> The .NET date. </param>
        /// <returns> The number of milliseconds since January 1, 1970, 00:00:00 UTC </returns>
        private static double ToJSDate(DateTime? utcDateTime)
        {
            if (utcDateTime == null)
                return double.NaN;
            if (utcDateTime.Value.Kind != DateTimeKind.Utc)
                throw new ArgumentException("DateTime.Kind needs to be Utc");
            // The spec requires that the time value is an integer.
            // We could round to nearest, but then date.toUTCString() would be different from Date(date.getTime()).toUTCString().
            return Math.Floor(utcDateTime.Value.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
        }

        /// <summary>
        /// Converts a javascript date into a .NET date.
        /// </summary>
        /// <param name="milliseconds"> The number of milliseconds since January 1, 1970, 00:00:00 UTC. </param>
        /// <returns> The equivalent .NET date. </returns>
        private static DateTime? ToUtcDateTime(double milliseconds)
        {
            // Check if the milliseconds value is out of range.
            if (double.IsNaN(milliseconds))
                return null;

            try
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    .AddMilliseconds(Math.Truncate(milliseconds));
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// Given the components of a date, returns the equivalent .NET date.
        /// </summary>
        /// <param name="engine">The engine.</param>
        /// <param name="year"> The full year. </param>
        /// <param name="month"> The month as an integer between 0 and 11 (january to december). </param>
        /// <param name="day"> The day of the month, from 1 to 31.  Defaults to 1. </param>
        /// <param name="hour"> The number of hours since midnight, from 0 to 23.  Defaults to 0. </param>
        /// <param name="minute"> The number of minutes, from 0 to 59.  Defaults to 0. </param>
        /// <param name="second"> The number of seconds, from 0 to 59.  Defaults to 0. </param>
        /// <param name="millisecond"> The number of milliseconds, from 0 to 999.  Defaults to 0. </param>
        /// <param name="kind"> Indicates whether the components are in UTC or local time. </param>
        /// <returns> The equivalent .NET date. </returns>
        private static DateTime? ToUtcDateTime(ScriptEngine engine, int year, int month, int day, int hour, int minute, int second, int millisecond, DateTimeKind kind)
        {
            // DateTime doesn't support years below year 1.
            if (year < 0)
                return null;

            DateTime value;
            if (month >= 0 && month < 12 &&
                day >= 1 && day <= DateTime.DaysInMonth(year, month + 1) &&
                hour >= 0 && hour < 24 &&
                minute >= 0 && minute < 60 &&
                second >= 0 && second < 60 &&
                millisecond >= 0 && millisecond < 1000)
            {
                // All parameters are in range.
                value = new DateTime(year, month + 1, day, hour, minute, second, millisecond, kind);
            }
            else
            {
                // One or more parameters are out of range.
                try
                {
                    value = new DateTime(year, 1, 1, 0, 0, 0, kind);
                    value = value.AddMonths(month);
                    if (day != 1)
                        value = value.AddDays(day - 1);
                    if (hour != 0)
                        value = value.AddHours(hour);
                    if (minute != 0)
                        value = value.AddMinutes(minute);
                    if (second != 0)
                        value = value.AddSeconds(second);
                    if (millisecond != 0)
                        value = value.AddMilliseconds(millisecond);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // One or more of the parameters was NaN or way too big or way too small.
                    // Return a sentinel invalid date.
                    return null;
                }
            }

            // Ensure to convert to UTC.
            return ConvertTimeToUtc(engine, value);
        }

        /// <summary>
        /// Gets the current time and date.
        /// </summary>
        /// <returns> The current time and date. </returns>
        private static DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Returns a string of the form "GMT+1200 (New Zealand Standard Time)".
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="dateTime"> The date to get the time zone information from. </param>
        /// <returns> A string of the form "GMT+1200 (New Zealand Standard Time)". </returns>
        private static string ToTimeZoneString(ScriptEngine engine, DateTime dateTime)
        {
            var timeZone = engine.LocalTimeZone;

            // Compute the time zone offset in hours-minutes.
            int offsetInMinutes = (int)timeZone.GetUtcOffset(dateTime).TotalMinutes;
            int hhmm = offsetInMinutes / 60 * 100 + offsetInMinutes % 60;

            // Get the time zone name.
            string zoneName;
            if (timeZone.IsDaylightSavingTime(dateTime))
                zoneName = timeZone.DaylightName;
            else
                zoneName = timeZone.StandardName;

            if (hhmm < 0)
                return string.Format("GMT{0:d4} ({1})", hhmm, zoneName);
            else
                return string.Format("GMT+{0:d4} ({1})", hhmm, zoneName);
        }

        private static DateTime? ConvertTimeFromUtc(ScriptEngine engine, DateTime? value)
        {
            if (value == null)
                return null;

            value = TimeZoneInfo.ConvertTimeFromUtc(value.Value, engine.LocalTimeZone);

            // Ensure that the kind is local for consistency.
            if (value.Value.Kind == DateTimeKind.Unspecified)
                DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
            return value;
        }

        private static DateTime? ConvertTimeToUtc(ScriptEngine engine, DateTime? value)
        {
            if (value == null)
                return null;
            if (value.Value.Kind == DateTimeKind.Utc)
                return value;

            // TimeZoneInfo.ConvertToUtc() expects the DateTime's Kind to be Local when the timezone
            // is reference-equal to TimeZoneInfo.Local, and expects Kind to be Unspecified when the
            // timezone is not reference-equal to TimeZoneInfo.Local.
            if (object.ReferenceEquals(TimeZoneInfo.Local, engine.LocalTimeZone))
                value = DateTime.SpecifyKind(value.Value, DateTimeKind.Local);
            else
                value = DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(value.Value, engine.LocalTimeZone);
        }

    }
}
