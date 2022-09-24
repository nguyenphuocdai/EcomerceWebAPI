using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalMap : EntityTypeConfiguration<Personal>
    {
        public PersonalMap()
        {
            ToTable("Personal");
            HasKey(c => c.Id);
            Property(u => u.CustomerId);
            //Property(u => u.FullName).HasMaxLength(450);
            //Property(u => u.Title).HasMaxLength(100);
            //Property(u => u.AliasName).HasMaxLength(450);
            //Property(u => u.BirthDate);

            Property(u => u.Email).HasMaxLength(100);
            Property(u => u.PhoneNumber).HasMaxLength(100);
        }
    }
}
