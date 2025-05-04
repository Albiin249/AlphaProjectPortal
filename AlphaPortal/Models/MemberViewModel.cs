using Domain.Models;

namespace AlphaPortal.Models;

public class MemberViewModel
{
    public IEnumerable<User> User { get; set; } = [];
    public AddMemberViewModel AddMemberViewModel { get; set; } = new();
    public EditMemberViewModel EditMemberViewModel { get; set; } = new();

    public IEnumerable<MemberDisplayViewModel> Users { get; set; } = [];

}
