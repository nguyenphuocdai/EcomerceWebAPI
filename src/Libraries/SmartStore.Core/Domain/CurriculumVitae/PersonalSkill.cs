using System;
using System.Runtime.Serialization;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class PersonalSkill : BaseEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string SkillType { get; set; }
        [DataMember]
        public string SkillName { get; set; }
        [DataMember]
        public string SkillShortName { get; set; }
        [DataMember]
        public decimal SkillPercentage { get; set; }
        [DataMember]
        public decimal SkillStar { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
