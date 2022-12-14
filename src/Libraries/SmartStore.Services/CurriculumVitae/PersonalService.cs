using System.Collections.Generic;
using System.Linq;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalService : IPersonalService
    {
        private readonly IRepository<Personal> _personalRepository;

        public PersonalService(IRepository<Personal> personalRepository)
        {
            _personalRepository = personalRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<Personal> SearchPersonals(PersonalSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<Personal> query = null;
            query = _personalRepository.Table;

            if (q.Email.HasValue())
            {
                query = query.Where(c => c.Email.Contains(q.Email));
            }

            if (q.PhoneNumber.HasValue())
            {
                query = query.Where(c => c.PhoneNumber.Contains(q.PhoneNumber));
            }
            var personals = new PagedList<Personal>(query, q.PageIndex, q.PageSize);
            return personals;
        }

        public virtual void InsertPersonal(Personal personal)
        {
            Guard.NotNull(personal, nameof(personal));

            _personalRepository.Insert(personal);
        }

        /// <summary>
        /// Allow insert new personal each customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns></returns>
        public virtual bool IsAllowInsert(int customerId)
        {
            Personal person = GetPersonalByCustomerId(customerId);
            return person != null;
        }

        public virtual Personal GetPersonalById(int id)
        {
            if (id == 0)
                return null;

            return _personalRepository.GetById(id);
        }

        public virtual Personal GetPersonalByCustomerId(int id)
        {
            if (id == 0)
                return null;
            return (_personalRepository.Table).SingleOrDefault(item => item.CustomerId == id);
        }

        public virtual IList<Personal> GetAllPersonalDisplayedOnHomePage()
        {
            var query = from c in _personalRepository.Table
                orderby c.Id
                where c.ShowOnHomePage
                select c;

            var personals = query.ToList();
            return personals;
        }

        public virtual void UpdatePersonal(Personal personal)
        {
            Guard.NotNull(personal, nameof(personal));

            _personalRepository.Update(personal);
        }
    }
}
