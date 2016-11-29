namespace ObscureWare.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DataTable<T>
    {
        private Dictionary<T, string[]> data = new Dictionary<T, string[]>();

        public string[] Header { get; internal set; }

        public void AddRow(T src, string[] rowValues)
        {
            this.data.Add(src, rowValues);
        }

        public IEnumerable<string[]> GetRows()
        {
            return this.data.Values;
        }

        /// <summary>
        /// Finds first value that is identified by value stored in the first column or NULL
        /// </summary>
        /// <param name="aIdentifier"></param>
        /// <returns></returns>
        public T GetUnderlyingValue(string aIdentifier)
        {
            return this.data.FirstOrDefault(pair => pair.Value.First().Equals(aIdentifier, StringComparison.InvariantCultureIgnoreCase)).Key;
        }

        /// <summary>
        /// Finds first value that matches given predicate filtering function or NULL
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="matchingFunc"></param>
        /// <returns></returns>
        public T FindValueWhere(string identifier, Func<T, string, bool> matchingFunc)
        {
            return this.data.FirstOrDefault(pair => matchingFunc(pair.Key, identifier)).Key;
        }
    }
}