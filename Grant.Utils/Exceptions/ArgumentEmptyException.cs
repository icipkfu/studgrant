namespace Grant.Utils.Exceptions
{
    public class ArgumentEmptyException : GrantException
    {
        public ArgumentEmptyException(string parameterName) : base(string.Format("Parameter {0} cannot be empty", parameterName))
        {
            
        }
    }
}