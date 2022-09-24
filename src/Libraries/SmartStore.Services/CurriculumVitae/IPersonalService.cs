using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalService
    {
        IPagedList<Personal> SearchPersonals(PersonalSearchQuery q);
        void InsertPersonal(Personal personal);
        Personal GetPersonalById(int id);
        void UpdatePersonal(Personal personal);
    }
}
