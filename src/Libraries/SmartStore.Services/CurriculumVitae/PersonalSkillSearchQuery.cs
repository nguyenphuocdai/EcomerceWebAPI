namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalSkillSearchQuery
    {
        public int CustomerId { get; set; }

        public string SkillName { get; set; }
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