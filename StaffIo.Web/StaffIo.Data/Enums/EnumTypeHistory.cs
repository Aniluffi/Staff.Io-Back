namespace StaffIo.Data.Enums
{
    /// <summary>
    /// Тип истории изменений сотрудника
    /// </summary>
    public enum EnumTypeHistory
    {
        /// <summary>
        /// Изменение зарплаты
        /// </summary>
        ChangeSalary = 1,
        /// <summary>
        /// Изменение фотографии профиля
        /// </summary>
        ChangeFotoProfile = 2,
        /// <summary>
        /// Изменение фамилии
        /// </summary>
        ChangeFirstName = 3,
        /// <summary>
        /// Изменение имени
        /// </summary>
        ChangeMiddleName = 4,
        /// <summary>
        /// Изменение отчества
        /// </summary>
        ChangeLastName = 5,
        /// <summary>
        /// Изменение должности
        /// </summary>
        ChangePosition = 6,
        /// <summary>
        /// Изменение отдела
        /// </summary>
        ChangeDepartment = 7, // ← смотри ниже
        /// <summary>
        /// Изменение рабочего графика
        /// </summary>
        ChangeWorkPlan = 8,
        /// <summary>
        /// Изменение фотографий документов
        /// </summary>
        ChangeFotoDocuments = 9,
        /// <summary>
        /// Изменение статуса сотрудника
        /// </summary>
        ChangeStatus = 10
    }
}
