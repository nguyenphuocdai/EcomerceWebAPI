using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalTagListModel
    {
        public int GridPageSize { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalTag.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }

    }
}