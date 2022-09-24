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
    public class PersonalSkillController : AdminControllerBase
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPersonalSkillService _personalSkillService;
        private readonly ICommonServices _services;

        public PersonalSkillController(
            IPersonalSkillService personalSkillService,
            AdminAreaSettings adminAreaSettings,
            ICommonServices services,
            IEventPublisher eventPublisher,
            ICustomerActivityService customerActivityService,
            IMediaService mediaService,
            MediaSettings mediaSettings)
        {
            _personalSkillService = personalSkillService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        [Permission(Permissions.CurriculumVitae.PersonalSkill.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalSkillListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.PersonalSkill.Read)]
        public ActionResult PersonalSkillList(GridCommand command, PersonalSkillListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalSkillModel>();

            var q = new PersonalSkillSearchQuery
            {
                SkillName = model.SkillName,
                CustomerId = model.CustomerId,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var skills = _personalSkillService.SearchPersonalSkills(q);

            gridModel.Data = skills.Select(PrepareSkillModelForList);
            gridModel.Total = skills.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalSkillModel PrepareSkillModelForList(PersonalSkill item)
        {
            var model = new PersonalSkillModel();
            model.Id = item.Id;
            model.CustomerId = item.CustomerId;

            model.SkillType = int.Parse(item.SkillType);
            model.SkillName = item.SkillName;
            model.SkillShortName = item.SkillShortName;
            model.SkillPercentage = item.SkillPercentage;
            model.SkillStar = item.SkillStar;
            model.CreatedDate = item.CreatedDate;
            model.ModifiedDate = item.ModifiedDate;
            return model;
        }

        [Permission(Permissions.CurriculumVitae.PersonalSkill.Create)]
        public ActionResult Create()
        {
            var model = new PersonalSkillModel();
            PrepareSkillModel(model, null);

            return View(model);
        }

        protected void PrepareSkillModel(PersonalSkillModel model, PersonalSkill personalSkill)
        {
            Guard.NotNull(model, nameof(model));
            Customer currentCustomer = _services.WorkContext.CurrentCustomer;

            if (personalSkill == null)
            {
                model.CustomerId = currentCustomer.Id;
                model.SkillName = string.Empty;
                model.SkillShortName = string.Empty;
                model.SkillPercentage = 0;
                model.SkillStar = 0;
            }
            else
            {
                model.Id = personalSkill.Id;
                model.CustomerId = personalSkill.CustomerId;
                model.SkillType = int.Parse(personalSkill.SkillType);
                model.SkillName = personalSkill.SkillName;
                model.SkillShortName = personalSkill.SkillShortName;
                model.SkillPercentage = personalSkill.SkillPercentage;
                model.SkillStar = personalSkill.SkillStar;
                model.CreatedDate = personalSkill.CreatedDate;
                model.ModifiedDate = personalSkill.ModifiedDate;
            }
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalSkill.Create)]
        public ActionResult Create(PersonalSkillModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                PersonalSkill skills = new PersonalSkill();
                MapModelToSkill(model, skills);

                if (_personalSkillService.IsExistPersonalSkill(skills.CustomerId, skills.SkillName))
                {
                    NotifyWarning(T("Admin.Catalog.Skill.Exists"));
                    return View(model);
                }

                _personalSkillService.InsertPersonalSkill(skills);

                _customerActivityService.InsertActivity("AddNewClient", T("ActivityLog.AddNewClient"), skills.SkillName);
            }

            if (continueEditing)
            {
                return View(model);
            }

            NotifySuccess(T("Admin.Catalog.Client.Added"));
            // If we got this far, something failed, redisplay form.
            PrepareSkillModel(model, null);
            return RedirectToAction("List");
        }

        [NonAction]
        protected void MapModelToSkill(PersonalSkillModel model, PersonalSkill personal)
        {
            var p = personal;
            var m = model;
            var currentTime = DateTime.Now;

            p.CustomerId = m.CustomerId;
            p.SkillType = m.SkillType.ToString();
            p.SkillName = m.SkillName;
            p.SkillShortName = m.SkillShortName;
            p.SkillPercentage = m.SkillPercentage;
            p.SkillStar = m.SkillStar;
            p.CreatedDate = currentTime;
            p.ModifiedDate = currentTime;
        }

        [Permission(Permissions.CurriculumVitae.PersonalSkill.Read)]
        public ActionResult Edit(int id)
        {
            var skill = _personalSkillService.GetPersonalSkillById(id);
            if (skill == null)
            {
                NotifyWarning(T("Skill.NotFound", id));
                return RedirectToAction("List");
            }

            var model = skill.ToModel();
            PrepareSkillModel(model, skill);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalSkill.Update)]
        public ActionResult Edit(PersonalSkillModel model, bool continueEditing)
        {
            var skill = _personalSkillService.GetPersonalSkillById(model.Id);
            if (skill == null)
            {
                NotifyWarning(T("Skill.NotFound", model.Id));
                return RedirectToAction("List");
            }

            if (ModelState.IsValid)
            {
                skill.SkillType = model.SkillType.ToString();
                skill.SkillName = model.SkillName;
                skill.SkillShortName = model.SkillShortName;
                skill.SkillPercentage = model.SkillPercentage;
                skill.SkillStar = model.SkillStar;
                skill.ModifiedDate = DateTime.Now;

                _personalSkillService.UpdateSkill(skill);

                _customerActivityService.InsertActivity("EditSkill", T("ActivityLog.EditSkill"), skill.SkillName);

                NotifySuccess(T("Admin.Catalog.Skill.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = skill.Id }) : RedirectToAction("List");
            }

            // If we got this far, something failed, redisplay form.
            PrepareSkillModel(model, skill);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalSkill.Delete)]
        public ActionResult Delete(int id)
        {
            var skill = _personalSkillService.GetPersonalSkillById(id);
            _personalSkillService.DeleteSkill(skill);

            _customerActivityService.InsertActivity("DeleteProduct", T("ActivityLog.DeleteSkill"), skill.SkillName);

            NotifySuccess(T("Admin.Catalog.Skill.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalSkill.Delete)]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    _personalSkillService.DeleteSkill(id);
                }
            }

            NotifySuccess(T("Admin.Common.TaskSuccessfullyProcessed"));

            return Json(new { success = true });
        }
    }
}