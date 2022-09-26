using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalCertificateService
    {
        IPagedList<PersonalCertificate> SearchCertificates(PersonalCertificateSearchQuery q);
        void InsertCertificate(PersonalCertificate client);
        PersonalCertificate GetCertificateById(int id);
        void UpdateCertificate(PersonalCertificate client);
        void DeleteCertificate(PersonalCertificate client);
        void DeleteCertificate(int id);

    }
}
