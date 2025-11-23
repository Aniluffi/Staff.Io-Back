namespace StaffIo.Data.Enums
{
    /// <summary>
    /// Рабочий график
    /// </summary>
    public enum EnumWorkPlan
    {
        /// <summary>
        /// 5/2 — пятидневная рабочая неделя
        /// </summary>
        FiveTwo = 1,

        /// <summary>
        /// 2/2 — сменный график "два через два"
        /// </summary>
        TwoTwo = 2,

        /// <summary>
        /// 3/3 — сменный график "три через три"
        /// </summary>
        ThreeThree = 3,

        /// <summary>
        /// 1/1 — один рабочий, один выходной
        /// </summary>
        OneOne = 4,

        /// <summary>
        /// 6/1 — шестидневная рабочая неделя
        /// </summary>
        SixOne = 5,

        /// <summary>
        /// 4/2 — четыре рабочих, два выходных
        /// </summary>
        FourTwo = 6,

        /// <summary>
        /// 4/3 — четыре рабочих, три выходных
        /// </summary>
        FourThree = 7,

        /// <summary>
        /// 3/1 — три рабочих, один выходной
        /// </summary>
        ThreeOne = 8
    }

}
