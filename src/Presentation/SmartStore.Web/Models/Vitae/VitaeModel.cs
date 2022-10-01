using System.Collections.Generic;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Web.Framework.Modelling;
using SmartStore.Web.Models.Media;

namespace SmartStore.Web.Models.Catalog
{
    public partial class VitaeModel : ModelBase
    {
        public VitaeModel()
        {
            PictureModel = new PictureModel();
            PersonalSkills = new List<PersonalSkill>();
            PersonalClients = new List<WebPersonalClientModel>();
        }

        public Personal Personal { get; set; }
        public List<PersonalSkill> PersonalSkills { get; set; }
        public List<WebPersonalClientModel> PersonalClients { get; set; }
        public PictureModel PictureModel { get; set; }
        public bool IsExists { get; set; }
    }

    public partial class WebPersonalClientModel
    {
        public PersonalClient Client { get; set; }
        public PictureModel ClientPicture { get; set; }
    }

    public partial class VitaeMenuBarModel : ModelBase
    {
        public int CustomerID { get; set; }
    }
}