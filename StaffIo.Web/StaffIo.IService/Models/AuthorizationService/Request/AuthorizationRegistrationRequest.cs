using StaffIo.Data.Enums;

namespace StaffIo.IService.Models.AuthorizationService.Request
{
    /// <summary>
    /// модель запроса для регистрации аккаунта
    /// </summary>
    public class AuthorizationRegistrationRequest
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        public EnumUserRole UserRole { get; set; }  

        public static bool Validation(AuthorizationRegistrationRequest model)
        {
            if (model.UserRole == EnumUserRole.Owner)
            {
                if (model.MiddleName != null
                    && model.LastName != null)
                    throw new Exception("При регистрации пользователя с ролью Owner должен иметь только FirstName(в ФИО)");
            }else if (model.UserRole == EnumUserRole.Admin)
            {
                if (model.MiddleName == null
                    && model.LastName == null)
                    throw new Exception("При регистрации пользователя с ролью Admin должен иметь полное ФИО");
            }
            else
            {
                throw new Exception("Роль Employee не поддерживается");
            }

            return true;
        }
    }
}
