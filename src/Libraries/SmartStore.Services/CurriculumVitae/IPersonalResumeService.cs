using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalResumeService
    {
        IPagedList<PersonalResume> SearchResumes(PersonalResumeSearchQuery q);
        void InsertResume(PersonalResume resume);
        PersonalResume GetResumeById(int id);
        void UpdateResume(PersonalResume client);
        void DeleteResume(PersonalResume client);
        void DeleteResume(int id);
    }
}
