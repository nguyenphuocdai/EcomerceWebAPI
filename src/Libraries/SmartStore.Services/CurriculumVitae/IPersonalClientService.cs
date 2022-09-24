using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalClientService
    {
        IPagedList<PersonalClient> SearchPersonalClients(PersonalClientSearchQuery q);
        void InsertPersonalClient(PersonalClient personal);
    }
}
