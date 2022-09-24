using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalClientMap : EntityTypeConfiguration<PersonalClient>
    {
        public PersonalClientMap()
        {
            ToTable("PersonalClient");
            HasKey(c => c.Id);
            Property(u => u.CustomerId);
        }
    }
}
