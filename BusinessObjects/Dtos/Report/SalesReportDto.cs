namespace BusinessObjects.Dtos.Report;

public class SalesReportDto
{
    public int TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProductsSold { get; set; }

    public List<TimelineRevenueDto> ByDay { get; set; }
    public List<TimelineRevenueDto> ByMonth { get; set; }
    public List<TimelineRevenueDto> ByYear { get; set; }

    public List<TopProductDto> TopProducts { get; set; }
}