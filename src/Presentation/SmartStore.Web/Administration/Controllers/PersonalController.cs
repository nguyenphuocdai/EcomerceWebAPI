using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NuGet;
using SmartStore.Admin.Models.Catalog;
using SmartStore.Admin.Models.CurriculumVitae;
using SmartStore.Admin.Models.Customers;
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
using SmartStore.Web.Framework.Modelling;
using SmartStore.Web.Framework.Security;
using Telerik.Web.Mvc;

namespace SmartStore.Admin.Controllers
{
    [AdminAuthorize]
    public class PersonalController : AdminControllerBase
    {
        private readonly IPersonalService _personalService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ICommonServices _services;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IMediaService _mediaService;
        private readonly IDownloadService _downloadService;
        private readonly MediaSettings _mediaSettings;

        public PersonalController(
            IPersonalService personalService, 
            AdminAreaSettings adminAreaSettings,
            ICommonServices services, 
            IEventPublisher eventPublisher, 
            ICustomerActivityService customerActivityService, 
            IMediaService mediaService, 
            IDownloadService downloadService, 
            MediaSettings mediaSettings)
        {
            _personalService = personalService;
            _adminAreaSettings = adminAreaSettings;
            _services = services;
            _eventPublisher = eventPublisher;
            _customerActivityService = customerActivityService;
            _mediaService = mediaService;
            _downloadService = downloadService;
            _mediaSettings = mediaSettings;
        }

        [Permission(Permissions.CurriculumVitae.Personal.Read)]
        public ActionResult List()
        {
            var listModel = new PersonalListModel();
            listModel.GridPageSize = _adminAreaSettings.GridPageSize;

            return View(listModel);
        }

        [HttpPost, GridAction(EnableCustomBinding = true)]
        [Permission(Permissions.CurriculumVitae.Personal.Read)]
        public ActionResult PersonalList(GridCommand command, PersonalListModel model)
        {
            // We use own own binder for searchCustomerRoleIds property.
            var gridModel = new GridModel<PersonalModel>();

            var q = new PersonalSearchQuery()
            {
                Email = model.SearchEmail,
                PhoneNumber = model.SearchPhoneNumber,
                PageIndex = command.Page - 1,
                PageSize = command.PageSize
            };

            var personals = _personalService.SearchPersonals(q);

            gridModel.Data = personals.Select(PreparePersonalModelForList);
            gridModel.Total = personals.TotalCount;

            return new JsonResult
            {
                Data = gridModel
            };
        }

        [NonAction]
        protected PersonalModel PreparePersonalModelForList(Personal personal)
        {
            var model = new PersonalModel();
            model.Id = personal.Id;
            model.CustomerId = personal.CustomerId;
            model.FullName = personal.FullName;
            model.Title = personal.Title;
            model.AliasName = personal.AliasName;
            model.BirthDate = personal.BirthDate;
            model.Residence = personal.Residence;
            model.Address = personal.Address;
            model.Email = personal.Email;
            model.PhoneNumber = personal.PhoneNumber;
            model.Introduce = personal.Introduce;
            model.Avatar = personal.Avatar;
            model.CV = personal.CV;
            model.IntroduceTitle00 = personal.IntroduceTitle00;
            model.IntroduceContent00 = personal.IntroduceContent00;
            model.IntroduceTitle01 = personal.IntroduceTitle01;
            model.IntroduceContent01 = personal.IntroduceContent01;
            model.IntroduceTitle02 = personal.IntroduceTitle02;
            model.IntroduceContent02 = personal.IntroduceContent02;
            model.FunFactTitle00 = personal.FunFactTitle00;
            model.FunFactContent00 = personal.FunFactContent00;
            model.FunFactTitle01 = personal.FunFactTitle01;
            model.FunFactContent01 = personal.FunFactContent01;
            model.FunFactTitle02 = personal.FunFactTitle02;
            model.FunFactContent02 = personal.FunFactContent02;
            model.FunFactTitle03 = personal.FunFactTitle03;
            model.FunFactContent03 = personal.FunFactContent03;
            model.IsFreelanceAvailable = personal.IsFreelanceAvailable;
            model.IsOpeningJob = personal.IsOpeningJob;
            model.FacebookLink = personal.FacebookLink;
            model.YoutubeLink = personal.YoutubeLink;
            model.TelegramLink = personal.TelegramLink;
            model.InstagramLink = personal.InstagramLink;
            model.TwitterLink = personal.TwitterLink;
            model.CreatedDate = personal.CreatedDate;
            model.ModifiedDate = personal.ModifiedDate;
            model.ShowOnHomePage = personal.ShowOnHomePage;
            return model;
        }

