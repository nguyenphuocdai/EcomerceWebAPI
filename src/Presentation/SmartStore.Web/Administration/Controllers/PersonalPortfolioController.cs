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
    public class PersonalPortfolioController : AdminControllerBase
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPersonalPortfolioService _personalPortfolioService;
        private readonly ICommonServices _services;

        public PersonalPortfolioController(
            IPersonalPortfolioService personalPortfolioService,
            AdminAreaSettings adminAreaSettings,
            ICommonServices services,
            IEventPublisher eventPublisher,
            ICustomerActivityService customerActivityService,
            IMediaService mediaService,
            MediaSettings mediaSettings)
        {
            _personalPortfolioService = personalPortfolioService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalPortfolioListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Read)]
        public ActionResult PersonalPortfolioList(GridCommand command, PersonalPortfolioListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalPortfolioModel>();

            var q = new PersonalPortfolioSearchQuery
            {
                CustomerId = model.CustomerId,
                SearchName = model.SearchName,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var resumes = _personalPortfolioService.SearchPortfolios(q);

            gridModel.Data = resumes.Select(PreparePortfolioModelForList);
            gridModel.Total = resumes.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalPortfolioModel PreparePortfolioModelForList(PersonalPortfolio portfolio)
        {
            var model = new PersonalPortfolioModel();
            var p = portfolio;
            model.Id = p.Id;
            model.CustomerId = p.CustomerId;
            model.PortfolioName = p.PortfolioName;
            model.PortfolioUrl = p.PortfolioUrl;
            model.ShortMessage = p.ShortMessage;
            model.From = p.From;
            model.To = p.To;
            model.Published = p.Published;
            model.Status = int.Parse(p.Status);
            model.Description = p.Description;
            model.Tag = p.Tag;
            model.ShareLinkFacebook = p.ShareLinkFacebook;
            model.ShareLinkInstagram = p.ShareLinkInstagram;
            model.ShareLinkTwitter = p.ShareLinkTwitter;
            model.CreatedDate = p.CreatedDate;
            model.ModifiedDate = p.ModifiedDate;
            return model;
        }


        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Create)]
        public ActionResult Create()
        {
            var model = new PersonalPortfolioModel();
            PreparePortfolioModel(model, null);

            return View(model);
        }

        //[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        //[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //[Permission(Permissions.CurriculumVitae.PersonalResume.Create)]
        //public ActionResult Create(PersonalResumeModel model, bool continueEditing)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        PersonalResume portfolio = new PersonalResume();
        //        MapModelToClient(model, portfolio);

        //        _personalResumeService.InsertResume(portfolio);
        //        _customerActivityService.InsertActivity("AddNewResume", T("ActivityLog.AddNewResume"), portfolio.Company);
        //    }

        //    if (continueEditing)
        //    {
        //        return View(model);
        //    }

        //    NotifySuccess(T("Admin.Catalog.Resume.Added"));
        //    // If we got this far, something failed, redisplay form.
        //    PreparePortfolioModel(model, null);
        //    return RedirectToAction("List");
        //}

        //[NonAction]
        //protected void MapModelToClient(PersonalResumeModel model, PersonalResume portfolio)
        //{
        //    var p = portfolio;
        //    var m = model;
        //    var currentTime = DateTime.Now;

        //    p.CustomerId = m.CustomerId;
        //    p.ResumeType = m.ResumeType.ToString();
        //    p.ResumeShort = m.ResumeShort;
        //    p.Company = m.Company;
        //    p.Address = m.Address;
        //    p.Title = m.Title;
        //    p.From = m.From;
        //    p.To = m.To;
        //    p.Description = m.Description;
        //    p.CreatedDate = currentTime;
        //    p.ModifiedDate = currentTime;
        //}

        protected void PreparePortfolioModel(PersonalPortfolioModel model, PersonalPortfolio portfolio)
        {
            Guard.NotNull(model, nameof(model));
            Customer currentCustomer = _services.WorkContext.CurrentCustomer;
            if (portfolio == null)
            {
                model.CustomerId = currentCustomer.Id;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
            }
            else
            {
                model.CustomerId = portfolio.CustomerId;
                model.PortfolioName = portfolio.PortfolioName;
                model.PortfolioUrl = portfolio.PortfolioUrl;
                model.ShortMessage = portfolio.ShortMessage;
                model.From = portfolio.From;
                model.To = portfolio.To;
                model.Published = portfolio.Published;
                model.Status = int.Parse(portfolio.Status);
                model.Description = portfolio.Description;
                model.Tag = portfolio.Tag;
                model.ShareLinkFacebook = portfolio.ShareLinkFacebook;
                model.ShareLinkInstagram = portfolio.ShareLinkInstagram;
                model.ShareLinkTwitter = portfolio.ShareLinkTwitter;

                model.CreatedDate = portfolio.CreatedDate;
                model.ModifiedDate = portfolio.ModifiedDate;
            }
        }

        //[Permission(Permissions.CurriculumVitae.PersonalResume.Read)]
        //public ActionResult Edit(int id)
        //{
        //    var portfolio = _personalResumeService.GetResumeById(id);
        //    if (portfolio == null)
        //    {
        //        NotifyWarning(T("Resume.NotFound", id));
        //        return RedirectToAction("List");
        //    }

        //    var model = portfolio.ToModel();
        //    PreparePortfolioModel(model, portfolio);

        //    return View(model);
        //}

        //[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        //[ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        //[Permission(Permissions.CurriculumVitae.PersonalResume.Update)]
        //public ActionResult Edit(PersonalResumeModel model, bool continueEditing)
        //{
        //    var portfolio = _personalResumeService.GetResumeById(model.Id);
        //    if (portfolio == null)
        //    {
        //        NotifyWarning(T("Resume.NotFound", model.Id));
        //        return RedirectToAction("List");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        portfolio.ResumeType = model.ResumeType.ToString();
        //        portfolio.ResumeShort = model.ResumeShort;
        //        portfolio.Company = model.Company;
        //        portfolio.Address = model.Address;
        //        portfolio.Title = model.Title;
        //        portfolio.From = model.From;
        //        portfolio.To = model.To;
        //        portfolio.Description = model.Description;
        //        portfolio.ModifiedDate = DateTime.Now;

        //        _personalResumeService.UpdateResume(portfolio);

        //        _customerActivityService.InsertActivity("EditResume", T("ActivityLog.EditResume"), portfolio.Company);

        //        NotifySuccess(T("Admin.Catalog.Resume.Updated"));
        //        return continueEditing ? RedirectToAction("Edit", new { id = portfolio.Id }) : RedirectToAction("List");
        //    }

        //    // If we got this far, something failed, redisplay form.
        //    PreparePortfolioModel(model, portfolio);

        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Permission(Permissions.CurriculumVitae.PersonalResume.Delete)]
        //public ActionResult Delete(int id)
        //{
        //    var portfolio = _personalResumeService.GetResumeById(id);
        //    _personalResumeService.DeleteResume(portfolio);

        //    _customerActivityService.InsertActivity("DeleteProduct", T("ActivityLog.DeleteSkill"), portfolio.Company);

        //    NotifySuccess(T("Admin.Catalog.Resume.Deleted"));
        //    return RedirectToAction("List");
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Permission(Permissions.CurriculumVitae.PersonalResume.Delete)]
        //public ActionResult DeleteSelected(ICollection<int> selectedIds)
        //{
        //    if (selectedIds != null)
        //    {
        //        foreach (var id in selectedIds)
        //        {
        //            _personalResumeService.DeleteResume(id);
        //        }
        //    }

        //    NotifySuccess(T("Admin.Common.TaskSuccessfullyProcessed"));

        //    return Json(new { success = true });
        //}
    }
}