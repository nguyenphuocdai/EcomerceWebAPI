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
        private readonly IPersonalTagService _personalTagService;
        private readonly ICommonServices _services;

        public PersonalPortfolioController(
            IPersonalPortfolioService personalPortfolioService,
            AdminAreaSettings adminAreaSettings,
            ICommonServices services,
            IEventPublisher eventPublisher,
            ICustomerActivityService customerActivityService,
            IMediaService mediaService,
            MediaSettings mediaSettings,
            IPersonalTagService personalTagService)
        {
            _personalPortfolioService = personalPortfolioService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
            _personalTagService = personalTagService;
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
            //model.Tag = p.Tag;
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

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Create)]
        public ActionResult Create(PersonalPortfolioModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                PersonalPortfolio portfolio = new PersonalPortfolio();
                MapModelToClient(model, portfolio);

                _personalPortfolioService.InsertPortfolio(portfolio);
                _customerActivityService.InsertActivity("AddNewResume", T("ActivityLog.AddNewResume"), portfolio.PortfolioName);
            }

            if (continueEditing)
            {
                PreparePortfolioModel(model, null);
                return View(model);
            }

            NotifySuccess(T("Admin.Catalog.Resume.Added"));
            // If we got this far, something failed, redisplay form.
            PreparePortfolioModel(model, null);
            return RedirectToAction("List");
        }

        [NonAction]
        protected void MapModelToClient(PersonalPortfolioModel model, PersonalPortfolio portfolio)
        {
            var p = portfolio;
            var m = model;
            var currentTime = DateTime.Now;

            p.CustomerId = m.CustomerId;
            p.PortfolioName = m.PortfolioName;
            p.PortfolioUrl = m.PortfolioUrl;
            p.ShortMessage = m.ShortMessage;
            p.From = m.From;
            p.To = m.To;
            p.Published = m.Published;
            p.Status = m.Status.ToString();
            p.Description = m.Description;
            if (m.Tag != null)
            {
                p.Tag = string.Join(",", m.Tag);
            }
            p.ShareLinkFacebook = m.ShareLinkFacebook;
            p.ShareLinkInstagram = m.ShareLinkInstagram;
            p.ShareLinkTwitter = m.ShareLinkTwitter;

            p.CreatedDate = currentTime;
            p.ModifiedDate = currentTime;
        }

        protected void PreparePortfolioModel(PersonalPortfolioModel model, PersonalPortfolio portfolio)
        {
            Guard.NotNull(model, nameof(model));
            Customer currentCustomer = _services.WorkContext.CurrentCustomer;
            if (portfolio == null)
            {
                var allTags = _personalTagService.GetAllTagNames();
                model.AvailableProductTags = new MultiSelectList(allTags, model.Tag);

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

                model.Tag = new[] { portfolio.Tag };
                var allTags = _personalTagService.GetAllTagNames();
                model.AvailableProductTags = new MultiSelectList(allTags, model.Tag);

                model.ShareLinkFacebook = portfolio.ShareLinkFacebook;
                model.ShareLinkInstagram = portfolio.ShareLinkInstagram;
                model.ShareLinkTwitter = portfolio.ShareLinkTwitter;

                model.CreatedDate = portfolio.CreatedDate;
                model.ModifiedDate = portfolio.ModifiedDate;
            }
        }

        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Read)]
        public ActionResult Edit(int id)
        {
            var portfolio = _personalPortfolioService.GetPortfolioById(id);
            if (portfolio == null)
            {
                NotifyWarning(T("Portfolio.NotFound", id));
                return RedirectToAction("List");
            }

            var model = portfolio.ToModel();
            PreparePortfolioModel(model, portfolio);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Update)]
        public ActionResult Edit(PersonalPortfolioModel model, bool continueEditing)
        {
            var portfolio = _personalPortfolioService.GetPortfolioById(model.Id);
            if (portfolio == null)
            {
                NotifyWarning(T("Portfolio.NotFound", model.Id));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                portfolio.PortfolioName = model.PortfolioName.ToString();
                portfolio.PortfolioUrl = model.PortfolioUrl;
                portfolio.ShortMessage = model.ShortMessage;
                portfolio.From = model.From;
                portfolio.To = model.To;
                portfolio.Published = model.Published;
                portfolio.Status = model.Status.ToString();
                portfolio.Description = model.Description;
                portfolio.Tag = string.Join(",", model.Tag);
                portfolio.ShareLinkFacebook = model.ShareLinkFacebook;
                portfolio.ShareLinkInstagram = model.ShareLinkInstagram;
                portfolio.ShareLinkTwitter = model.ShareLinkTwitter;
                portfolio.ModifiedDate = DateTime.Now;

                _personalPortfolioService.UpdatePortfolio(portfolio);

                _customerActivityService.InsertActivity("EditResume", T("ActivityLog.EditPortfolio"), portfolio.PortfolioName);

                NotifySuccess(T("Admin.Catalog.Portfolio.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = portfolio.Id }) : RedirectToAction("List");
            }

            // If we got this far, something failed, redisplay form.
            PreparePortfolioModel(model, portfolio);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Delete)]
        public ActionResult Delete(int id)
        {
            var portfolio = _personalPortfolioService.GetPortfolioById(id);
            _personalPortfolioService.DeletePortfolio(portfolio);

            _customerActivityService.InsertActivity("DeleteProduct", T("ActivityLog.DeleteSkill"), portfolio.PortfolioName);

            NotifySuccess(T("Admin.Catalog.Resume.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalPortfolio.Delete)]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    _personalPortfolioService.DeletePortfolio(id);
                }
            }

            NotifySuccess(T("Admin.Common.TaskSuccessfullyProcessed"));

            return Json(new { success = true });
        }
    }
}