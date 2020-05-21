using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.AppCode
{
    public static class ConnectionExtensions
    {
        public static string GetFullWebApplicationUrl(this ConnectionDetail connectiondetail)
        {
            var url = connectiondetail.WebApplicationUrl;
            if (string.IsNullOrEmpty(url))
            {
                url = connectiondetail.ServerName;
            }
            if (!url.ToLower().StartsWith("http"))
            {
                url = string.Concat("http://", url);
            }
            var uri = new Uri(url);
            if (!uri.Host.EndsWith(".dynamics.com"))
            {
                if (string.IsNullOrEmpty(uri.AbsolutePath.Trim('/')))
                {
                    uri = new Uri(uri, connectiondetail.Organization);
                }
            }
            return uri.ToString();
        }

        public static string GetWebApiServiceUrl(this ConnectionDetail connectiondetail)
        {
            var url = new Uri(new Uri(connectiondetail.GetFullWebApplicationUrl()), $"api/data/v{connectiondetail.OrganizationMajorVersion}.{connectiondetail.OrganizationMinorVersion}");
            return url.ToString();
        }

        public static string GetEntityUrl(this ConnectionDetail connectiondetail, Entity entity)
        {
            if (entity == null)
            {
                return string.Empty;
            }
            var entref = entity.ToEntityReference();
            switch (entref.LogicalName)
            {
                case "activitypointer":
                    if (!entity.Contains("activitytypecode"))
                    {
                        MessageBox.Show("To open records of type activitypointer, attribute 'activitytypecode' must be included in the query.", "Open Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entref.LogicalName = string.Empty;
                    }
                    else
                    {
                        entref.LogicalName = entity["activitytypecode"].ToString();
                    }
                    break;
                case "activityparty":
                    if (!entity.Contains("partyid"))
                    {
                        MessageBox.Show("To open records of type activityparty, attribute 'partyid' must be included in the query.", "Open Record", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        entref.LogicalName = string.Empty;
                    }
                    else
                    {
                        var party = (EntityReference)entity["partyid"];
                        entref.LogicalName = party.LogicalName;
                        entref.Id = party.Id;
                    }
                    break;
            }
            return connectiondetail.GetEntityReferenceUrl(entref);
        }

        public static string GetEntityReferenceUrl(this ConnectionDetail connectiondetail, EntityReference entref)
        {
            if (string.IsNullOrWhiteSpace(entref?.LogicalName) || Guid.Empty.Equals(entref.Id))
            {
                return string.Empty;
            }
            var url = connectiondetail.GetFullWebApplicationUrl();
            url = string.Concat(url,
                url.EndsWith("/") ? "" : "/",
                "main.aspx?etn=",
                entref.LogicalName,
                "&pagetype=entityrecord&id=",
                entref.Id.ToString());
            return url;
        }
    }
}
