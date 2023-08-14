using Skedl.DataStorage.Models;
using Skedl.DataStorage.Models.ApiModels;

namespace Skedl.DataStorage.Services.Data.DataGroup;

public class GroupService : IGroupService
{
    private readonly DatabaseContext _context;
    
    public GroupService(DatabaseContext context)
    {
        _context = context;
    }
    
    public void UpdateOrCreateGroup(BaseLink link)
    {
        var group = _context.Groups.FirstOrDefault(x => x.Name == link.Name);
        
        if (group != null)
        {
            group.Link = group.Link;
            _context.Groups.Update(group);
        }
        else
        {
            group = new Group()
            {
                Name = link.Name,
                Link = link.Link
            };

            _context.Groups.Add(group);
        }
    }

    public void UpdateOrCreateGroups(List<BaseLink> baseLinks)
    {
        foreach (var baseLink in baseLinks)
        {
            UpdateOrCreateGroup(baseLink);
        }
    }
}