namespace BusinessObjects.Options;

public class WithdrawRulesSettings
{
    public int MinAmount { get; set; }
    public int MaxAmount { get; set; }
    public int LimitWithdraw { get; set; }
    public WithdrawStatuses Statuses { get; set; }
}