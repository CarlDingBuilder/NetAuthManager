using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetAuthManager.Application.Sys.Results.Users;

public class GetRoleUsersItem
{
    public string UserAccount { get; set; }
    public string UserDisplayName { get; set; }
}

public class GetRoleUsersItemByRole
{
    public string UserAccount { get; set; }
    public string UserDisplayName { get; set; }
    public string RoleCode { get; set; }
}