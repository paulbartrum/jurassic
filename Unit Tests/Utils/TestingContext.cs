using Jurassic;
using Jurassic.Library;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public sealed class TestingContext : ArrayInstance
    {
        public TestingContext(ScriptEngine engine)
            : base(engine.Array.InstancePrototype, new object[0])
        {

        }

        public void Clear()
        {
            while (Length > 0) Pop();
        }

        public IReadOnlyList<object> Results()
        {
            return TypeUtilities.Iterate(Engine, TypeUtilities.GetIterator(Engine, this)).ToList();
        }
    }
}
