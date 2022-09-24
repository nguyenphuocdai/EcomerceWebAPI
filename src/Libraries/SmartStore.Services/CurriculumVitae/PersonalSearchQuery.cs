namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalSearchQuery
    {
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
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