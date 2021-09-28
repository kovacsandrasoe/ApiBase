using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiBase.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public IList<string> Roles { get; set; }
    }
}
