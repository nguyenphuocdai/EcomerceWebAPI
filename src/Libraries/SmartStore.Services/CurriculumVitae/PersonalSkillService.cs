using System.Linq;
using System.ServiceModel.Configuration;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalSkillService : IPersonalSkillService
    {
        private readonly IRepository<PersonalSkill> _personalSkillRepository;
        public PersonalSkillService(IRepository<PersonalSkill> personalSkillRepository)
        {
            _personalSkillRepository = personalSkillRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<PersonalSkill> SearchPersonalSkills(PersonalSkillSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<PersonalSkill> query = null;
            query = _personalSkillRepository.Table;

            if (q.SkillName.HasValue())
            {
                query = query.Where(c => c.SkillName.Contains(q.SkillName));
            }

            if (q.CustomerId > 0)
            {
                query = query.Where(c => c.CustomerId == q.CustomerId);
            }
            var skills = new PagedList<PersonalSkill>(query, q.PageIndex, q.PageSize);
            return skills;
        }

        public virtual void InsertPersonalSkill(PersonalSkill personalSkill)
        {
            Guard.NotNull(personalSkill, nameof(personalSkill));

            _personalSkillRepository.Insert(personalSkill);
        }

        public virtual bool IsExistPersonalSkill(int customerId, string skillName)
        {
            IQueryable<PersonalSkill> query = null;
            query = _personalSkillRepository.Table;

            if (customerId > 0)
            {
                query = query.Where(c => c.CustomerId == customerId);
            }

            if (skillName.HasValue())
            {
                query = query.Where(c => c.SkillName.Trim().ToUpper() == skillName.Trim().ToUpper());
            }

            var skills = new PagedList<PersonalSkill>(query, 0, 9999);
            if (skills.Count > 0)
            {
                return true;
            }

            return false;
        }

        public virtual PersonalSkill GetPersonalSkillById(int id)
        {
            var skill = (_personalSkillRepository.Table).SingleOrDefault(x => x.Id == id);
            return skill;
        }

        public virtual void UpdateSkill(PersonalSkill personalSkill)
        {
            Guard.NotNull(personalSkill, nameof(personalSkill));

            _personalSkillRepository.Update(personalSkill);
        }

        public virtual void DeleteSkill(PersonalSkill personalSkill)
        {
            Guard.NotNull(personalSkill, nameof(personalSkill));

            _personalSkillRepository.Delete(personalSkill);
        }

        public virtual void DeleteSkill(int id)
        {
            var skill = GetPersonalSkillById(id);
            if (skill != null)
                DeleteSkill(skill);
        }
    }
}
