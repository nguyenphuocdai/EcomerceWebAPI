using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalListModel
    {
        public int GridPageSize { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.Personal.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.Personal.List.SearchPhoneNumber")]
        [AllowHtml]
        public string SearchPhoneNumber { get; set; }
    }
}