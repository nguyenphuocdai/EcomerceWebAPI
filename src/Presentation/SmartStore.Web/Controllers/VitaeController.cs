using System.Collections.Generic;
using System.Web.Mvc;
using SmartStore.Core.Domain.Media;
using SmartStore.Services.CurriculumVitae;
using SmartStore.Services.Localization;
using SmartStore.Services.Media;
using SmartStore.Web.Framework.Controllers;
using SmartStore.Web.Framework.Seo;
using SmartStore.Web.Models.Catalog;
using SmartStore.Web.Models.Media;

namespace SmartStore.Web.Controllers
{
    public partial class VitaeController : PublicControllerBase
    {
        private readonly IPersonalService _personalService;
        private readonly IPersonalSkillService _personalSkillService;
        private readonly IPersonalClientService _personalClientService;
        private readonly MediaSettings _mediaSettings;
        private readonly IMediaService _mediaService;
        public VitaeController(IPersonalService personalService, MediaSettings mediaSettings, IMediaService mediaService, IPersonalSkillService personalSkillService, IPersonalClientService personalClientService)
        {
            _personalService = personalService;
            _mediaSettings = mediaSettings;
            _mediaService = mediaService;
            _personalSkillService = personalSkillService;
            _personalClientService = personalClientService;
        }

        [RewriteUrl(SslRequirement.No)]
        public ActionResult Index(int customerId)
        {
            var vitae = VitaeModel(customerId);
            return View(vitae);
        }

        [RewriteUrl(SslRequirement.No)]
        public ActionResult Resume(int customerId)
        {
            var vitae = VitaeModel(customerId);
            return View(vitae);
        }
        private VitaeModel VitaeModel(int customerId)
        {
            var person = _personalService.GetPersonalByCustomerId(customerId);
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            VitaeModel vitae = new VitaeModel();
            vitae.IsExists = person != null;
            vitae.Personal = person;

            // skills
            int.TryParse(person.Avatar, out int imageID);
            var file = _mediaService.GetFileById(imageID);
            vitae.PictureModel = new PictureModel();
            vitae.PictureModel.PictureId = imageID;
            vitae.PictureModel.Size = pictureSize;
            vitae.PictureModel.ImageUrl = _mediaService.GetUrl(file, pictureSize, null);
            vitae.PictureModel.FullSizeImageUrl = _mediaService.GetUrl(file, 0, null, false);
            vitae.PictureModel.FullSizeImageWidth = file?.Dimensions.Width;
            vitae.PictureModel.FullSizeImageHeight = file?.Dimensions.Height;
            vitae.PictureModel.Title = file?.File?.GetLocalized(x => x.Title)?.Value.NullEmpty() ??
                                       string.Format(T("Media.Vitae.ImageLinkTitleFormat"), person.FullName);
            vitae.PictureModel.AlternateText = file?.File?.GetLocalized(x => x.Alt)?.Value.NullEmpty() ??
                                               string.Format(T("Media.Vitae.ImageLinkTitleFormat"), person.FullName);
            vitae.PictureModel.File = file;

            vitae.PersonalSkills = _personalSkillService.GetSkillByCustomerId(customerId);
            // clients
            var clients = _personalClientService.GetClientByCustomerId(customerId);
            List<WebPersonalClientModel> listClients = new List<WebPersonalClientModel>();
            foreach (var client in clients)
            {
                WebPersonalClientModel item = new WebPersonalClientModel();
                item.Client = client;

                var clientFile = _mediaService.GetFileById(client.ClientImageId);
                item.ClientPicture = new PictureModel();
                item.ClientPicture.PictureId = client.ClientImageId;
                item.ClientPicture.Size = pictureSize;
                item.ClientPicture.ImageUrl = _mediaService.GetUrl(clientFile, pictureSize, null);
                item.ClientPicture.FullSizeImageUrl = _mediaService.GetUrl(clientFile, 0, null, false);
                item.ClientPicture.FullSizeImageWidth = clientFile?.Dimensions.Width;
                item.ClientPicture.FullSizeImageHeight = clientFile?.Dimensions.Height;
                item.ClientPicture.Title = clientFile?.File?.GetLocalized(x => x.Title)?.Value.NullEmpty() ??
                                           string.Format(T("Media.Vitae.ImageLinkTitleFormat"), client.ClientName);
                item.ClientPicture.AlternateText = clientFile?.File?.GetLocalized(x => x.Alt)?.Value.NullEmpty() ??
                                                   string.Format(T("Media.Vitae.ImageLinkTitleFormat"), client.ClientName);
                item.ClientPicture.File = clientFile;

                listClients.Add(item);
            }

            vitae.PersonalClients = listClients;
            return vitae;
        }

        [ChildActionOnly]
        public ActionResult MenuBar(int customerId)
        {
            VitaeMenuBarModel model = new VitaeMenuBarModel();
            model.CustomerID = customerId;
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return PartialView();
        }
    }
}
