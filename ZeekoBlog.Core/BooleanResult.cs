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
}
