using DotNetNuke.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNNService
{
    //TODO: this is a common class, need to abstact to a shared DLL
    public class UserInfoMap
    {
        public int Id { get; set; }
        public int AffiliateID { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSuperUser { get; set; }
        public string LastIPAddress { get; set; }
        public string LastName { get; set; }
        public int PortalID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public string Password { get; set; }
        public UserInfoMap() { }
        public bool Active { get; set; }

        public UserInfoMap(UserInfo userInfo)
        {
            this.Id = userInfo.UserID;
            this.AffiliateID = userInfo.AffiliateID;
            this.DisplayName = userInfo.DisplayName;
            this.Email = userInfo.Email;
            this.FirstName = userInfo.FirstName;
            this.IsDeleted = userInfo.IsDeleted;
            this.IsSuperUser = userInfo.IsSuperUser;
            this.LastIPAddress = userInfo.LastIPAddress;
            this.LastName = userInfo.LastName;
            this.PortalID = userInfo.PortalID;
            this.UserID = userInfo.UserID;
            this.Username =userInfo.Username;
            this.FullName = userInfo.FullName;
            this.Roles = userInfo.Roles.ToList();
            this.Active = this.Roles.Contains("Member");
        }
    }
}
