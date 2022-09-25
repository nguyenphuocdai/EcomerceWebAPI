using System;
using System.Runtime.Serialization;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class PersonalResume : BaseEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string ResumeType { get; set; }
        [DataMember]
        public string ResumeShort { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public DateTime? From { get; set; }
        [DataMember]
        public DateTime? To { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
