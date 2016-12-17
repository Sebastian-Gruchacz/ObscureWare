namespace Obscureware.DataFlow.Implementation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class DataFlowToken : Hashtable
    {
        public Guid TokenId { get; private set; }

        public bool HasTerminated { get; set; }

        public DataFlowToken()
        {
            this.TokenId = Guid.NewGuid();
        }

        public DataFlowToken(DataFlowToken token)
        {
            foreach (var key in token.Keys)
            {
                base.Add(key, token[key]);
            }

            this.TokenId = Guid.NewGuid();
        }

        public virtual bool ContainsKey<T>(string key = null)
        {
            var keyValue = GetKey<T>(key);
            return this.ContainsKey(keyValue);
        }

        public virtual T Get<T>(string key = null)
        {
            var searchKey = GetKey<T>(key);
            var obj = this[searchKey];
            if (obj == null)
            {
                if (this.ContainsKey<T>(key))
                {
                    return default(T);
                }
                throw new InvalidOperationException($"No item with key {searchKey}");
            }
            var typedObj = (T)obj;
            if (typedObj == null)
            {
                throw new InvalidOperationException(
                    $"Object with key {searchKey} is of type {obj.GetType()}, and not of type {typeof(T)}");
            }

            return typedObj;
        }

        public virtual object Get(Type type, string key = null)
        {
            var searchKey = key ?? type.FullName;
            var obj = this[searchKey];
            if (obj == null)
            {
                if (this.ContainsKey(type, key))
                {
                    return null;
                }
                throw new InvalidOperationException($"No item with key {searchKey}");
            }

            if (obj.GetType() != type)
            {
                throw new InvalidOperationException(
                    $"Object with key {searchKey} is of type {obj.GetType()}, and not of type {type}");
            }

            return obj;
        }

        public virtual T GetDefault<T>(T defaultValue = default(T), string key = null)
        {
            if (this.ContainsKey<T>(key))
                return this.Get<T>(key);

            return defaultValue;
        }

        private bool ContainsKey(Type type, string key)
        {
            var keyValue = key ?? type.FullName;
            return this.ContainsKey(keyValue);
        }

        public virtual void Add<T>(T obj, string key = null)
        {
            var keyname = GetKey<T>(key);
            if (this[keyname] != null)
            {
                throw new InvalidOperationException($"Item with key {keyname} would be overwritten.");
            }

            base.Add(keyname, obj);
        }

        public virtual void Remove<T>(string key = null)
        {
            var keyname = GetKey<T>(key);
            base.Remove(keyname);
        }

        protected static string GetKey<T>(string key = null)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return key;
            }

            //Type typeInfo = typeof (T);
            //if (typeInfo.IsGenericType)
            //{
            //    return typeInfo.Name.Substring(0, typeInfo.Name.IndexOf("`"))
            //        + "<"
            //        + string.Join(",", typeInfo.GetGenericArguments().Select(a => a.FullName))
            //        + ">";
            //}

            // TODO: add more specific situations

            return typeof(T).FullName;//.ToString();
        }

        /// <summary>
        /// Get all stored key-values
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<object, object>> GetAll()
        {
            return
                from object key in this.Keys
                select new KeyValuePair<object, object>(key, this[key]);
        }
    }
}