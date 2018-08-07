namespace NexTaskScheduler.Core.Models.HRM
{
    public class LateInEarlyOutModel
    {
        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string ConversationId { get; set; }

        public int BalanceLastYear { get; set; }

        public int BalanceCurrentYear { get; set; }

        public int Jan { get; set; }

        public int Feb { get; set; }

        public int Mar { get; set; }

        public int Apr { get; set; }

        public int May { get; set; }

        public int Jun { get; set; }

        public int Jul { get; set; }

        public int Aug { get; set; }

        public int Sep { get; set; }

        public int Oct { get; set; }

        public int Nov { get; set; }

        public int Dec { get; set; }

        public int TimesInJan { get; set; }

        public int TimesInFeb { get; set; }

        public int TimesInMar { get; set; }

        public int TimesInApr { get; set; }

        public int TimesInMay { get; set; }

        public int TimesInJun { get; set; }

        public int TimesInJul { get; set; }

        public int TimesInAug { get; set; }

        public int TimesInSep { get; set; }

        public int TimesInOct { get; set; }

        public int TimesInNov { get; set; }

        public int TimesInDec { get; set; }

        public int TotalCurrentYear { get; set; }

        public int TotalTimesCurrentYear { get; set; }

        public int LeaveDeduction { get; set; }

        public int OvertimeBalance { get; set; }
    }
}