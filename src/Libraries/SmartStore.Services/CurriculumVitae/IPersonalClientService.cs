using System.Collections.Generic;
using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalClientService
    {
        IPagedList<PersonalClient> SearchPersonalClients(PersonalClientSearchQuery q);
        void InsertPersonalClient(PersonalClient client);
        PersonalClient GetPersonalClientById(int id);
        void UpdateClient(PersonalClient client);
        void DeleteClient(PersonalClient client);
        void DeleteClient(int id);
        List<PersonalClient> GetClientByCustomerId(int id);

    }
}
