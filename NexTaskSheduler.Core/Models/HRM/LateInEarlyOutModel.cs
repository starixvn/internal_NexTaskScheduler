namespace NexTaskScheduler.Core.Models.HRM
{
    public class LateInEarlyOutModel
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public int BalanceLastYear { get; set; }

        public int BalanceCurrentYear { get; set; }

        public int TotalCurrentYear { get; set; }

        public int TotalTimesCurrentYear { get; set; }

        public int LeaveDeduction { get; set; }

        public int OvertimeBalance { get; set; }
    }
}