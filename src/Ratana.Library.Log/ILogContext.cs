using System;
using System.Collections.Generic;
using System.Text;

namespace Ratana.Library.Log
{
    /// <summary>
    /// ILogContext is essentially a wrapper around a key/value dictionary.
    /// All key/values should get logged as part of each log entry.
    /// </summary>
    public interface ILogContext
    {
        void Add(string key, string value);

        bool ContainsKey(string key);

        bool Remove(string key);

        bool TryGetValue(string key, out string value);
    }
}
