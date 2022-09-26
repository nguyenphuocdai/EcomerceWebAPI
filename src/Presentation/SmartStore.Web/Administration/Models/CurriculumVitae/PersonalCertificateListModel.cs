using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalCertificateListModel
    {
        [AllowHtml]
        public int CustomerId { get; set; }

        public int GridPageSize { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalClient.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }
    }
}