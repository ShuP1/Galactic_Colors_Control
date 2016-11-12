namespace Galactic_Colors_Control_Common.Protocol
{
    public class RequestResult
    {
        public ResultTypes type;
        public string[] result;

        public RequestResult(ResultTypes p1, string[] p2 = null)
        {
            type = p1;
            result = p2;
        }
    }
}