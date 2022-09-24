using System.Web.Mvc;
using SmartStore.Web.Framework;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    public class PersonalSkillListModel
    {
        public int GridPageSize { get; set; }

        [AllowHtml]
        public int CustomerId { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.PersonalSkill.List.SearchName")]
        [AllowHtml]
        public string SkillName { get; set; }
    }
}