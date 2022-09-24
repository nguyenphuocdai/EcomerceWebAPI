using System;
using System.Runtime.Serialization;
using SmartStore.Core;

namespace SmartStore.Core.Domain.CurriculumVitae
{
    [DataContract]
    public partial class Personal : BaseEntity
    {
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string AliasName { get; set; }
        [DataMember]
        public DateTime? BirthDate { get; set; }
        [DataMember]
        public string Residence { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string Introduce { get; set; }
        [DataMember]
        public string Avatar { get; set; }
        [DataMember]
        public string CV { get; set; }
        [DataMember]
        public string IntroduceTitle00 { get; set; }
        [DataMember]
        public string IntroduceContent00 { get; set; }
        [DataMember]
        public string IntroduceTitle01 { get; set; }
        [DataMember]
        public string IntroduceContent01 { get; set; }
        [DataMember]
        public string IntroduceTitle02 { get; set; }
        [DataMember]
        public string IntroduceContent02 { get; set; }
        [DataMember]
        public string FunFactTitle00 { get; set; }
        [DataMember]
        public string FunFactContent00 { get; set; }
        [DataMember]
        public string FunFactTitle01 { get; set; }
        [DataMember]
        public string FunFactContent01 { get; set; }
        [DataMember]
        public string FunFactTitle02 { get; set; }
        [DataMember]
        public string FunFactContent02 { get; set; }
        [DataMember]
        public string FunFactTitle03 { get; set; }
        [DataMember]
        public string FunFactContent03 { get; set; }
        [DataMember]
        public bool IsFreelanceAvailable { get; set; }
        [DataMember]
        public bool IsOpeningJob { get; set; }
        [DataMember]
        public bool IsDownload { get; set; }
        [DataMember]
        public string FacebookLink { get; set; }
        [DataMember]
        public string YoutubeLink { get; set; }
        [DataMember]
        public string TelegramLink { get; set; }
        [DataMember]
        public string InstagramLink { get; set; }
        [DataMember]
        public string TwitterLink { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public DateTime? ModifiedDate { get; set; }
    }
}
