using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalPortfolioListModel
    {
        [AllowHtml]
        public int CustomerId { get; set; }

        public int GridPageSize { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalPortfolio.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }
    }
}