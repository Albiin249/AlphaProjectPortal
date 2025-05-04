namespace AlphaPortal.Models;

public class ProjectViewModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProjectImage { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string ClientName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal? Budget {  get; set; }
    public string? ClientId { get; set; }
    public string? StatusName { get; set; } 
    public int StatusId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public IEnumerable<MemberImageViewModel> Members { get; set; } = new List<MemberImageViewModel>();

    //Tog hjälp från ChatGPT för att skriva detta kodsnipp, för att få ut hur lång tid det är kvar tills deadline.
    public string TimeLeft
    {
        get
        {
            if (EndDate == null)
                return "No end date.";

            var timeLeft = EndDate.Value - DateTime.Now;

            if (timeLeft.TotalSeconds < 0)
                return "Project is finished";

            if (timeLeft.TotalDays > 7)
            {
                var weeksLeft = (int)(timeLeft.TotalDays / 7);
                return $"{weeksLeft} week{(weeksLeft > 1 ? "s" : "")} left";
            }

            return $"{timeLeft.Days} day{(timeLeft.Days > 1 ? "s" : "")} left"; //Tog hjälp utav ChatGPT för att få ut skriften days eller day beroende på om det är mer än en dag eller inte. Samma på weeks.
        }
    }
    public bool IsNearDeadline
    {
        get
        {
            if (EndDate == null)
                return false;

            var timeLeft = EndDate.Value - DateTime.Now;

            return timeLeft.TotalDays <= 7 && timeLeft.TotalSeconds > 0; 
        }
    }
}
