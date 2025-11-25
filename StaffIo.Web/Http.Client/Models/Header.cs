namespace Http.Client.Models
{
    /// <summary>
    /// заголовок http запроса
    /// </summary>
    public class Header
    {
        /// <summary>
        /// тип авторизации
        /// </summary>
        public string TypeAuth {  get; set; }
        /// <summary>
        /// тоекн авторизации
        /// </summary>
        public string AccessToken { get; set; }
        public string MediaType { get; set; }
    }
}