        [Permission(Permissions.CurriculumVitae.Personal.Create)]
        public ActionResult Create()
        {
            var model = new PersonalModel();
            PreparePersonalModel(model, null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.Personal.Create)]
        public ActionResult Create(PersonalModel model, bool continueEditing, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                Personal personal = new Personal();
                MapModelToPersonal(model, personal, form, out _);

                _personalService.InsertPersonal(personal);

                _customerActivityService.InsertActivity("AddNewPersonal", T("ActivityLog.AddNewPersonal"),
                    personal.FullName);
            }

            if (continueEditing)
            {
                return View(model);
            }

            NotifySuccess(T("Admin.Catalog.Vitae.Added"));
            // If we got this far, something failed, redisplay form.
            PreparePersonalModel(model, null);
            return View(model);
        }

        protected void PreparePersonalModel(PersonalModel model, Personal personal)
        {
            Guard.NotNull(model, nameof(model));
            Customer currentCustomer = _services.WorkContext.CurrentCustomer;
            if (personal == null)
            {
                model.CustomerId = currentCustomer.Id;
                model.FullName = currentCustomer.FullName;
                model.Title = string.Empty;
                model.AliasName = string.Empty;
                model.BirthDate = currentCustomer.BirthDate;
                model.Residence = string.Empty;
                model.Address = string.Empty;
                model.Email = currentCustomer.Email;
                model.PhoneNumber = string.Empty;
                model.Introduce = string.Empty;
                model.Avatar = string.Empty;
                model.CV = string.Empty;
                model.IntroduceTitle00 = string.Empty;
                model.IntroduceContent00 = string.Empty;
                model.IntroduceTitle01 = string.Empty;
                model.IntroduceContent01 = string.Empty;
                model.IntroduceTitle02 = string.Empty;
                model.IntroduceContent02 = string.Empty;
                model.FunFactTitle00 = string.Empty;
                model.FunFactContent00 = string.Empty;
                model.FunFactTitle01 = string.Empty;
                model.FunFactContent01 = string.Empty;
                model.FunFactTitle02 = string.Empty;
                model.FunFactContent02 = string.Empty;
                model.FunFactTitle03 = string.Empty;
                model.FunFactContent03 = string.Empty;
                model.IsFreelanceAvailable = false;
                model.IsOpeningJob = false;
                model.FacebookLink = string.Empty;
                model.YoutubeLink = string.Empty;
                model.TelegramLink = string.Empty;
                model.InstagramLink = string.Empty;
                model.TwitterLink = string.Empty;
                model.ShowOnHomePage = true;
                model.CreatedDate = null;
                model.ModifiedDate = null;
            }
            else
            {
                model.CustomerId = personal.CustomerId;
                model.FullName = personal.FullName;
                model.Title = personal.Title;
                model.AliasName = personal.AliasName;
                model.BirthDate = personal.BirthDate;
                model.Residence = personal.Residence;
                model.Address = personal.Address;
                model.Email = personal.Email;
                model.PhoneNumber = personal.PhoneNumber;
                model.Introduce = personal.Introduce;
                model.Avatar = personal.Avatar;
                model.CV = personal.CV;
                model.AddPictureModel.PictureId = string.IsNullOrWhiteSpace(personal.Avatar) ? 0 : int.Parse(personal.Avatar);

                var personalDownloads = _downloadService.GetDownloadsFor(personal, true);
                model.DownloadVersions = personalDownloads
                    .Select(x => new DownloadVersion
                    {
                        FileVersion = x.FileVersion,
                        DownloadId = x.Id,
                        FileName = x.UseDownloadUrl ? x.DownloadUrl : x.MediaFile?.Name,
                        DownloadUrl = x.UseDownloadUrl ? x.DownloadUrl : Url.Action("DownloadFile", "Download", new { downloadId = x.Id })
                    })
                    .ToList();
                var currentDownload = personalDownloads.FirstOrDefault();
                model.DownloadId = currentDownload?.Id;
                model.CurrentDownload = currentDownload;
                model.DownloadFileVersion = (currentDownload?.FileVersion).EmptyNull();
                if (currentDownload != null && currentDownload.MediaFile != null)
                {
                    model.DownloadThumbUrl = _mediaService.GetUrl(currentDownload.MediaFile.Id, _mediaSettings.CartThumbPictureSize, null, true);
                    currentDownload.DownloadUrl = Url.Action("DownloadFile", "Download", new { downloadId = currentDownload.Id });
                }

                model.IntroduceTitle00 = personal.IntroduceTitle00;
                model.IntroduceContent00 = personal.IntroduceContent00;
                model.IntroduceTitle01 = personal.IntroduceTitle01;
                model.IntroduceContent01 = personal.IntroduceContent01;
                model.IntroduceTitle02 = personal.IntroduceTitle02;
                model.IntroduceContent02 = personal.IntroduceContent02;
                model.FunFactTitle00 = personal.FunFactTitle00;
                model.FunFactContent00 = personal.FunFactContent00;
                model.FunFactTitle01 = personal.FunFactTitle01;
                model.FunFactContent01 = personal.FunFactContent01;
                model.FunFactTitle02 = personal.FunFactTitle02;
                model.FunFactContent02 = personal.FunFactContent02;
                model.FunFactTitle03 = personal.FunFactTitle03;
                model.FunFactContent03 = personal.FunFactContent03;
                model.IsFreelanceAvailable = personal.IsFreelanceAvailable;
                model.IsOpeningJob = personal.IsOpeningJob;
                model.FacebookLink = personal.FacebookLink;
                model.YoutubeLink = personal.YoutubeLink;
                model.TelegramLink = personal.TelegramLink;
                model.InstagramLink = personal.InstagramLink;
                model.TwitterLink = personal.TwitterLink;
                model.CreatedDate = personal.CreatedDate;
                model.ModifiedDate = personal.ModifiedDate;

                model.GridPageSize = _adminAreaSettings.GridPageSize;

                model.ShowOnHomePage = personal.ShowOnHomePage;
            }
        }

