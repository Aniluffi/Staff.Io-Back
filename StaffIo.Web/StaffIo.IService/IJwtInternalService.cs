namespace StaffIo.IService
{
    /// <summary>
    /// сервис для рабооты с jwt токенами
    /// </summary>
    public interface IJwtInternalService
    {
        /// <summary>
        /// метод для генерации токена
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GenerateToken(Guid userId);

        /// <summary>
        /// метод для получения названия токена в cookie
        /// </summary>
        /// <returns></returns>
        string GetNameToken();

        /// <summary>
        /// метод для получения времени жизни Jwt
        /// </summary>
        /// <returns></returns>
        double GetTimeInHoursLiveJwt();
    }
}
