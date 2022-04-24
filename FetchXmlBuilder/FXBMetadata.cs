using Cinteros.Xrm.FetchXmlBuilder.AppCode;
using McTools.Xrm.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Rappen.XRM.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Cinteros.Xrm.FetchXmlBuilder
{
    public partial class FetchXmlBuilder
    {
        internal Dictionary<string, EntityMetadata> entities;
        private static List<string> entityShitList = new List<string>();

        #region Internal Methods

        internal AttributeMetadata GetAttribute(string entityName, string attributeName)
        {
            if (entities != null && entities.ContainsKey(entityName) && entities[entityName].Attributes is AttributeMetadata[] attrs)
            {
                return attrs.FirstOrDefault(a => a.LogicalName == attributeName);
            }
            return null;
        }

        internal string GetAttributeDisplayName(string entityName, string attributeName)
        {
            if (!friendlyNames)
            {
                return attributeName;
            }
            var attribute = GetAttribute(entityName, attributeName);
            if (attribute != null)
            {
                attributeName = GetAttributeDisplayName(attribute);
            }
            return attributeName;
        }

        internal static string GetAttributeDisplayName(AttributeMetadata attribute)
        {
            string attributeName = attribute.LogicalName;
            if (friendlyNames)
            {
                if (attribute.DisplayName.UserLocalizedLabel != null)
                {
                    attributeName = attribute.DisplayName.UserLocalizedLabel.Label;
                }
                if (attributeName == attribute.LogicalName && attribute.DisplayName.LocalizedLabels.Count > 0)
                {
                    attributeName = attribute.DisplayName.LocalizedLabels[0].Label;
                }
            }
            return attributeName;
        }

        internal string GetEntityDisplayName(string entityName)
        {
            if (!friendlyNames)
            {
                return entityName;
            }
            if (entities != null && entities.ContainsKey(entityName))
            {
                entityName = GetEntityDisplayName(entities[entityName]);
            }
            return entityName;
        }

        internal static string GetEntityDisplayName(EntityMetadata entity)
        {
            var result = entity.LogicalName;
            if (friendlyNames)
            {
                if (entity.DisplayName.UserLocalizedLabel != null)
                {
                    result = entity.DisplayName.UserLocalizedLabel.Label;
                }
                if (result == entity.LogicalName && entity.DisplayName.LocalizedLabels.Count > 0)
                {
                    result = entity.DisplayName.LocalizedLabels[0].Label;
                }
            }
            return result;
        }

        internal AttributeMetadata[] GetAllAttribues(string entityName)
        {
            return entities?.ContainsKey(entityName) == true ? entities[entityName].Attributes : new AttributeMetadata[0];
        }

        internal AttributeMetadata[] GetDisplayAttributes(string entityName) => GetDisplayAttributes(entityName, settings.ShowAttributes);

        internal AttributeMetadata[] GetDisplayAttributes(string entityName, ShowMetaTypesAttribute selectattributes)
        {
            var result = new List<AttributeMetadata>();
            var attributes = GetAllAttribues(entityName);
            foreach (var attribute in attributes)
            {
                if (selectattributes.AlwaysPrimary && attribute.IsLogical != true && (attribute.IsPrimaryId == true || attribute.IsPrimaryName == true))
                {
                    result.Add(attribute);
                    continue;
                }
                if (selectattributes.AlwaysAddresses && attribute.IsLogical == true && attribute.AttributeType != AttributeTypeCode.Virtual && attribute.LogicalName.StartsWith("address"))
                {
                    result.Add(attribute);
                    continue;
                }
                if (!CheckMetadata(selectattributes.IsManaged, attribute.IsManaged)) { continue; }
                if (!CheckMetadata(selectattributes.IsCustom, attribute.IsCustomAttribute)) { continue; }
                if (!CheckMetadata(selectattributes.IsCustomizable, attribute.IsCustomizable)) { continue; }
                if (!CheckMetadata(selectattributes.IsValidForAdvancedFind, attribute.IsValidForAdvancedFind)) { continue; }
                if (!CheckMetadata(selectattributes.IsAuditEnabled, attribute.IsAuditEnabled)) { continue; }
                if (!CheckMetadata(selectattributes.IsLogical, attribute.IsLogical)) { continue; }
                if (!CheckMetadata(selectattributes.IsValidForRead, attribute.IsValidForRead)) { continue; }
                if (!CheckMetadata(selectattributes.IsValidForGrid, attribute.IsValidForGrid)) { continue; }
                if (!CheckMetadata(selectattributes.IsFiltered, attribute.IsFilterable)) { continue; }
                if (!CheckMetadata(selectattributes.IsRetrievable, attribute.IsRetrievable)) { continue; }
                if (!CheckMetadata(selectattributes.AttributeOf, !string.IsNullOrEmpty(attribute.AttributeOf))) { continue; }
                result.Add(attribute);
            }
            return result.ToArray();
        }

        internal string GetPrimaryIdAttribute(string entityName)
        {
            if (entities != null && entities.TryGetValue(entityName, out var entity))
            {
                return entity.PrimaryIdAttribute;
            }

            return null;
        }

        internal Dictionary<string, EntityMetadata> GetDisplayEntities() => GetDisplayEntities(settings.ShowEntities);

        internal Dictionary<string, EntityMetadata> GetDisplayEntities(ShowMetaTypesEntity selectentities)
        {
            var result = new Dictionary<string, EntityMetadata>();
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    if (!CheckMetadata(selectentities.IsManaged, entity.Value.IsManaged)) { continue; }
                    if (!CheckMetadata(selectentities.IsCustom, entity.Value.IsCustomEntity)) { continue; }
                    if (!CheckMetadata(selectentities.IsCustomizable, entity.Value.IsCustomizable)) { continue; }
                    if (!CheckMetadata(selectentities.IsValidForAdvancedFind, entity.Value.IsValidForAdvancedFind)) { continue; }
                    if (!CheckMetadata(selectentities.IsAuditEnabled, entity.Value.IsAuditEnabled)) { continue; }
                    if (!CheckMetadata(selectentities.IsLogical, entity.Value.IsLogicalEntity)) { continue; }
                    if (!CheckMetadata(selectentities.IsIntersect, entity.Value.IsIntersect)) { continue; }
                    if (!CheckMetadata(selectentities.IsActivity, entity.Value.IsActivity)) { continue; }
                    if (!CheckMetadata(selectentities.IsActivityParty, entity.Value.IsActivityParty)) { continue; }
                    if (!CheckMetadata(selectentities.Virtual, entity.Value.DataProviderId.HasValue)) { continue; }
                    if (!CheckMetadata(selectentities.Ownerships, entity.Value.OwnershipType)) { continue; }
                    result.Add(entity.Key, entity.Value);
                }
            }
            return result;
        }

        internal void LoadEntityDetails(string entityName, Action detailsLoaded, bool async = true, bool update = true)
        {
            if (detailsLoaded != null && !async)
            {
                throw new ArgumentException("Cannot handle call-back method for synchronous loading.", "detailsLoaded");
            }
            working = true;
            var name = GetEntityDisplayName(entityName);
            if (async)
            {
                WorkAsync(new WorkAsyncInfo($"Loading {name}...",
                    (eventargs) =>
                    {
                        eventargs.Result = MetadataExtensions.LoadEntityDetails(Service, entityName);
                    })
                {
                    PostWorkCallBack = (completedargs) =>
                    {
                        LoadEntityDetailsCompleted(entityName, completedargs.Error == null ? completedargs.Result as EntityMetadata : null, completedargs.Error, update);
                        if (completedargs.Error == null && detailsLoaded != null)
                        {
                            detailsLoaded();
                        }
                    }
                });
            }
            else
            {
                try
                {
                    var resp = MetadataExtensions.LoadEntityDetails(Service, entityName);
                    LoadEntityDetailsCompleted(entityName, resp, null, update);
                }
                catch (Exception e)
                {
                    LoadEntityDetailsCompleted(entityName, null, e, update);
                }
            }
        }

        internal EntityMetadata GetEntity(string entityname)
        {
            return entities?.Select(e => e.Value)?.FirstOrDefault(e => e.LogicalName == entityname);
        }

        #endregion Internal Methods

        #region Private Methods

        private static bool CheckMetadata(CheckState checkstate, bool? metafield)
        {
            if (metafield != null & metafield.HasValue)
            {
                switch (checkstate)
                {
                    case CheckState.Checked:
                        return metafield.Value == true;
                    case CheckState.Unchecked:
                        return metafield.Value == false;
                }
            }
            return true;
        }

        private static bool CheckMetadata(CheckState checkstate, BooleanManagedProperty metafield)
        {
            return CheckMetadata(checkstate, metafield?.Value);
        }

        private static bool CheckMetadata(OwnershipTypes[] options, OwnershipTypes? metafield)
        {
            if (options != null && options.Length > 0)
            {
                return metafield != null && options.Contains(metafield.Value);
            }
            return true;
        }

        private void LoadEntities(ConnectionDetail connectionDetail)
        {
            working = true;
            entities = null;
            entityShitList = new List<string>();
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading entities...",
                Work = (worker, eventargs) =>
                {
                    EnableControls(false);

                    if (settings.TryMetadataCache && connectionDetail.MetadataCacheLoader != null)
                    {   // Try cache metadata
                        if (connectionDetail.MetadataCache != null)
                        {   // Already cached
                            eventargs.Result = connectionDetail.MetadataCache;
                        }
                        else if (settings.WaitUntilMetadataLoaded)
                        {   // Load the cache until done
                            eventargs.Result = connectionDetail.MetadataCacheLoader.ConfigureAwait(false).GetAwaiter().GetResult()?.EntityMetadata;
                        }
                        else
                        {   // Load the cache in background
                            connectionDetail.MetadataCacheLoader.ContinueWith(task =>
                            {   // Waiting for loaded
                                SetAfterEntitiesLoaded(task.Result?.EntityMetadata);
                            });
                        }
                    }
                    else
                    {   // Load as usual, the old way
                        eventargs.Result = Service.LoadEntities(connectionDetail.OrganizationMajorVersion, connectionDetail.OrganizationMinorVersion);
                    }
                },
                PostWorkCallBack = (completedargs) =>
                {
                    if (completedargs.Error != null)
                    {
                        ShowErrorDialog(completedargs.Error, "Load Entities");
                    }
                    else
                    {
                        if (completedargs.Result is RetrieveMetadataChangesResponse meta)
                        {
                            SetAfterEntitiesLoaded(meta.EntityMetadata);
                        }
                        else if (completedargs.Result is IEnumerable<EntityMetadata> entitiesmeta)
                        {
                            SetAfterEntitiesLoaded(entitiesmeta);
                        }
                    }
                    SetAfterEntitiesLoaded(null);
                }
            });
        }

        private void SetAfterEntitiesLoaded(IEnumerable<EntityMetadata> newEntityMetadata)
        {
            MethodInvoker mi = delegate
            {
                if (newEntityMetadata != null)
                {
                    entities = newEntityMetadata?.ToDictionary(e => e.LogicalName);
                }
                UpdateLiveXML();
                working = false;
                EnableControls(true);
                dockControlBuilder?.ApplyCurrentSettings();
            };
            if (InvokeRequired)
            {
                Invoke(mi);
            }
            else
            {
                mi();
            }
        }

        private void LoadEntityDetailsCompleted(string entityName, EntityMetadata Result, Exception Error, bool update)
        {
            if (Error != null)
            {
                entityShitList.Add(entityName);
                ShowErrorDialog(Error, "Load attribute metadata");
            }
            else
            {
                if (Result != null)
                {
                    if (entities == null)
                    {
                        entities = new Dictionary<string, EntityMetadata>();
                    }
                    if (entities.ContainsKey(entityName))
                    {
                        entities[entityName] = Result;
                    }
                    else
                    {
                        entities.Add(entityName, Result);
                    }
                }
                else
                {
                    entityShitList.Add(entityName);
                    MessageBox.Show("Metadata not found for entity " + entityName, "Load attribute metadata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                working = false;

                if (update)
                {
                    dockControlBuilder.UpdateAllNode();
                    UpdateLiveXML();
                }
            }
            working = false;
        }

        #endregion Private Methods
    }
}
