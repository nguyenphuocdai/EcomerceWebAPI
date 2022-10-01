using System.Collections.Generic;
using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalSkillService
    {
        IPagedList<PersonalSkill> SearchPersonalSkills(PersonalSkillSearchQuery q);
        void InsertPersonalSkill(PersonalSkill personal);
        bool IsExistPersonalSkill(int customerId, string skillName);
        PersonalSkill GetSkillById(int id);
        void UpdateSkill(PersonalSkill personalSkill);

        void DeleteSkill(PersonalSkill personalSkill);
        void DeleteSkill(int id);
        List<PersonalSkill> GetSkillByCustomerId(int id);
    }
}