        [NonAction]
        protected void MapModelToPersonal(PersonalModel model, Personal personal, FormCollection form,
            out bool nameChanged)
        {
            if (model.LoadedTabs == null || model.LoadedTabs.Length == 0)
            {
                model.LoadedTabs = new string[] { "Info" };
            }

            nameChanged = false;

            foreach (var tab in model.LoadedTabs)
            {
                switch (tab.ToLowerInvariant())
                {
                    case "info":
                        UpdatePersonalGeneralInfo(personal, model, out nameChanged);
                        break;
                        //case "inventory":
                        //    UpdateProductInventory(product, model);
                        //    break;
                        //case "bundleitems":
                        //    UpdateProductBundleItems(product, model);
                        //    break;
                        //case "price":
                        //    UpdateProductPrice(product, model);
                        //    break;
                        //case "attributes":
                        //    UpdateProductAttributes(product, model);
                        //    break;
                        //case "downloads":
                        //    UpdateProductDownloads(product, model);
                        //    break;
                        //case "pictures":
                        //    UpdateProductPictures(product, model);
                        //    break;
                        //case "seo":
                        //    UpdateProductSeo(product, model);
                        //    break;
                }
            }

            _eventPublisher.Publish(new ModelBoundEvent(model, personal, form));
        }


        [NonAction]
        protected void UpdatePersonalGeneralInfo(Personal personal, PersonalModel model, out bool emailChanged)
        {
            var p = personal;
            var m = model;
            var currentTime = DateTime.Now;
            emailChanged = !string.Equals(p.Email, m.Email, StringComparison.CurrentCultureIgnoreCase);
            p.CustomerId = m.CustomerId;
            p.FullName = m.FullName;
            p.Title = m.Title;
            p.AliasName = m.AliasName;
            p.BirthDate = m.BirthDate;
            p.Residence = m.Residence;
            p.Address = m.Address;
            p.Email = m.Email;
            p.PhoneNumber = m.PhoneNumber;
            p.Introduce = m.Introduce;
            p.Avatar = m.AddPictureModel.PictureId.ToString();
            p.IntroduceTitle00 = m.IntroduceTitle00;
            p.IntroduceContent00 = m.IntroduceContent00;
            p.IntroduceTitle01 = m.IntroduceTitle01;
            p.IntroduceContent01 = m.IntroduceContent01;
            p.IntroduceTitle02 = m.IntroduceTitle02;
            p.IntroduceContent02 = m.IntroduceContent02;
            p.FunFactTitle00 = m.FunFactTitle00;
            p.FunFactContent00 = m.FunFactContent00;
            p.FunFactTitle01 = m.FunFactTitle01;
            p.FunFactContent01 = m.FunFactContent01;
            p.FunFactTitle02 = m.FunFactTitle02;
            p.FunFactContent02 = m.FunFactContent02;
            p.FunFactTitle03 = m.FunFactTitle03;
            p.FunFactContent03 = m.FunFactContent03;
            p.IsFreelanceAvailable = m.IsFreelanceAvailable;
            p.IsOpeningJob = m.IsOpeningJob;
            p.FacebookLink = m.FacebookLink;
            p.YoutubeLink = m.YoutubeLink;
            p.TelegramLink = m.TelegramLink;
            p.InstagramLink = m.InstagramLink;
            p.TwitterLink = m.TwitterLink;
            p.ShowOnHomePage = m.ShowOnHomePage;
            p.CreatedDate = currentTime;
            p.ModifiedDate = currentTime;
        }

