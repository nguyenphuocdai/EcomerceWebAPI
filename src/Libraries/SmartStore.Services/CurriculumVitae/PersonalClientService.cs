using System.Collections.Generic;
using System.Linq;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalClientService : IPersonalClientService
    {
        private readonly IRepository<PersonalClient> _personalClientRepository;
        public PersonalClientService(IRepository<PersonalClient> personalClientRepository)
        {
            _personalClientRepository = personalClientRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<PersonalClient> SearchPersonalClients(PersonalClientSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<PersonalClient> query = null;
            query = _personalClientRepository.Table;

            if (q.Name.HasValue())
            {
                query = query.Where(c => c.ClientName.Contains(q.Name));
            }

            if (q.Description.HasValue())
            {
                query = query.Where(c => c.ClientDescription.Contains(q.Description));
            }
            var clients = new PagedList<PersonalClient>(query, q.PageIndex, q.PageSize);
            return clients;
        }

        public virtual void InsertPersonalClient(PersonalClient personalClient)
        {
            Guard.NotNull(personalClient, nameof(personalClient));

            _personalClientRepository.Insert(personalClient);
        }

        public virtual PersonalClient GetPersonalClientById(int id)
        {
            var client = (_personalClientRepository.Table).SingleOrDefault(x => x.Id == id);
            return client;
        }

        public virtual List<PersonalClient> GetClientByCustomerId(int id)
        {
            var clients = (_personalClientRepository.Table).Where(x => x.CustomerId == id).ToList();
            return clients;
        }

        public virtual void UpdateClient(PersonalClient personalClient)
        {
            Guard.NotNull(personalClient, nameof(personalClient));

            _personalClientRepository.Update(personalClient);
        }

        public virtual void DeleteClient(PersonalClient personalClient)
        {
            Guard.NotNull(personalClient, nameof(personalClient));

            _personalClientRepository.Delete(personalClient);
        }

        public virtual void DeleteClient(int id)
        {
            var skill = GetPersonalClientById(id);
            if (skill != null)
                DeleteClient(skill);
        }
    }
}
