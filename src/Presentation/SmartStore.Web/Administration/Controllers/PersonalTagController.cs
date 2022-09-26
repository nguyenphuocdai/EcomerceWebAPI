using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SmartStore.Admin.Models.CurriculumVitae;
using SmartStore.Core.Domain.Common;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Logging;
using SmartStore.Core.Security;
using SmartStore.Services.CurriculumVitae;
using SmartStore.Web.Framework.Controllers;
using SmartStore.Web.Framework.Filters;
using SmartStore.Web.Framework.Security;
using Telerik.Web.Mvc;

namespace SmartStore.Admin.Controllers
{
    [AdminAuthorize]
    public class PersonalTagController : AdminControllerBase
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPersonalTagService _personalTagService;

        public PersonalTagController(
            IPersonalTagService personalTagService,
            AdminAreaSettings adminAreaSettings,
            ICustomerActivityService customerActivityService)
        {
            _personalTagService = personalTagService;

            _adminAreaSettings = adminAreaSettings;
            _customerActivityService = customerActivityService;
        }

        [Permission(Permissions.CurriculumVitae.PersonalTag.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalTagListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost]
        [GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.PersonalTag.Read)]
        public ActionResult PersonalTagList(GridCommand command, PersonalTagListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalTagModel>();

            var q = new PersonalTagSearchQuery()
            {
                Name = model.SearchName,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var tags = _personalTagService.SearchTags(q);

            gridModel.Data = tags.Select(PrepareTagModelForList);
            gridModel.Total = tags.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalTagModel PrepareTagModelForList(PersonalTag tag)
        {
            var model = new PersonalTagModel();
            model.Id = tag.Id;
            model.Name = tag.Name;
            model.Published = tag.Published;
            model.CreatedDate = tag.CreatedDate;
            model.ModifiedDate = tag.ModifiedDate;

            return model;
        }

        [Permission(Permissions.CurriculumVitae.PersonalTag.Create)]
        public ActionResult Create()
        {
            var model = new PersonalTagModel();
            PrepareTagModel(model, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalTag.Create)]
        public ActionResult Create(PersonalTagModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                PersonalTag tag = new PersonalTag();
                MapModelToClient(model, tag);

                if (_personalTagService.IsExistTag(model.Name))
                {
                    NotifyWarning(T("Admin.Catalog.Tag.Exist"));
                    return View(model);
                }
                else
                {
                    _personalTagService.InsertTag(tag);
                    _customerActivityService.InsertActivity("AddNewTag", T("ActivityLog.AddNewClient"), tag.Name);
                }
            }

            if (continueEditing)
            {
                return View(model);
            }

            NotifySuccess(T("Admin.Catalog.Tag.Added"));
            // If we got this far, something failed, redisplay form.
            PrepareTagModel(model, null);
            return RedirectToAction("List");
        }

        [NonAction]
        protected void MapModelToClient(PersonalTagModel model, PersonalTag tag)
        {
            var p = tag;
            var m = model;
            var currentTime = DateTime.Now;
            p.Name = m.Name;
            p.Published = m.Published;
            p.CreatedDate = currentTime;
            p.ModifiedDate = currentTime;
        }

        protected void PrepareTagModel(PersonalTagModel model, PersonalTag tag)
        {
            Guard.NotNull(model, nameof(model));
            if (tag == null)
            {
                model.Published = true;
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
            }
            else
            {
                model.Name = tag.Name;
                model.Published = tag.Published;
                model.CreatedDate = tag.CreatedDate;
                model.ModifiedDate = tag.ModifiedDate;
            }
        }

        [Permission(Permissions.CurriculumVitae.PersonalTag.Read)]
        public ActionResult Edit(int id)
        {
            var tag = _personalTagService.GetTagById(id);
            if (tag == null)
            {
                NotifyWarning(T("Tag.NotFound", id));
                return RedirectToAction("List");
            }

            var model = tag.ToModel();
            PrepareTagModel(model, tag);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalTag.Update)]
        public ActionResult Edit(PersonalTagModel model, bool continueEditing)
        {
            var tag = _personalTagService.GetTagById(model.Id);
            if (tag == null)
            {
                NotifyWarning(T("Tag.NotFound", model.Id));
                return RedirectToAction("List");
            }

            if (_personalTagService.IsExistTag(model.Name))
            {
                NotifyWarning(T("Admin.Catalog.Tag.Exist"));
                PrepareTagModel(model, tag);
                return View(model);
            }

            if (ModelState.IsValid)
            {
                tag.Name = model.Name;
                tag.Published = model.Published;
                tag.ModifiedDate = DateTime.Now;

                _personalTagService.UpdateTag(tag);

                _customerActivityService.InsertActivity("EditTag", T("ActivityLog.EditTag"), tag.Name);

                NotifySuccess(T("Admin.Catalog.Tag.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = tag.Id }) : RedirectToAction("List");
            }

            // If we got this far, something failed, redisplay form.
            PrepareTagModel(model, tag);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalTag.Delete)]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    _personalTagService.DeleteTag(id);
                }
            }

            NotifySuccess(T("Admin.Common.TaskSuccessfullyProcessed"));

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.PersonalTag.Delete)]
        public ActionResult Delete(int id)
        {
            var tag = _personalTagService.GetTagById(id);
            _personalTagService.DeleteTag(tag);

            _customerActivityService.InsertActivity("DeleteClient", T("ActivityLog.DeleteClient"), tag.Name);

            NotifySuccess(T("Admin.Catalog.Client.Deleted"));
            return RedirectToAction("List");
        }
    }
}