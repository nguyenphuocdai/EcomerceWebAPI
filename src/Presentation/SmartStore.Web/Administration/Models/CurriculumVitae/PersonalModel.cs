using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation;
using FluentValidation.Attributes;
using SmartStore.Admin.Models.Catalog;
using SmartStore.Core.Domain.Catalog;
using SmartStore.Core.Domain.Media;
using SmartStore.Core.Localization;
using SmartStore.Web.Framework;
using SmartStore.Web.Framework.Modelling;

namespace SmartStore.Admin.Models.CurriculumVitae
{
    [Validator(typeof(PersonalValidator))]
    public class PersonalModel : TabbableModel
    {
        public PersonalModel() 
        {
            AddPictureModel = new PersonalPictureModel();
            AddPersonalClientModel = new PersonalClientModel();
        }

        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string FullName { get; set; }

        public string Title { get; set; }

        public string AliasName { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Residence { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Introduce { get; set; }

        public string Avatar { get; set; }

        public string CV { get; set; }

        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.IntroduceTitle00")]
        public string IntroduceTitle00 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.IntroduceContent00")]
        public string IntroduceContent00 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.IntroduceTitle01")]
        public string IntroduceTitle01 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.IntroduceContent01")]
        public string IntroduceContent01 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.IntroduceTitle02")]
        public string IntroduceTitle02 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.IntroduceContent02")]
        public string IntroduceContent02 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactTitle00")] 
        public string FunFactTitle00 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactContent00")]
        public string FunFactContent00 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactTitle01")]
        public string FunFactTitle01 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactContent01")]
        public string FunFactContent01 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactTitle02")]
        public string FunFactTitle02 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactContent02")]
        public string FunFactContent02 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactTitle03")]
        public string FunFactTitle03 { get; set; }
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.FunFactContent03")]
        public string FunFactContent03 { get; set; }

        public bool IsFreelanceAvailable { get; set; }

        public bool IsOpeningJob { get; set; }

        public string FacebookLink { get; set; }

        public string YoutubeLink { get; set; }

        public string TelegramLink { get; set; }

        public string TwitterLink { get; set; }
        public string InstagramLink { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
        public bool ShowOnHomePage { get; set; }

        //Pictures.
        [SmartResourceDisplayName("Admin.Vitae.Personal.Fields.HasPreviewPicture")]
        public bool HasPreviewPicture { get; set; }
        public IList<ProductMediaFile> ProductMediaFiles { get; set; }
        public PersonalPictureModel AddPictureModel { get; set; }




        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.IsDownload")]
        public bool IsDownload { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.NewVersionDownloadId")]
        [UIHint("Download")]
        public int? NewVersionDownloadId { get; set; }

        [SmartResourceDisplayName("Common.Download.Version")]
        public string NewVersion { get; set; }

        public List<DownloadVersion> DownloadVersions { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.Download")]
        [UIHint("Download")]
        public int? DownloadId { get; set; }
        public string DownloadThumbUrl { get; set; }
        public Download CurrentDownload { get; set; }

        [SmartResourceDisplayName("Common.Download.Version")]
        public string DownloadFileVersion { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.UnlimitedDownloads")]
        public bool UnlimitedDownloads { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.MaxNumberOfDownloads")]
        public int MaxNumberOfDownloads { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.DownloadExpirationDays")]
        public int? DownloadExpirationDays { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.DownloadActivationType")]
        public int DownloadActivationTypeId { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.HasSampleDownload")]
        public bool HasSampleDownload { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.SampleDownload")]
        [UIHint("Download")]
        public int? SampleDownloadId { get; set; }

        public int? OldSampleDownloadId { get; set; }

        [AllowHtml]
        public string AddChangelog { get; set; }






        // Clients

        public PersonalClientModel AddPersonalClientModel { get; set; }

        public int GridPageSize { get; set; }

        // End Clients









    }
    public class PersonalPictureModel : EntityModelBase
    {
        public int ProductId { get; set; }

        [UIHint("Media"), AdditionalMetadata("album", "catalog"), AdditionalMetadata("typeFilter", "image,video")]
        [SmartResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
        public int PictureId { get; set; }

        [SmartResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
        public string PictureUrl { get; set; }

        [SmartResourceDisplayName("Common.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public ProductMediaFile ProductMediaFile { get; set; }
    }

    public partial class PersonalValidator : AbstractValidator<PersonalModel>
    {
        public PersonalValidator(Localizer T)
        {
            RuleFor(x => x.FullName).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.NewVersion)
                .NotEmpty()
                .When(x => x.NewVersionDownloadId != null && x.NewVersionDownloadId != 0)
                .WithMessage(T("Admin.Catalog.Products.Download.SemanticVersion.NotValid"));
        }
    }

    public class PersonalClientModel : EntityModelBase
    {
        public int CustomerId { get; set; }
        public string ClientName { get; set; }
        public string ClientDescription { get; set; }

        [UIHint("Media"), AdditionalMetadata("album", "catalog"), AdditionalMetadata("typeFilter", "image,video")]
        [SmartResourceDisplayName("Admin.Catalog.Personal.Client.Fields.Picture")]
        public int? ClientImageId { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        public ProductMediaFile ClientMediaFile { get; set; }
        public bool NoThumb { get; set; }
    }

    public class PersonalCertificateModel : EntityModelBase
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [UIHint("Media"), AdditionalMetadata("album", "catalog"), AdditionalMetadata("typeFilter", "image,video")]
        [SmartResourceDisplayName("Admin.Catalog.Personal.Client.Fields.Picture")]
        public int? ImageId { get; set; }
        public DateTime? CertificateDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [SmartResourceDisplayName("Admin.Catalog.Products.Fields.PictureThumbnailUrl")]
        public string PictureThumbnailUrl { get; set; }
        public ProductMediaFile MediaFile { get; set; }
        public bool NoThumb { get; set; }
    }

    public class PersonalTagModel : EntityModelBase
    {
        public string Name { get; set; }
        public bool Published { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class PersonalResumeModel : EntityModelBase
    {
        public int CustomerId { get; set; }
        public int ResumeType { get; set; }
        public string ResumeShort { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Validator(typeof(PortfolioValidator))]
    public class PersonalPortfolioModel : EntityModelBase
    {
        public int CustomerId { get; set; }
        public string PortfolioName { get; set; }
        public string PortfolioUrl { get; set; }
        public string ShortMessage { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTime? Published { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string[] Tag { get; set; }
        public MultiSelectList AvailableProductTags { get; set; }
        public string ShareLinkFacebook { get; set; }
        public string ShareLinkTwitter { get; set; }
        public string ShareLinkInstagram { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    [Validator(typeof(SkillValidator))]
    public class PersonalSkillModel : EntityModelBase
    {
        public int CustomerId { get; set; }
        public int SkillType { get; set; }
        public string SkillName { get; set; }
        public string SkillShortName { get; set; }
        public decimal SkillPercentage { get; set; }
        public decimal SkillStar { get; set; }
        public int? ClientImageId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public partial class SkillValidator : AbstractValidator<PersonalSkillModel>
    {
        public SkillValidator(Localizer T)
        {
            RuleFor(x => x.SkillName).NotEmpty().Length(1, 150);
            RuleFor(x => x.SkillPercentage).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SkillStar).GreaterThanOrEqualTo(0).LessThan(6);
        }
    }

    public partial class PortfolioValidator : AbstractValidator<PersonalPortfolioModel>
    {
        public PortfolioValidator(Localizer T)
        {
            RuleFor(x => x.PortfolioName).NotEmpty().Length(1, 250);
        }
    }
}