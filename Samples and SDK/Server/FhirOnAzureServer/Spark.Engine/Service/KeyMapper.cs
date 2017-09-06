#region Information

// Solution:  Spark
// Spark.Engine
// File:  KeyMapper.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:59 PM

#endregion

namespace FhirOnAzure.Service
{
    using System;
    using System.Collections.Generic;

    public class Mapper<TKEY, TVALUE>
    {
        private readonly Dictionary<TKEY, TVALUE> mapping = new Dictionary<TKEY, TVALUE>();

        public void Clear()
        {
            mapping.Clear();
        }

        public TVALUE TryGet(TKEY key)
        {
            TVALUE value;
            if (mapping.TryGetValue(key, out value))
                return value;
            return default(TVALUE);
        }

        public bool Exists(TKEY key)
        {
            foreach (var item in mapping)
                if (item.Key.Equals(key))
                    return true;
            return false;
        }

        public TVALUE Remap(TKEY key, TVALUE value)
        {
            if (Exists(key)) throw new Exception("Duplicate key");
            mapping.Add(key, value);
            return value;
        }

        public void Merge(Mapper<TKEY, TVALUE> mapper)
        {
            foreach (var keyValuePair in mapper.mapping)
                if (!Exists(keyValuePair.Key))
                    mapping.Add(keyValuePair.Key, keyValuePair.Value);
                else if (Exists(keyValuePair.Key) && TryGet(keyValuePair.Key).Equals(keyValuePair.Value) == false)
                    throw new InvalidOperationException("Incompatible mappings");
        }
    }
}