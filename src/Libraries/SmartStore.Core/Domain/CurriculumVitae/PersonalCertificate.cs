using System;
using System.Runtime.Serialization;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class PersonalCertificate : BaseEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int? ImageId { get; set; }
        [DataMember]
        public DateTime? CertificateDate { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
