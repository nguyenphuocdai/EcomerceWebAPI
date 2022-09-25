using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalResumeListModel
    {
        [AllowHtml]
        public int CustomerId { get; set; }

        public int GridPageSize { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalResume.List.SearchCompany")]
        [AllowHtml]
        public string SearchCompany { get; set; }
    }
}