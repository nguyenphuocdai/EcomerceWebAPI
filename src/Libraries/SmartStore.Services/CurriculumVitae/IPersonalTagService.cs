using System.Collections.Generic;
using SmartStore.Core;
using SmartStore.Core.Domain.CurriculumVitae;

namespace SmartStore.Services.CurriculumVitae
{
    public partial interface IPersonalTagService
    {
        IPagedList<PersonalTag> SearchTags(PersonalTagSearchQuery q);
        void InsertTag(PersonalTag client);
        bool IsExistTag(string tagName);
        PersonalTag GetTagById(int id);
        void UpdateTag(PersonalTag client);
        void DeleteTag(PersonalTag client);
        void DeleteTag(int id);
        IList<string> GetAllTagNames();
    }
}
