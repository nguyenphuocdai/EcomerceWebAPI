using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalPortfolioMap : EntityTypeConfiguration<PersonalPortfolio>
    {
        public PersonalPortfolioMap()
        {
            ToTable("PersonalPortfolio");
            HasKey(c => c.Id);
            Property(u => u.CustomerId);
        }
    }
}
