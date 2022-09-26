using System.Data.Entity.ModelConfiguration;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Data.Mapping.CurriculumVitae
{
    public partial class PersonalCertificateMap : EntityTypeConfiguration<PersonalCertificate>
    {
        public PersonalCertificateMap()
        {
            ToTable("PersonalCertificate");
            HasKey(c => c.Id);
            Property(u => u.CustomerId);
        }
    }
}
