using System.Collections.Generic;
using System.Linq;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalTagService : IPersonalTagService
    {
        private readonly IRepository<PersonalTag> _personalTagRepository;
        public PersonalTagService(IRepository<PersonalTag> personalTagRepository)
        {
            _personalTagRepository = personalTagRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<PersonalTag> SearchTags(PersonalTagSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<PersonalTag> query = null;
            query = _personalTagRepository.Table;

            if (q.Name.HasValue())
            {
                query = query.Where(c => c.Name.Contains(q.Name));
            }

            var tags = new PagedList<PersonalTag>(query, q.PageIndex, q.PageSize);
            return tags;
        }

        public virtual void InsertTag(PersonalTag tag)
        {
            Guard.NotNull(tag, nameof(tag));

            _personalTagRepository.Insert(tag);
        }

        public virtual bool IsExistTag(string tagName)
        {
            var skill = (_personalTagRepository.Table).SingleOrDefault(x => x.Name == tagName);
            return skill != null;
        }

        public virtual PersonalTag GetTagById(int id)
        {
            var skill = (_personalTagRepository.Table).SingleOrDefault(x => x.Id == id);
            return skill;
        }

        public virtual void UpdateTag(PersonalTag tag)
        {
            Guard.NotNull(tag, nameof(tag));

            _personalTagRepository.Update(tag);
        }

        public virtual void DeleteTag(PersonalTag tag)
        {
            Guard.NotNull(tag, nameof(tag));

            _personalTagRepository.Delete(tag);
        }

        public virtual void DeleteTag(int id)
        {
            var skill = GetTagById(id);
            if (skill != null)
                DeleteTag(skill);
        }

        public virtual IList<string> GetAllTagNames()
        {
            var query = from pt in _personalTagRepository.TableUntracked
                orderby pt.Name ascending
                select pt.Name;
            return query.ToList();
        }
    }
}
