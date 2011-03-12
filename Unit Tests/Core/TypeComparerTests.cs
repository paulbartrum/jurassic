using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Library;

namespace UnitTests
{
    /// <summary>
    /// Test the type comparison routines.
    /// </summary>
    [TestClass]
    public class TypeComparerTests
    {
        [TestMethod]
        public void Equals()
        {
            var engine = new ScriptEngine();

            // Undefined.
            Assert.AreEqual(true,  TypeComparer.Equals(Undefined.Value, Undefined.Value));
            Assert.AreEqual(true,  TypeComparer.Equals(Undefined.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, false));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, true));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, 0));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, 0.0));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, 5.5));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, double.NaN));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, ""));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, " "));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, "5.5"));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, "foo"));
            Assert.AreEqual(false, TypeComparer.Equals(Undefined.Value, engine.Object.Construct()));

            // Null.
            Assert.AreEqual(true,  TypeComparer.Equals(Null.Value, Undefined.Value));
            Assert.AreEqual(true,  TypeComparer.Equals(Null.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, false));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, true));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, 0));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, 0.0));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, 5.5));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, double.NaN));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, ""));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, " "));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, "5.5"));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, "foo"));
            Assert.AreEqual(false, TypeComparer.Equals(Null.Value, engine.Object.Construct()));

            // Boolean.
            Assert.AreEqual(false, TypeComparer.Equals(false, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.Equals(false, Null.Value));
            Assert.AreEqual(true,  TypeComparer.Equals(false, false));
            Assert.AreEqual(false, TypeComparer.Equals(false, true));
            Assert.AreEqual(true,  TypeComparer.Equals(false, 0));
            Assert.AreEqual(true,  TypeComparer.Equals(false, 0.0));
            Assert.AreEqual(false, TypeComparer.Equals(false, 5.5));
            Assert.AreEqual(false, TypeComparer.Equals(false, double.NaN));
            Assert.AreEqual(true,  TypeComparer.Equals(false, ""));
            Assert.AreEqual(true,  TypeComparer.Equals(false, " "));
            Assert.AreEqual(true,  TypeComparer.Equals(false, "0"));
            Assert.AreEqual(false, TypeComparer.Equals(false, "1"));
            Assert.AreEqual(false, TypeComparer.Equals(false, "5.5"));
            Assert.AreEqual(false, TypeComparer.Equals(false, "false"));
            Assert.AreEqual(false, TypeComparer.Equals(false, "true"));
            Assert.AreEqual(false, TypeComparer.Equals(false, "foo"));
            Assert.AreEqual(false, TypeComparer.Equals(false, engine.Object.Construct()));
            Assert.AreEqual(true,  TypeComparer.Equals(false, engine.Boolean.Construct(false)));
            Assert.AreEqual(false, TypeComparer.Equals(false, engine.Boolean.Construct(true)));
            Assert.AreEqual(true,  TypeComparer.Equals(false, engine.Number.Construct(0)));
            Assert.AreEqual(false, TypeComparer.Equals(false, engine.Number.Construct(1)));
            Assert.AreEqual(false, TypeComparer.Equals(true, "0"));
            Assert.AreEqual(true,  TypeComparer.Equals(true, "1"));

            // Number.
            Assert.AreEqual(false, TypeComparer.Equals(5.5, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, Null.Value));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, false));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, true));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, 0));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, 0.0));
            Assert.AreEqual(true,  TypeComparer.Equals(5.5, 5.5));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, double.NaN));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, ""));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, " "));
            Assert.AreEqual(true,  TypeComparer.Equals(5.5, "5.5"));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, "foo"));
            Assert.AreEqual(false, TypeComparer.Equals(5.5, engine.Object.Construct()));
            Assert.AreEqual(true,  TypeComparer.Equals(5.5, engine.Number.Construct(5.5)));
            Assert.AreEqual(true,  TypeComparer.Equals(0, 0.0));
            Assert.AreEqual(true,  TypeComparer.Equals(5, 5.0));
            Assert.AreEqual(false, TypeComparer.Equals(double.NaN, double.NaN));
            Assert.AreEqual(false, TypeComparer.Equals(double.NaN, engine.Number.Construct(double.NaN)));

            // String.
            Assert.AreEqual(false, TypeComparer.Equals("5.5", Undefined.Value));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", Null.Value));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", false));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", true));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", 0));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", 0.0));
            Assert.AreEqual(true,  TypeComparer.Equals("5.5", 5.5));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", double.NaN));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", ""));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", " "));
            Assert.AreEqual(true,  TypeComparer.Equals("5.5", "5.5"));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", "foo"));
            Assert.AreEqual(false, TypeComparer.Equals("5.5", engine.Object.Construct()));
            Assert.AreEqual(true,  TypeComparer.Equals("5.5", engine.Number.Construct(5.5)));
            Assert.AreEqual(true,  TypeComparer.Equals("5.5", engine.String.Construct("5.5")));
            Assert.AreEqual(true,  TypeComparer.Equals("0", engine.Boolean.Construct(false)));
            Assert.AreEqual(true,  TypeComparer.Equals("1", engine.Boolean.Construct(true)));
            Assert.AreEqual(true,  TypeComparer.Equals("", engine.String.Construct()));
            Assert.AreEqual(true,  TypeComparer.Equals("", engine.String.Construct("")));

            // Object.
            var temp = engine.Object.Construct();
            Assert.AreEqual(true,  TypeComparer.Equals(temp, temp));
            Assert.AreEqual(true,  TypeComparer.Equals(engine.Boolean.Construct(false), "0"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Boolean.Construct(false), "1"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Boolean.Construct(true), "0"));
            Assert.AreEqual(true,  TypeComparer.Equals(engine.Boolean.Construct(true), "1"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Boolean.Construct(false), "false"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Boolean.Construct(false), "true"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Boolean.Construct(true), "false"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Boolean.Construct(true), "true"));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Object.Construct(), engine.Object.Construct()));
            Assert.AreEqual(false, TypeComparer.Equals(engine.Number.Construct(5.5), engine.Number.Construct(5.5)));
            Assert.AreEqual(true,  TypeComparer.Equals(engine.Number.Construct(5.5), 5.5));
            Assert.AreEqual(false, TypeComparer.Equals(engine.String.Construct("5.5"), engine.String.Construct("5.5")));
            Assert.AreEqual(true,  TypeComparer.Equals(engine.String.Construct("5.5"), 5.5));
            Assert.AreEqual(true,  TypeComparer.Equals(engine.String.Construct(""), ""));
        }

        [TestMethod]
        public void StrictEquals()
        {
            var engine = new ScriptEngine();

            // Undefined.
            Assert.AreEqual(true,  TypeComparer.StrictEquals(Undefined.Value, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, false));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, true));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, 0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, 0.0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, 5.5));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, double.NaN));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, ""));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, " "));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, "5.5"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, "foo"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Undefined.Value, engine.Object.Construct()));

            // Null.
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, Undefined.Value));
            Assert.AreEqual(true,  TypeComparer.StrictEquals(Null.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, false));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, true));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, 0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, 0.0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, 5.5));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, double.NaN));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, ""));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, " "));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, "5.5"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, "foo"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(Null.Value, engine.Object.Construct()));

            // Boolean.
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, Null.Value));
            Assert.AreEqual(true,  TypeComparer.StrictEquals(false, false));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, true));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, 0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, 0.0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, 5.5));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, double.NaN));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, ""));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, " "));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, "5.5"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, "foo"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, engine.Object.Construct()));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, engine.Boolean.Construct(false)));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, engine.Boolean.Construct(true)));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, engine.Number.Construct(0)));
            Assert.AreEqual(false, TypeComparer.StrictEquals(false, engine.Number.Construct(1)));

            // Number.
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, Null.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, false));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, true));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, 0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, 0.0));
            Assert.AreEqual(true,  TypeComparer.StrictEquals(5.5, 5.5));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, double.NaN));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, ""));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, " "));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, "5.5"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, "foo"));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, engine.Object.Construct()));
            Assert.AreEqual(false, TypeComparer.StrictEquals(5.5, engine.Number.Construct(5.5)));
            Assert.AreEqual(true,  TypeComparer.StrictEquals(0, 0.0));
            Assert.AreEqual(true,  TypeComparer.StrictEquals(5, 5.0));
            Assert.AreEqual(false, TypeComparer.StrictEquals(double.NaN, double.NaN));

            // String.
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", Undefined.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", Null.Value));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", false));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", true));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", 0));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", 0.0));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", 5.5));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", double.NaN));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", ""));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", " "));
            Assert.AreEqual(true,  TypeComparer.StrictEquals("5.5", "5.5"));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", "foo"));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", engine.Object.Construct()));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", engine.Number.Construct(5.5)));
            Assert.AreEqual(false, TypeComparer.StrictEquals("5.5", engine.String.Construct("5.5")));

            // Object.
            var temp = engine.Object.Construct();
            Assert.AreEqual(true,  TypeComparer.StrictEquals(temp, temp));
            Assert.AreEqual(false, TypeComparer.StrictEquals(engine.Object.Construct(), engine.Object.Construct()));
            Assert.AreEqual(false, TypeComparer.StrictEquals(engine.Number.Construct(5.5), engine.Number.Construct(5.5)));
            Assert.AreEqual(false, TypeComparer.StrictEquals(engine.String.Construct("5.5"), engine.String.Construct("5.5")));
        }

        [TestMethod]
        public void SameValue()
        {
            // undefined
            Assert.AreEqual(true, TypeComparer.SameValue(Undefined.Value, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Undefined.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Undefined.Value, 0));
            Assert.AreEqual(true, TypeComparer.SameValue(null, null));
            Assert.AreEqual(true, TypeComparer.SameValue(null, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(null, Null.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(null, 0));

            // null
            Assert.AreEqual(true, TypeComparer.SameValue(Null.Value, Null.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Null.Value, Undefined.Value));
            Assert.AreEqual(false, TypeComparer.SameValue(Null.Value, 0));

            // number
            Assert.AreEqual(true, TypeComparer.SameValue(+0.0, +0.0));
            Assert.AreEqual(true, TypeComparer.SameValue(-0.0, -0.0));
            Assert.AreEqual(false, TypeComparer.SameValue(+0.0, -0.0));
            Assert.AreEqual(false, TypeComparer.SameValue(-0.0, +0.0));
            Assert.AreEqual(true, TypeComparer.SameValue(1, 1));
            Assert.AreEqual(false, TypeComparer.SameValue(0, 1));
            Assert.AreEqual(true, TypeComparer.SameValue(5, 5.0));
            Assert.AreEqual(true, TypeComparer.SameValue(5.0, 5));
            Assert.AreEqual(true, TypeComparer.SameValue(5.0, 5.0));
            Assert.AreEqual(false, TypeComparer.SameValue(5.0, 6.0));
            Assert.AreEqual(true, TypeComparer.SameValue(double.NaN, double.NaN));
            Assert.AreEqual(false, TypeComparer.SameValue(double.NaN, 5));
            Assert.AreEqual(false, TypeComparer.SameValue(double.NaN, 5.0));
            Assert.AreEqual(false, TypeComparer.SameValue(0, "0"));

            // string
            Assert.AreEqual(true, TypeComparer.SameValue("", ""));
            Assert.AreEqual(true, TypeComparer.SameValue("a", "a"));
            Assert.AreEqual(false, TypeComparer.SameValue("a", "b"));
            Assert.AreEqual(false, TypeComparer.SameValue("0", 0));

            // bool
            Assert.AreEqual(true, TypeComparer.SameValue(false, false));
            Assert.AreEqual(true, TypeComparer.SameValue(true, true));
            Assert.AreEqual(false, TypeComparer.SameValue(true, false));
            Assert.AreEqual(false, TypeComparer.SameValue(false, 0));

            // object
            var engine = new ScriptEngine();
            var temp1 = engine.Object.Construct();
            var temp2 = engine.Object.Construct();
            var number1 = engine.Number.Construct(5.0);
            Assert.AreEqual(true, TypeComparer.SameValue(temp1, temp1));
            Assert.AreEqual(false, TypeComparer.SameValue(temp1, temp2));
            Assert.AreEqual(true, TypeComparer.SameValue(number1, number1));
            Assert.AreEqual(false, TypeComparer.SameValue(number1, 5.0));
        }

    }
}
