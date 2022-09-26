using System.Collections.Generic;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Web.Framework.Modelling;
using SmartStore.Web.Models.Media;

namespace SmartStore.Web.Models.Catalog
{
    public partial class HomePagePersonalModel : EntityModelBase
    {
        public Personal PersonalModel { get; set; }
        public PictureModel PictureModel { get; set; } = new PictureModel();
    }
}