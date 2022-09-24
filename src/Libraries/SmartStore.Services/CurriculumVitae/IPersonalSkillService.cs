using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalSkillService
    {
        IPagedList<PersonalSkill> SearchPersonalSkills(PersonalSkillSearchQuery q);
        void InsertPersonalSkill(PersonalSkill personal);
        bool IsExistPersonalSkill(int customerId, string skillName);
        PersonalSkill GetPersonalSkillById(int id);
        void UpdateSkill(PersonalSkill personalSkill);

        void DeleteSkill(PersonalSkill personalSkill);
        void DeleteSkill(int id);
    }
}
