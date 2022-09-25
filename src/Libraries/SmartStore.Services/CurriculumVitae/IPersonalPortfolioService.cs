using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalPortfolioService
    {
        IPagedList<PersonalPortfolio> SearchPortfolios(PersonalPortfolioSearchQuery q);
        void InsertPortfolio(PersonalPortfolio portfolio);
        PersonalPortfolio GetPortfolioById(int id);
        void UpdatePortfolio(PersonalPortfolio client);
        void DeletePortfolio(PersonalPortfolio client);
        void DeletePortfolio(int id);
    }
}
