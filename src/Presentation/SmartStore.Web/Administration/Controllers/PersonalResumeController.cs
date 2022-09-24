using System.Linq;
using System.Web.Mvc;
using SmartStore.Admin.Models.CurriculumVitae;
using SmartStore.Core.Domain.Common;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Domain.Media;
using SmartStore.Core.Events;
using SmartStore.Core.Logging;
using SmartStore.Core.Security;
using SmartStore.Services;
using SmartStore.Services.CurriculumVitae;
using SmartStore.Services.Media;
using SmartStore.Web.Framework.Controllers;
using SmartStore.Web.Framework.Security;
using Telerik.Web.Mvc;

namespace SmartStore.Admin.Controllers
{
    [AdminAuthorize]
    public class PersonalResumeController : AdminControllerBase
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPersonalClientService _personalClientService;
        private readonly ICommonServices _services;

        public PersonalResumeController(
            IPersonalClientService personalClientService,
            AdminAreaSettings adminAreaSettings,
            ICommonServices services,
            IEventPublisher eventPublisher,
            ICustomerActivityService customerActivityService,
            IMediaService mediaService,
            MediaSettings mediaSettings)
        {
            _personalClientService = personalClientService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        [Permission(Permissions.CurriculumVitae.PersonalClient.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalClientListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.PersonalClient.Read)]
        public ActionResult PersonalClientList(GridCommand command, PersonalClientListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalClientModel>();

            var q = new PersonalClientSearchQuery
            {
                Name = model.SearchName,
                Description = model.SearchDescription,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var personals = _personalClientService.SearchPersonalClients(q);

            gridModel.Data = personals.Select(PrepareClientModelForList);
            gridModel.Total = personals.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalClientModel PrepareClientModelForList(PersonalClient personalClient)
        {
            var model = new PersonalClientModel();
            model.Id = personalClient.Id;
            model.CustomerId = personalClient.CustomerId;
            model.ClientName = personalClient.ClientName;
            model.ClientDescription = personalClient.ClientDescription;
            model.ClientImageId = personalClient.ClientImageId;
            model.CreatedDate = personalClient.CreatedDate;
            model.ModifiedDate = personalClient.ModifiedDate;
            return model;
        }
    }
}