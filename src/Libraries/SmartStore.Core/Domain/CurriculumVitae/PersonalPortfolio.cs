using System;
using System.Runtime.Serialization;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class PersonalPortfolio : BaseEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string PortfolioName { get; set; }
        [DataMember]
        public string PortfolioUrl { get; set; }
        [DataMember]
        public string ShortMessage { get; set; }
        [DataMember]
        public DateTime? From { get; set; }
        [DataMember]
        public DateTime? To { get; set; }
        [DataMember]
        public DateTime? Published { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Tag { get; set; }
        [DataMember]
        public string ShareLinkFacebook { get; set; }
        [DataMember]
        public string ShareLinkTwitter { get; set; }
        [DataMember]
        public string ShareLinkInstagram { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
