using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Model.ContentLinks;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class AccountHelper
    {
        public static bool CreateUserAccount(string username, string password, string email, string firstName
           , string lastName, string passwordQuestion = "", string passwordAnswer = "", bool isApproved = true
           , object providerUserKey = null, bool isBackendUser = false, string roleName = "")
        {
            bool isSuccessful = false;
            UserManager userManager = UserManager.GetManager();
            RoleManager roleManager = RoleManager.GetManager();
            try
            {
                UserProfileManager profileManager = UserProfileManager.GetManager();

                userManager.Provider.SuppressSecurityChecks = true;
                MembershipCreateStatus status;

                User user = userManager.CreateUser(username, password, email, passwordQuestion, passwordAnswer,
                    isApproved, providerUserKey, out status);

                if (status == MembershipCreateStatus.Success)
                {
                    user.IsBackendUser = isBackendUser;
                    SitefinityProfile sfProfile = profileManager.CreateProfile(user, Guid.NewGuid(), typeof(SitefinityProfile)) as SitefinityProfile;

                    if (sfProfile != null)
                    {
                        sfProfile.FirstName = firstName;
                        sfProfile.LastName = lastName;
                        userManager.SaveChanges();
                        profileManager.RecompileItemUrls(sfProfile);
                        profileManager.SaveChanges();
                    }

                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        //Add user to Role
                        roleManager.Provider.SuppressSecurityChecks = true;
                        Role customerssRole = roleManager.GetRole(roleName);
                        roleManager.AddUserToRole(user, customerssRole);
                        roleManager.SaveChanges();
                    }

                    isSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                roleManager.Provider.SuppressSecurityChecks = false;
                userManager.Provider.SuppressSecurityChecks = false;
            }

            return isSuccessful;
        }

        public static bool ValidateRoleByUsername(string username, bool isDefaultRole, string roleName)
        {
            RoleManager roleManager = (isDefaultRole) ? RoleManager.GetManager(SecurityManager.ApplicationRolesProviderName)
                : RoleManager.GetManager();
            UserManager userManager = UserManager.GetManager();
            if (!userManager.UserExists(username))
                throw new Exception("Username does not exist!");
            if (!roleManager.RoleExists(roleName))
                throw new Exception("Role does not exist!");

            User user = userManager.GetUser(username);
            return roleManager.IsUserInRole(user.Id, roleName);
        }

        //public static bool ValidateRoleByIdentity(ClaimsIdentityProxy identity, bool isDefaultRole, string roleName)
        //{
        //    RoleManager roleManager = (isDefaultRole) ? RoleManager.GetManager(SecurityManager.ApplicationRolesProviderName)
        //        : RoleManager.GetManager();
        //    if (!roleManager.RoleExists(roleName))
        //        throw new Exception("Role does not exist!");

        //    return identity.Roles.Any(x => x.Name.ToLower() == roleName.ToLower()
        //        && x.Provider == roleManager.Provider.Name);
        //}

        public static bool ChangePassword(string username, string oldPassword, string newPassword, UserManager userManager = null)
        {
            bool isSuccessful = false;
            try
            {
                if (userManager == null)
                    userManager = UserManager.GetManager();

                userManager.Provider.SuppressSecurityChecks = true;
                if (oldPassword != newPassword)
                    userManager.ChangePassword(username, oldPassword, newPassword);
                userManager.SaveChanges();
                userManager.Provider.SuppressSecurityChecks = false;
                isSuccessful = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccessful;
        }

        //Remember to set enablePasswordReset = true in CMS settings first before consuming this method
        public static string ForgotPassword(string username)
        {
            UserManager userManager = UserManager.GetManager();
            User user = userManager.GetUser(username);
            string newPassword = user.ResetPassword();
            userManager.SaveChanges();
            return newPassword;
        }

        public static string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// SetupCode for Google Authentication
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool HasSetupCode(string userName)
        {
            UserProfileManager profileManager = UserProfileManager.GetManager();
            UserManager userManager = UserManager.GetManager();
            User user = userManager.GetUser(userName);
            SitefinityProfile profile = null;

            if (user != null)
            {
                profile = profileManager.GetUserProfile<SitefinityProfile>(user);

                if (profile.GetValue("SetupCode") != null && !string.IsNullOrEmpty(profile.GetValue("SetupCode").ToString()))
                    return true;
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="filepath">sample format: ~\App_Data\</param>
        /// <param name="filename">must include file extension</param>
        /// <returns></returns>
        public static List<string> GetPasswordHistoryByUsername(string username, string filepath, string filename)
        {
            if (!filepath.StartsWith("~"))
                filepath = "~" + filepath;
            if (!filepath.EndsWith(@"\"))
                filepath += @"\";
            string path = System.Web.HttpContext.Current.Server.MapPath(filepath + filename);
            string[] lines = System.IO.File.ReadAllLines(path);
            string TxtItem = string.Empty;
            List<string> passwordHistoryList = new List<string>();

            string pwRecord = lines.ToList().Find(x => x.Contains(username));

            if (!string.IsNullOrEmpty(pwRecord))
            {
                string _passwords = pwRecord.Substring(pwRecord.IndexOf(':') + 1);
                passwordHistoryList = _passwords.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            return passwordHistoryList;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="filepath">sample format: ~\App_Data\</param>
        /// <param name="filename">must include file extension</param>
        /// <param name="passwordHistoryCount">number of old password to be stored</param>
        public static void CreateUserPasswordHistory(string username, string password, string filepath, string filename, int passwordHistoryCount)
        {
            if (!filepath.StartsWith("~"))
                filepath = "~" + filepath;
            if (!filepath.EndsWith(@"\"))
                filepath += @"\";
            string path = System.Web.HttpContext.Current.Server.MapPath(filepath + filename);
            string[] lines = System.IO.File.ReadAllLines(path);
            List<string> passwordHistoryList = GetPasswordHistoryByUsername(username, filepath, filename);
            if (passwordHistoryList.Any())
            {
                //update record
                List<string> modifiedLines = new List<string>();
                while (passwordHistoryList.Count >= passwordHistoryCount)
                    passwordHistoryList.RemoveAt(0);
                string updatedHistory = string.Join(",", passwordHistoryList) + "," + Encrypt(password);
                foreach (string item in lines)
                {
                    string[] fields = item.Split(':');
                    string txtItem = fields[0] + ":";
                    txtItem += (fields[0] == username) ? updatedHistory : fields[1];
                    modifiedLines.Add(txtItem);
                }
                File.WriteAllLines(path, modifiedLines);
            }
            else
            {
                //create record
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(username + ":" + password + ",");
                tw.Close();
            }
        }

        public static string GetUserProfileImageUrl(Guid userId)
        {
            UserManager userManager = UserManager.GetManager();

            User user = userManager.GetUser(userId);
            if (user == null)
                throw new Exception(string.Format("UserId: {0} does not exist!", userId));

            UserProfileManager profileManager = UserProfileManager.GetManager();
            SitefinityProfile profile = profileManager.GetUserProfile<SitefinityProfile>(user);

            if (profile == null)
                throw new Exception("Profile does not exist!");

            if (profile.Avatar == null)
                return string.Empty;

            LibrariesManager librariesManager = LibrariesManager.GetManager();
            ContentLink avatarLink = profile.Avatar;
            Guid imageId = avatarLink.ChildItemId;
            Image avatar = librariesManager.GetImage(imageId);
            return avatar.MediaUrl;
        }
    }
}