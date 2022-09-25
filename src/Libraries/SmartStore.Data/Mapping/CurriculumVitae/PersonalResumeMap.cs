using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalResumeMap : EntityTypeConfiguration<PersonalResume>
    {
        public PersonalResumeMap()
        {
            ToTable("PersonalResume");
            HasKey(c => c.Id);
            Property(u => u.CustomerId);
        }
    }
}
