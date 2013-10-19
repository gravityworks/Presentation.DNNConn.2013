using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace DNNService
{
    public class UserController : DnnApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public string CreateUser(UserInfoMap userInfo)
        {
            UserInfo userToAdd = new UserInfo();

            if (userInfo != null)
            {
                UserMembership membership = new UserMembership();
                membership.Password = !string.IsNullOrEmpty(userInfo.Password) ? userInfo.Password : DotNetNuke.Entities.Users.UserController.GeneratePassword(8);

                userToAdd.AffiliateID = userInfo.AffiliateID;
                userToAdd.DisplayName = userInfo.DisplayName;
                userToAdd.Email = userInfo.Username;
                userToAdd.FirstName = userInfo.FirstName;
                userToAdd.IsDeleted = userInfo.IsDeleted;
                userToAdd.IsSuperUser = userInfo.IsSuperUser;
                userToAdd.LastIPAddress = userInfo.LastIPAddress;
                userToAdd.LastName = userInfo.LastName;
                userToAdd.PortalID = userInfo.PortalID;
                userToAdd.UserID = userInfo.UserID;
                userToAdd.Username = userInfo.Username;
                userToAdd.FullName = userInfo.FullName;
                userToAdd.Membership = membership;
                UserCreateStatus createStatus = DotNetNuke.Entities.Users.UserController.CreateUser(ref userToAdd);

                if (createStatus == UserCreateStatus.Success)
                {
                    var roles = userToAdd.Roles.Union(userInfo.Roles);
                    foreach (string roleName in roles)
                    {
                        AddUserToRole(userToAdd.UserID, roleName);    
                    }
                }
            }

            var response = userToAdd.UserID.ToString();
            return response;
        }

        private void AddUserToRole(int userId, string role)
        {

            RoleController roleController = new RoleController();
            RoleInfo roleToAdd = roleController.GetRoleByName(0, role);

            if (roleToAdd == null)
            {
                return;
            }

            roleController.AddUserRole(0, userId, roleToAdd.RoleID, System.DateTime.Now.AddDays(-1), DotNetNuke.Common.Utilities.Null.NullDate);
        }

        [AllowAnonymous]
        [HttpPut]
        public string EditUser(UserInfoMap userInfo)
        {
            UserInfo userToEdit = DotNetNuke.Entities.Users.UserController.GetUserById(Constants.PORTAL_ID, userInfo.UserID);

            if (userToEdit != null)
            {
                userToEdit.AffiliateID = userInfo.AffiliateID;
                userToEdit.DisplayName = userInfo.DisplayName;
                userToEdit.FirstName = userInfo.FirstName;
                userToEdit.IsDeleted = userInfo.IsDeleted;
                userToEdit.IsSuperUser = userInfo.IsSuperUser;
                userToEdit.LastName = userInfo.LastName;
                userToEdit.PortalID = userInfo.PortalID;
                userToEdit.FullName = userInfo.FullName;

                if (!string.IsNullOrEmpty(userInfo.Password))
                    userToEdit.Membership.Password = userInfo.Password;

                DotNetNuke.Entities.Users.UserController.UpdateUser(Constants.PORTAL_ID, userToEdit);
            }


            RoleController roleController = new RoleController();
            if (userInfo.Active)
            {
                AddUserToRole(userToEdit.UserID, "Member");
            }
            else
            {
                roleController.DeleteUserRole(0, userToEdit.UserID, roleController.GetRoleByName(0, "Member").RoleID);
            }
            if (!userInfo.Roles.Contains("Coordinator"))
            {
                roleController.DeleteUserRole(0, userToEdit.UserID, roleController.GetRoleByName(0, "Coordinator").RoleID);
            }
            if (!userInfo.Roles.Contains("Host"))
            {
                roleController.DeleteUserRole(0, userToEdit.UserID, roleController.GetRoleByName(0, "Host").RoleID);
            }
            if (!userInfo.Roles.Contains("XXXAdmin"))
            {
                roleController.DeleteUserRole(0, userToEdit.UserID, roleController.GetRoleByName(0, "XXX").RoleID);
            }
            foreach (string roleName in userInfo.Roles)
            {
                AddUserToRole(userToEdit.UserID, roleName);
            }

            // TODO: Deal with Return

            string success = "";
            return success.ToString();
        }

       // [AllowAnonymous]
        [DnnAuthorize]
        //[HttpGet]
        public HttpResponseMessage GetUserByName(int portalId, string userId)
        {

            UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserByName(portalId, userId);

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.OK, "");

            var userInfoMap = new UserInfoMap(user);
            return Request.CreateResponse(HttpStatusCode.OK, userInfoMap);
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetUser(int portalId, int userId)
        {
            UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserById(portalId, userId);

            if (user == null)
                return Request.CreateResponse(HttpStatusCode.OK, "");

            var userInfoMap = new UserInfoMap(user);
            return Request.CreateResponse(HttpStatusCode.OK, userInfoMap);
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetUsersFromIdList(int portalId, string userId)
        {
            if (string.IsNullOrEmpty(userId)) 
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            var userIds = userId.Split(new[] { '|' }).Select(s => Convert.ToInt32(s));
            ICollection<UserInfoMap> userInfoMaps = new HashSet<UserInfoMap>();

            foreach (var id in userIds)
            {
                UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserById(portalId, id);

                 userInfoMaps.Add(new UserInfoMap(user));
            }

            return Request.CreateResponse(HttpStatusCode.OK, userInfoMaps);
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetUsers(int portalId)
        {
            var allUsers = DotNetNuke.Entities.Users.UserController.GetUsers(portalId);
            var allMappedUsers = new List<UserInfoMap>();

            foreach (UserInfo item in allUsers)
            {
                allMappedUsers.Add(new UserInfoMap(item));
            }

            return Request.CreateResponse(HttpStatusCode.OK, allMappedUsers);
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ChangePassword(int portalId, int userId, string password, string newValue)
        {
            UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserById(portalId, userId);

            var result = new DotNetNuke.Security.Membership.AspNetMembershipProvider().ChangePassword(user, password, newValue);
            DotNetNuke.Entities.Users.UserController.UpdateUser(portalId, user);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage ChangeUserName(int portalId, int userId, string password, string newValue)
        {
            UserInfo user = DotNetNuke.Entities.Users.UserController.GetUserById(portalId, userId);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }
            if (!System.Web.Security.Membership.ValidateUser(user.Username, password))
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }
            if (DotNetNuke.Entities.Users.UserController.GetUserByName(newValue) != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }

            using (var connection = new SqlConnection(WebConfigurationManager.AppSettings["SiteSqlServer"].ToString()))
            {
                connection.Open();
                var cmd = new SqlCommand(" UPDATE [XXXX].[dbo].[Users] SET Username = '" + newValue + "', Email = '" + newValue.ToLower() + "' WHERE UserID = " + userId +
                                         " UPDATE [XXXX].[dbo].[aspnet_Membership] SET Email = '" + newValue + "', LoweredEmail = '" + newValue.ToLower() + "' WHERE UserId = (SELECT TOP 1 UserId FROM [XXXX].[dbo].[aspnet_Users] WHERE UserName = '" + user.Username + "') " +
                                         " UPDATE [XXXXX].[dbo].[aspnet_Users] SET UserName = '" + newValue + "', LoweredUserName = '" + newValue.ToLower() + "' WHERE UserName = '" + user.Username + "'",
                                         connection);
                var affected = cmd.ExecuteNonQuery();
                connection.Close();
                DataCache.ClearUserCache(0, user.Username);
                DataCache.ClearCache();
            }

            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
    }
}