        [Permission(Permissions.CurriculumVitae.Personal.Read)]
        public ActionResult Edit(int id)
        {
            var personal = _personalService.GetPersonalById(id);
            if (personal == null)
            {
                NotifyWarning(T("Personal.NotFound", id));
                return RedirectToAction("List");
            }

            var model = personal.ToModel();
            PreparePersonalModel(model, personal);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Permission(Permissions.CurriculumVitae.Personal.Update)]
        public ActionResult Edit(PersonalModel model, bool continueEditing, FormCollection form)
        {
            var personal = _personalService.GetPersonalById(model.Id);
            if (personal == null)
            {
                NotifyWarning(T("Personal.NotFound", model.Id));
                return RedirectToAction("List");
            }

            if (model.NewVersion.HasValue())
            {
                InsertPersonalDownload(model.NewVersionDownloadId, model.Id, model.NewVersion);
                personal.CV = model.NewVersionDownloadId + string.Empty;
            }
            else
            {
                InsertPersonalDownload(model.DownloadId, model.Id, model.DownloadFileVersion);
            }

            if (ModelState.IsValid)
            {
                MapModelToPersonal(model, personal, form, out var nameChanged);
                _personalService.UpdatePersonal(personal);

                _customerActivityService.InsertActivity("EditPersonal", T("ActivityLog.EditPersonal"), personal.FullName);

                NotifySuccess(T("Admin.Catalog.Vitae.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = personal.Id }) : RedirectToAction("List");
            }

            // If we got this far, something failed, redisplay form.
            PreparePersonalModel(model, personal);

            return View(model);
        }

        [NonAction]
        protected void InsertPersonalDownload(int? fileId, int entityId, string fileVersion = "")
        {
            if (fileId > 0)
            {
                var isUrlDownload = Request.Form["is-url-download-" + fileId] == "true";

                if (!isUrlDownload)
                {
                    var mediaFileInfo = _mediaService.GetFileById((int)fileId);
                    var download = new Download
                    {
                        MediaFile = mediaFileInfo.File,
                        EntityId = entityId,
                        EntityName = "Personal",
                        DownloadGuid = Guid.NewGuid(),
                        UseDownloadUrl = false,
                        DownloadUrl = string.Empty,
                        UpdatedOnUtc = DateTime.UtcNow,
                        IsTransient = false,
                        FileVersion = fileVersion
                    };

                    _downloadService.InsertDownload(download);
                }
                else
                {
                    var download = _downloadService.GetDownloadById((int)fileId);
                    download.FileVersion = fileVersion;
                    download.IsTransient = false;
                    _downloadService.UpdateDownload(download);
                }
            }
        }

        //[HttpPost, GridAction(EnableCustomBinding = true)]
        //[Permission(Permissions.CurriculumVitae.Personal.Read)]
        //public ActionResult PersonalClientList(GridCommand command)
        //{
        //    // We use own own binder for searchCustomerRoleIds property.
        //    var gridModel = new GridModel<PersonalClientModel>();
        //    var clients = _personalService.SearchPersonalClients(command.Page - 1, command.PageSize);

        //    gridModel.Data = clients.Select(PrepareSkillModelForList);
        //    gridModel.Total = clients.TotalCount;

        //    return new JsonResult
        //    {
        //        Data = gridModel
        //    };
        //}

        //[NonAction]
        //protected PersonalClientModel PrepareSkillModelForList(PersonalClient client)
        //{
        //    var model = new PersonalClientModel();
        //    model.Id = client.Id;
        //    model.CustomerId = client.CustomerId;
        //    model.ClientName = client.ClientName;
        //    model.ClientDescription = client.ClientDescription;
        //    model.ClientImageId = client.ClientImageId;
        //    model.CreatedDate = client.CreatedDate;
        //    model.ModifiedDate = client.ModifiedDate;
        //    return model;
        //}
    }

}