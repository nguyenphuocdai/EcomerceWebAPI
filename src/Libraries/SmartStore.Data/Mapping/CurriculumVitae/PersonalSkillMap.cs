using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalSkillMap : EntityTypeConfiguration<PersonalSkill>
    {
        public PersonalSkillMap()
        {
            ToTable("PersonalSkills");
            HasKey(c => c.Id);
            Property(u => u.CustomerId);
        }
    }
}
