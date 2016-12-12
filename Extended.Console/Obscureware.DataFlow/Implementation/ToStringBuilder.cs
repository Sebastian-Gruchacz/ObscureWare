namespace Obscureware.DataFlow.Implementation
{
    using System.Text;

    public class ToStringBuilder<T>
    {
        private readonly StringBuilder _builder = new StringBuilder();

        private bool _isFirst = true;

        public static ToStringBuilder<T> Create()
        {
            return new ToStringBuilder<T>();
        }

        public ToStringBuilder<T> Append(string name, object value)
        {
            this._builder.AppendFormat("{0}{1}={2}", this._isFirst ? string.Empty : ";", name, value);
            this._isFirst = false;
            return this;
        }

        public override string ToString()
        {
            return string.Format("{0}{{{1}}}", typeof(T).Name, this._builder);
        }
    }
}