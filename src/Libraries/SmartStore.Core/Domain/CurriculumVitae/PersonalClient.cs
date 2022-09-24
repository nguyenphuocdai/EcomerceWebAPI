using System;
using System.Runtime.Serialization;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class PersonalClient : BaseEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string ClientName { get; set; }
        [DataMember]
        public string ClientDescription { get; set; }
        [DataMember]
        public int? ClientImageId { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
