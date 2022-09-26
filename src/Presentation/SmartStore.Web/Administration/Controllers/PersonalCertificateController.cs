using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SmartStore.Admin.Models.CurriculumVitae;
using SmartStore.Core.Domain.Common;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Domain.Customers;
using SmartStore.Core.Domain.Media;
using SmartStore.Core.Events;
using SmartStore.Core.Logging;
using SmartStore.Core.Security;
using SmartStore.Services;
using SmartStore.Services.CurriculumVitae;
using SmartStore.Services.Media;
using SmartStore.Web.Framework.Controllers;
using SmartStore.Web.Framework.Filters;
using SmartStore.Web.Framework.Security;
using Telerik.Web.Mvc;

namespace SmartStore.Admin.Controllers
{
    [AdminAuthorize]
    public class PersonalCertificateController : AdminControllerBase
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPersonalCertificateService _personalCertificateService;
        private readonly ICommonServices _services;

        public PersonalCertificateController(
            IPersonalCertificateService personalCertificateService,
            AdminAreaSettings adminAreaSettings,
            ICommonServices services,
            ICustomerActivityService customerActivityService,
            IMediaService mediaService,
            MediaSettings mediaSettings)
        {
            _personalCertificateService = personalCertificateService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        [Permission(Permissions.CurriculumVitae.PersonalCertificate.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalCertificateListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.PersonalCertificate.Read)]
        public ActionResult PersonalCertificateList(GridCommand command, PersonalCertificateListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalCertificateModel>();

            var q = new PersonalCertificateSearchQuery()
            {
                Name = model.SearchName,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var personals = _personalCertificateService.SearchCertificates(q);

            gridModel.Data = personals.Select(PrepareCertificateModelForList);
            gridModel.Total = personals.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalCertificateModel PrepareCertificateModelForList(PersonalCertificate certificate)
        {
            var model = new PersonalCertificateModel();
            model.Id = certificate.Id;
            model.CustomerId = certificate.CustomerId;
            model.Name = certificate.Name;
            model.Description = certificate.Description;
            model.CertificateDate = certificate.CertificateDate;
            model.ImageId = certificate.ImageId;
            model.CreatedDate = certificate.CreatedDate;
            model.ModifiedDate = certificate.ModifiedDate;

            // Media files.
            var file = _mediaService.GetFileById(certificate.ImageId ?? 0);
            model.PictureThumbnailUrl = _mediaService.GetUrl(file, _mediaSettings.CartThumbPictureSize);
            model.NoThumb = file == null;
            return model;
        }

        [Permission(Permissions.CurriculumVitae.PersonalCertificate.Create)]
        public ActionResult Create()
        {
            var model = new PersonalCertificateModel();
            PrepareCertificateModel(model, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalCertificate.Create)]
        public ActionResult Create(PersonalCertificateModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                PersonalCertificate certificate = new PersonalCertificate();
                MapModelToClient(model, certificate);

                _personalCertificateService.InsertCertificate(certificate);
                _customerActivityService.InsertActivity("AddNewCertificate", T("ActivityLog.AddNewCertificate"), certificate.Name);
            }

            if (continueEditing)
            {
                return View(model);
            }

            NotifySuccess(T("Admin.Catalog.Certificate.Added"));
            // If we got this far, something failed, redisplay form.
            PrepareCertificateModel(model, null);
            return RedirectToAction("List");
        }

        [NonAction]
        protected void MapModelToClient(PersonalCertificateModel model, PersonalCertificate certificate)
        {
            var p = certificate;
            var m = model;
            var currentTime = DateTime.Now;
            p.CustomerId = m.CustomerId;
            p.Name = m.Name;
            p.Description = m.Description;
            p.CertificateDate = m.CertificateDate;
            p.ImageId = m.ImageId;
            p.CreatedDate = currentTime;
            p.ModifiedDate = currentTime;
        }

        protected void PrepareCertificateModel(PersonalCertificateModel model, PersonalCertificate certificate)
        {
            Guard.NotNull(model, nameof(model));
            Customer currentCustomer = _services.WorkContext.CurrentCustomer;
            if (certificate == null)
            {
                model.CustomerId = currentCustomer.Id;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
            }
            else
            {
                model.CustomerId = certificate.CustomerId;
                model.Name = certificate.Name;
                model.Description = certificate.Description;
                model.CertificateDate = certificate.CertificateDate;
                model.ImageId = certificate.ImageId;
                model.CreatedDate = certificate.CreatedDate;
                model.ModifiedDate = certificate.ModifiedDate;
            }
        }

        [Permission(Permissions.CurriculumVitae.PersonalCertificate.Read)]
        public ActionResult Edit(int id)
        {
            var certificate = _personalCertificateService.GetCertificateById(id);
            if (certificate == null)
            {
                NotifyWarning(T("Certificate.NotFound", id));
                return RedirectToAction("List");
            }

            var model = certificate.ToModel();
            PrepareCertificateModel(model, certificate);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalCertificate.Update)]
        public ActionResult Edit(PersonalCertificateModel model, bool continueEditing)
        {
            var certificate = _personalCertificateService.GetCertificateById(model.Id);
            if (certificate == null)
            {
                NotifyWarning(T("Certificate.NotFound", model.Id));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                certificate.Name = model.Name;
                certificate.Description = model.Description;
                certificate.CertificateDate = model.CertificateDate;
                certificate.ImageId = model.ImageId;
                certificate.ModifiedDate = DateTime.Now;

                _personalCertificateService.UpdateCertificate(certificate);

                _customerActivityService.InsertActivity("EditCertificate", T("ActivityLog.EditCertificate"), certificate.Name);

                NotifySuccess(T("Admin.Catalog.Certificate.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = certificate.Id }) : RedirectToAction("List");
            }

            // If we got this far, something failed, redisplay form.
            PrepareCertificateModel(model, certificate);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalClient.Delete)]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    _personalCertificateService.DeleteCertificate(id);
                }
            }

            NotifySuccess(T("Admin.Common.TaskSuccessfullyProcessed"));

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalClient.Delete)]
        public ActionResult Delete(int id)
        {
            var certificate = _personalCertificateService.GetCertificateById(id);
            _personalCertificateService.DeleteCertificate(certificate);

            _customerActivityService.InsertActivity("DeleteCertificate", T("ActivityLog.DeleteCertificate"), certificate.Name);

            NotifySuccess(T("Admin.Catalog.Certificate.Deleted"));
            return RedirectToAction("List");
        }
    }
}