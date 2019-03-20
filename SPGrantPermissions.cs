using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace shareFolderWebService
{
    public class SPGrantPermission
    {
        public static string grantPermit( string siteUrl, string folderUrl, string userEmail)
        {
            try
            {
                //SPSite s = new SPSite(siteUrl);
                //SPWeb w = s.OpenWeb();
                int fileCount = 0;

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    using (SPSite site = new SPSite(siteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            //get folder
                            SPFolder folder = web.GetFolder(@"RSAs Library/" + folderUrl);

                            //get list of items in folder to grant permission to
                            SPFileCollection folderFiles = folder.Files;
                            //SPFolderCollection subFolders = folder.SubFolders;

                            //select User
                            SPPrincipal user = web.SiteUsers.GetByEmail(userEmail);

                            //define read role
                            SPRoleDefinition readRole = web.RoleDefinitions.GetById(1073741826);

                            web.AllowUnsafeUpdates = true;
                            //remove permissions on files
                            folder.Item.BreakRoleInheritance(true);

                            //create Role Assignment Ref
                            SPRoleAssignment roleAssignment = new SPRoleAssignment((SPPrincipal)user);

                            //grant user permission to the folder
                            roleAssignment.RoleDefinitionBindings.Add(readRole);

                            folder.Item.RoleAssignments.Add(roleAssignment);

                            //grant user permission to all files in folder
                            
                            foreach (SPFile file in folderFiles)
                            {
                                file.Item.BreakRoleInheritance(true);
                                file.Item.RoleAssignments.Add(roleAssignment);
                                fileCount++;
                            }
                            web.AllowUnsafeUpdates = false;
                        }
                    }                    
                });
                return String.Format("User {1} has been granted access to View {2} files in {0} Folder", fileCount, userEmail, folderUrl);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
