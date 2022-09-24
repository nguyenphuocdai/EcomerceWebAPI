using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalClientListModel
    {
        public int GridPageSize { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalClient.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalClient.List.SearchDescription")]
        [AllowHtml]
        public string SearchDescription { get; set; }
    }
}