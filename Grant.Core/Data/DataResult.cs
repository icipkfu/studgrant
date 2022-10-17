namespace Grant.Core
{
    public class DataResult
    {
        public bool Success;

        public object Data;

        public string Message;


        public static DataResult Failure(string msg)
        {
            return new DataResult
            {
                Success = false,
                Message = msg
            };
        }

        public static DataResult Ok()
        {
            return new DataResult
            {
                Success = true
            };
        }

        public static DataResult Ok(object data)
        {
            return new DataResult
            {
                Success = true,
                Data = data
            };
        }
    }
}