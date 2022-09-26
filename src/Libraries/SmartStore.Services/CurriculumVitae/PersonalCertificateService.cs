using System.Linq;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalCertificateService : IPersonalCertificateService
    {
        private readonly IRepository<PersonalCertificate> _personalCertificateRepository;
        public PersonalCertificateService(IRepository<PersonalCertificate> personalCertificateRepository)
        {
            _personalCertificateRepository = personalCertificateRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<PersonalCertificate> SearchCertificates(PersonalCertificateSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<PersonalCertificate> query = null;
            query = _personalCertificateRepository.Table;

            if (q.Name.HasValue())
            {
                query = query.Where(c => c.Name.Contains(q.Name));
            }

            var clients = new PagedList<PersonalCertificate>(query, q.PageIndex, q.PageSize);
            return clients;
        }

        public virtual void InsertCertificate(PersonalCertificate certificate)
        {
            Guard.NotNull(certificate, nameof(certificate));

            _personalCertificateRepository.Insert(certificate);
        }

        public virtual PersonalCertificate GetCertificateById(int id)
        {
            var certificate = (_personalCertificateRepository.Table).SingleOrDefault(x => x.Id == id);
            return certificate;
        }

        public virtual void UpdateCertificate(PersonalCertificate certificate)
        {
            Guard.NotNull(certificate, nameof(certificate));

            _personalCertificateRepository.Update(certificate);
        }

        public virtual void DeleteCertificate(PersonalCertificate certificate)
        {
            Guard.NotNull(certificate, nameof(certificate));

            _personalCertificateRepository.Delete(certificate);
        }

        public virtual void DeleteCertificate(int id)
        {
            var skill = GetCertificateById(id);
            if (skill != null)
                DeleteCertificate(skill);
        }
    }
}
