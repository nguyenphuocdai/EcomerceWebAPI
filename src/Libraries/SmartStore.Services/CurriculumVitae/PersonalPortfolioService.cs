using System.Linq;
using SmartStore.Core;
using SmartStore.Core.Data;
using SmartStore.Core.Domain.CurriculumVitae;
using SmartStore.Core.Localization;
using SmartStore.Core.Logging;

namespace SmartStore.Services.CurriculumVitae
{
    public class PersonalPortfolioService : IPersonalPortfolioService
    {
        private readonly IRepository<PersonalPortfolio> _personalPortfolioRepository;
        public PersonalPortfolioService(IRepository<PersonalPortfolio> personalPortfolioRepository)
        {
            _personalPortfolioRepository = personalPortfolioRepository;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public virtual IPagedList<PersonalPortfolio> SearchPortfolios(PersonalPortfolioSearchQuery q)
        {
            Guard.NotNull(q, nameof(q));

            IQueryable<PersonalPortfolio> query = null;
            query = _personalPortfolioRepository.Table;

            if (q.CustomerId > 0)
            {
                query = query.Where(c => c.CustomerId == q.CustomerId);
            }

            if (q.SearchName.HasValue())
            {
                query = query.Where(c => c.PortfolioName.Contains(q.SearchName));
            }
            var portfolios = new PagedList<PersonalPortfolio>(query, q.PageIndex, q.PageSize);
            return portfolios;
        }

        public virtual void InsertPortfolio(PersonalPortfolio portfolio)
        {
            Guard.NotNull(portfolio, nameof(portfolio));

            _personalPortfolioRepository.Insert(portfolio);
        }

        public virtual PersonalPortfolio GetPortfolioById(int id)
        {
            var portfolio = (_personalPortfolioRepository.Table).SingleOrDefault(x => x.Id == id);
            return portfolio;
        }

        public virtual void UpdatePortfolio(PersonalPortfolio portfolio)
        {
            Guard.NotNull(portfolio, nameof(portfolio));

            _personalPortfolioRepository.Update(portfolio);
        }

        public virtual void DeletePortfolio(PersonalPortfolio portfolio)
        {
            Guard.NotNull(portfolio, nameof(portfolio));

            _personalPortfolioRepository.Delete(portfolio);
        }

        public virtual void DeletePortfolio(int id)
        {
            var resume = GetPortfolioById(id);
            if (resume != null)
                DeletePortfolio(resume);
        }
    }
}
