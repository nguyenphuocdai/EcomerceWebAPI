using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalTagMap : EntityTypeConfiguration<PersonalTag>
    {
        public PersonalTagMap()
        {
            ToTable("PersonalTag");
            HasKey(c => c.Id);
        }
    }
}
