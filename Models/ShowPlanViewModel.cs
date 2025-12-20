namespace fitnessCenter.Models
{
    public class ShowPlanViewModel
    {
        public string PlanText { get; set; }
        public string BeforeImageUrl { get; set; } // Base64 or URL
        public string AfterImageUrl { get; set; }
        public string PlanType { get; set; }
        public int Duration { get; set; }
    }
}
