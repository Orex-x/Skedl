using Skedl.DataStorage.Models;
using Skedl.DataStorage.Models.ApiModels;

namespace Skedl.DataStorage.Services.Data.DataGroup;

public interface IGroupService
{
    public void UpdateOrCreateGroup(BaseLink group);
    public void UpdateOrCreateGroups(List<BaseLink> group);
}