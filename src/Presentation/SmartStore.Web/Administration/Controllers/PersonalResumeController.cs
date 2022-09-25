using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SmartStore.Admin.Models.CurriculumVitae;
using SmartStore.Core.Domain.Catalog;
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
    public class PersonalResumeController : AdminControllerBase
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPersonalResumeService _personalResumeService;
        private readonly ICommonServices _services;

        public PersonalResumeController(
            IPersonalResumeService personalResumeService,
            AdminAreaSettings adminAreaSettings,
            ICommonServices services,
            IEventPublisher eventPublisher,
            ICustomerActivityService customerActivityService,
            IMediaService mediaService,
            MediaSettings mediaSettings)
        {
            _personalResumeService = personalResumeService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        [Permission(Permissions.CurriculumVitae.PersonalResume.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalResumeListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.PersonalResume.Read)]
        public ActionResult PersonalResumeList(GridCommand command, PersonalResumeListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalResumeModel>();

            var q = new PersonalResumeSearchQuery
            {
                CustomerId = model.CustomerId,
                SearchCompany = model.SearchCompany,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var resumes = _personalResumeService.SearchResumes(q);

            gridModel.Data = resumes.Select(PrepareResumeModelForList);
            gridModel.Total = resumes.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalResumeModel PrepareResumeModelForList(PersonalResume resume)
        {
            var model = new PersonalResumeModel();
            model.Id = resume.Id;
            model.CustomerId = resume.CustomerId;
            model.ResumeType = int.Parse(resume.ResumeType);
            model.ResumeShort = resume.ResumeShort;
            model.Company = resume.Company;
            model.Address = resume.Address;
            model.Title = resume.Title;
            model.From = resume.From;
            model.To = resume.To;
            model.Description = resume.Description;
            model.CreatedDate = resume.CreatedDate;
            model.ModifiedDate = resume.ModifiedDate;
            return model;
        }


        [Permission(Permissions.CurriculumVitae.PersonalResume.Create)]
        public ActionResult Create()
        {
            var model = new PersonalResumeModel();
            PrepareResumeModel(model, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalResume.Create)]
        public ActionResult Create(PersonalResumeModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                PersonalResume resume = new PersonalResume();
                MapModelToClient(model, resume);

                _personalResumeService.InsertResume(resume);
                _customerActivityService.InsertActivity("AddNewResume", T("ActivityLog.AddNewResume"), resume.Company);
            }

            if (continueEditing)
            {
                return View(model);
            }

            NotifySuccess(T("Admin.Catalog.Resume.Added"));
            // If we got this far, something failed, redisplay form.
            PrepareResumeModel(model, null);
            return RedirectToAction("List");
        }

        [NonAction]
        protected void MapModelToClient(PersonalResumeModel model, PersonalResume resume)
        {
            var p = resume;
            var m = model;
            var currentTime = DateTime.Now;

            p.CustomerId = m.CustomerId;
            p.ResumeType = m.ResumeType.ToString();
            p.ResumeShort = m.ResumeShort;
            p.Company = m.Company;
            p.Address = m.Address;
            p.Title = m.Title;
            p.From = m.From;
            p.To = m.To;
            p.Description = m.Description;
            p.CreatedDate = currentTime;
            p.ModifiedDate = currentTime;
        }

        protected void PrepareResumeModel(PersonalResumeModel model, PersonalResume resume)
        {
            Guard.NotNull(model, nameof(model));
            Customer currentCustomer = _services.WorkContext.CurrentCustomer;
            if (resume == null)
            {
                model.CustomerId = currentCustomer.Id;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
            }
            else
            {
                model.CustomerId = resume.CustomerId;
                model.ResumeType = int.Parse(resume.ResumeType);
                model.ResumeShort = resume.ResumeShort;
                model.Company = resume.Company;
                model.Address = resume.Address;
                model.Title = resume.Title;
                model.From = resume.From;
                model.To = resume.To;
                model.Description = resume.Description;
                model.CreatedDate = resume.CreatedDate;
                model.ModifiedDate = resume.ModifiedDate;
            }
        }

        [Permission(Permissions.CurriculumVitae.PersonalResume.Read)]
        public ActionResult Edit(int id)
        {
            var resume = _personalResumeService.GetResumeById(id);
            if (resume == null)
            {
                NotifyWarning(T("Resume.NotFound", id));
                return RedirectToAction("List");
            }

            var model = resume.ToModel();
            PrepareResumeModel(model, resume);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalResume.Update)]
        public ActionResult Edit(PersonalResumeModel model, bool continueEditing)
        {
            var resume = _personalResumeService.GetResumeById(model.Id);
            if (resume == null)
            {
                NotifyWarning(T("Resume.NotFound", model.Id));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                resume.ResumeType = model.ResumeType.ToString();
                resume.ResumeShort = model.ResumeShort;
                resume.Company = model.Company;
                resume.Address = model.Address;
                resume.Title = model.Title;
                resume.From = model.From;
                resume.To = model.To;
                resume.Description = model.Description;
                resume.ModifiedDate = DateTime.Now;

                _personalResumeService.UpdateResume(resume);

                _customerActivityService.InsertActivity("EditResume", T("ActivityLog.EditResume"), resume.Company);

                NotifySuccess(T("Admin.Catalog.Resume.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = resume.Id }) : RedirectToAction("List");
            }

            // If we got this far, something failed, redisplay form.
            PrepareResumeModel(model, resume);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalResume.Delete)]
        public ActionResult Delete(int id)
        {
            var resume = _personalResumeService.GetResumeById(id);
            _personalResumeService.DeleteResume(resume);

            _customerActivityService.InsertActivity("DeleteProduct", T("ActivityLog.DeleteSkill"), resume.Company);

            NotifySuccess(T("Admin.Catalog.Resume.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalResume.Delete)]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    _personalResumeService.DeleteResume(id);
                }
            }

            NotifySuccess(T("Admin.Common.TaskSuccessfullyProcessed"));

            return Json(new { success = true });
        }
    }
}