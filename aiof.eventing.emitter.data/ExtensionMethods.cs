using System;
using System.Collections.Generic;
using System.Text;

namespace aiof.eventing.emitter.data
{
    public static class ExtensionMethods
    {
        public static IEnumerable<object[]> ToObjectArray<T>(this IEnumerable<T> entities) 
            where T : class
        {
            var entitiesObjectArray = new List<object[]>();

            foreach (var entity in entities)
                entitiesObjectArray.Add(new object[] { entity });

            return entitiesObjectArray;
        }
    }
}
