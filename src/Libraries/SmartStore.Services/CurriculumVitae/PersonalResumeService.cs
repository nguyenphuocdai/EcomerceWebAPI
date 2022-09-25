using System.Linq;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalResumeService : IPersonalResumeService
    {
        private readonly IRepository<PersonalResume> _personalResumeRepository;
        public PersonalResumeService(IRepository<PersonalResume> personalResumeRepository)
        {
            _personalResumeRepository = personalResumeRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<PersonalResume> SearchResumes(PersonalResumeSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<PersonalResume> query = null;
            query = _personalResumeRepository.Table;

            if (q.CustomerId > 0)
            {
                query = query.Where(c => c.CustomerId == q.CustomerId);
            }

            if (q.SearchCompany.HasValue())
            {
                query = query.Where(c => c.Company.Contains(q.SearchCompany));
            }
            var resumes = new PagedList<PersonalResume>(query, q.PageIndex, q.PageSize);
            return resumes;
        }

        public virtual void InsertResume(PersonalResume resume)
        {
            Guard.NotNull(resume, nameof(resume));

            _personalResumeRepository.Insert(resume);
        }

        public virtual PersonalResume GetResumeById(int id)
        {
            var resume = (_personalResumeRepository.Table).SingleOrDefault(x => x.Id == id);
            return resume;
        }

        public virtual void UpdateResume(PersonalResume resume)
        {
            Guard.NotNull(resume, nameof(resume));

            _personalResumeRepository.Update(resume);
        }

        public virtual void DeleteResume(PersonalResume resume)
        {
            Guard.NotNull(resume, nameof(resume));

            _personalResumeRepository.Delete(resume);
        }

        public virtual void DeleteResume(int id)
        {
            var resume = GetResumeById(id);
            if (resume != null)
                DeleteResume(resume);
        }
    }
}
