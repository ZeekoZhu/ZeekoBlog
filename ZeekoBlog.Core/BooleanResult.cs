namespace ZeekoBlog.Core
{
    public class BooleanResult
    {
        public bool Success { get; }
        public string Msg { get; }

        public BooleanResult(bool success, string msg = "")
        {
            Success = success;
            Msg = msg;
        }

        public BooleanResult(BooleanResult other, bool? success = null, string msg = null)
        {
            Success = success ?? other.Success;
            Msg = msg ?? other.Msg;
        }
    }

    public class BooleanResult<T> : BooleanResult
    {
        public T Value { get; set; }
        public BooleanResult(bool success, string msg = "") : base(success, msg)
        {
            Value = default(T);
        }

        public BooleanResult(bool success, T value = default(T), string msg = "") : base(success, msg)
        {
            Value = value;
        }

        public BooleanResult(BooleanResult other, bool? success = null, string msg = null) : base(other, success, msg)
        {
            Value = default(T);
        }
    }

}
