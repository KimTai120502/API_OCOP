namespace Sale.Models.Response
{
    public class ResponseModel
    {
        public int status { get; set; }
        public string code { get; set; }
        public string message { get; set; } = string.Empty;
        public string exception { get; set; } = string.Empty;
        public object data { get; set; }
    }
}
