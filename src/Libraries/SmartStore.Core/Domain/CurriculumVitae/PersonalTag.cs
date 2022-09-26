using System;
using System.Runtime.Serialization;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class PersonalTag : BaseEntity
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool Published { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
