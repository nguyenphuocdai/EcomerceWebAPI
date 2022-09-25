namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalResumeSearchQuery
    {
        public string SearchCompany { get; set; }

        public int CustomerId { get; set; }
        /// <summary>
        /// Page index. Default: 0.
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Page index. Default: 50.
        /// </summary>
        public int PageSize { get; set; } = 50;
    }
}