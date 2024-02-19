using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Rappen.XRM.Helpers.Extensions;
using Rappen.XTB.FetchXmlBuilder.Settings;
using Rappen.XTB.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using XrmToolBox.Extensibility;

namespace Rappen.XTB.FetchXmlBuilder
{
    public partial class FetchXmlBuilder
    {
        internal List<EntityMetadata> entities;
        private static List<string> entityShitList = new List<string>();
        internal List<Entity> solutionentities;
        internal List<Guid> solutionattributes;

        #region Internal Methods

        internal AttributeMetadata GetAttribute(string entityName, string attributeName)
        {
            if (GetEntity(entityName)?.Attributes is AttributeMetadata[] attrs)
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
            if (GetEntity(entityName) is EntityMetadata meta)
            {
                entityName = GetEntityDisplayName(meta);
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

        internal string GetContitionValue(string entityName, string attributeName, string value)
        {
            if (friendlyNames &&
                GetAttribute(entityName, attributeName) is AttributeMetadata attribute)
            {
                if (attribute is EnumAttributeMetadata enummeta &&
                    int.TryParse(value, out var optionsetvalue) &&
                    enummeta.OptionSet.Options.FirstOrDefault(o => o.Value == optionsetvalue) is OptionMetadata valuemeta &&
                    valuemeta.Label?.UserLocalizedLabel?.Label is string label &&
                    !string.IsNullOrEmpty(label))
                {
                    return label;
                }
                if (attribute is BooleanAttributeMetadata boolmeta &&
                    int.TryParse(value, out var boolvalue) &&
                    (boolvalue == 0 || boolvalue == 1) &&
                    boolmeta.OptionSet?.FalseOption?.Label?.UserLocalizedLabel?.Label is string falselabel &&
                    boolmeta.OptionSet?.TrueOption?.Label?.UserLocalizedLabel?.Label is string truelabel &&
                    !string.IsNullOrEmpty(falselabel) &&
                    !string.IsNullOrEmpty(truelabel))
                {
                    if (boolvalue == 0)
                    {
                        return falselabel;
                    }
                    if (boolvalue == 1)
                    {
                        return truelabel;
                    }
                }
            }
            return value;
        }

        internal IEnumerable<AttributeMetadata> GetAllAttribues(string entityName)
        {
            return GetEntity(entityName)?.Attributes ?? new AttributeMetadata[0];
        }

        internal IEnumerable<AttributeMetadata> GetDisplayAttributes(string entityName) => GetDisplayAttributes(entityName, connectionsettings.FilterSetting, connectionsettings.ShowAttributes);

        internal IEnumerable<AttributeMetadata> GetDisplayAttributes(string entityName, FilterSetting selectedfilter, ShowMetaTypesAttribute selectattributes)
        {
            var result = new List<AttributeMetadata>();
            var entity = GetEntity(entityName);
            if (entity == null)
            {
                return result;
            }
            if (solutionentities == null || solutionentities.Count == 0)
            {
                LoadSolutionsComponents(selectedfilter);
            }
            var includeall = solutionentities
                .Where(se => se.GetAttributeValue<Guid>("objectid").Equals(entity.MetadataId))
                .Any(se => se.GetAttributeValue<OptionSetValue>("rootcomponentbehavior").Value == 0);
            var attributes = GetAllAttribues(entityName);
            foreach (var attribute in attributes)
            {
                if (selectedfilter.AlwaysPrimary && attribute.IsLogical != true && (attribute.IsPrimaryId == true || attribute.IsPrimaryName == true))
                {
                    result.Add(attribute);
                    continue;
                }
                if (selectedfilter.AlwaysAddresses && attribute.IsLogical == true && attribute.AttributeType != AttributeTypeCode.Virtual && attribute.LogicalName.StartsWith("address"))
                {
                    result.Add(attribute);
                    continue;
                }
                if (!selectedfilter.ShowAllSolutions && !includeall &&
                    !solutionattributes.Contains((Guid)attribute.MetadataId))
                {
                    continue;
                }
                if (selectedfilter.FilterByMetadata)
                {
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
                    if (!CheckMetadata(selectattributes.CalculationOf, attribute is MoneyAttributeMetadata money ? !string.IsNullOrEmpty(money.CalculationOf) : false)) { continue; }
                }
                result.Add(attribute);
            }
            return result;
        }

        internal string GetPrimaryIdAttribute(string entityName)
        {
            return GetEntity(entityName)?.PrimaryIdAttribute;
        }

        internal List<EntityMetadata> GetDisplayEntities() => GetDisplayEntities(connectionsettings.FilterSetting, connectionsettings.ShowEntities);

        internal List<EntityMetadata> GetDisplayEntities(FilterSetting selectedfilter, ShowMetaTypesEntity selectentities)
        {
            var result = new List<EntityMetadata>();
            if (entities == null || selectedfilter.NoneEntitiesSelected)
            {
                return result;
            }
            if (solutionentities == null || solutionentities.Count == 0)
            {
                LoadSolutionsComponents(selectedfilter);
            }
            foreach (var entity in entities.Where(e => selectedfilter.ShowAllSolutions || solutionentities.Select(se => se["objectid"]).Contains((Guid)e.MetadataId)))
            {
                if (selectedfilter.FilterByMetadata)
                {
                    if (!CheckMetadata(selectentities.IsManaged, entity.IsManaged)) { continue; }
                    if (!CheckMetadata(selectentities.IsCustom, entity.IsCustomEntity)) { continue; }
                    if (!CheckMetadata(selectentities.IsCustomizable, entity.IsCustomizable)) { continue; }
                    if (!CheckMetadata(selectentities.IsValidForAdvancedFind, entity.IsValidForAdvancedFind)) { continue; }
                    if (!CheckMetadata(selectentities.IsAuditEnabled, entity.IsAuditEnabled)) { continue; }
                    if (!CheckMetadata(selectentities.IsLogical, entity.IsLogicalEntity)) { continue; }
                    if (!CheckMetadata(selectentities.IsIntersect, entity.IsIntersect)) { continue; }
                    if (!CheckMetadata(selectentities.IsActivity, entity.IsActivity)) { continue; }
                    if (!CheckMetadata(selectentities.IsActivityParty, entity.IsActivityParty)) { continue; }
                    if (!CheckMetadata(selectentities.Virtual, entity.DataProviderId.HasValue)) { continue; }
                    if (!CheckMetadata(selectentities.Ownerships, entity.OwnershipType)) { continue; }
                }
                result.Add(entity);
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
                        eventargs.Result = XRM.Helpers.Extensions.MetadataExtensions.LoadEntityDetails(Service, entityName);
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
                    var resp = XRM.Helpers.Extensions.MetadataExtensions.LoadEntityDetails(Service, entityName);
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
            return entities?.FirstOrDefault(e => e.LogicalName.Equals(entityname));
        }

        internal EntityMetadata GetEntity(int objecttypecode)
        {
            return entities?.FirstOrDefault(e => e.ObjectTypeCode.Equals(objecttypecode));
        }

        internal bool NeedToLoadEntity(string entityName)
        {
            return
                !string.IsNullOrEmpty(entityName) &&
                !entityShitList.Contains(entityName) &&
                Service != null &&
                GetEntity(entityName)?.Attributes == null;
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

        private void LoadEntities(bool forcereload = false)
        {
            if (SendMessageToStatusBar != null)
            {
                SendMessageToStatusBar(this, new XrmToolBox.Extensibility.Args.StatusBarMessageEventArgs($"{(forcereload ? "Reloading" : "Loading")} all entities..."));
            }
            working = true;
            entities = null;
            entityShitList = new List<string>();
            this.GetAllEntityMetadatas(SetAfterEntitiesLoaded, settings.TryMetadataCache, settings.WaitUntilMetadataLoaded || forcereload, forcereload);
        }

        private void SetAfterEntitiesLoaded(IEnumerable<EntityMetadata> newEntityMetadata, bool manually)
        {
            MethodInvoker mi = delegate
            {
                if (newEntityMetadata != null)
                {
                    entities = newEntityMetadata.ToList();
                    if (SendMessageToStatusBar != null)
                    {
                        SendMessageToStatusBar(this, new XrmToolBox.Extensibility.Args.StatusBarMessageEventArgs($"{(manually ? "Reloaded" : "Loaded")} {entities.Count} entities."));
                    }
                }
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
                if (!entityShitList.Contains(entityName))
                {
                    entityShitList.Add(entityName);
                }
                if (Error is FaultException<OrganizationServiceFault> srcexc &&
                    srcexc.Detail is OrganizationServiceFault orgerr &&
                    orgerr.ErrorCode == -2147220969)
                {
                    SendMessageToStatusBar(this, new XrmToolBox.Extensibility.Args.StatusBarMessageEventArgs($"Entity {entityName} not found in the database."));
                }
                else
                {
                    ShowErrorDialog(Error, "Load attribute metadata");
                }
            }
            else
            {
                if (Result != null)
                {
                    if (entities == null)
                    {
                        entities = new List<EntityMetadata>();
                    }
                    if (GetEntity(entityName) is EntityMetadata entity)
                    {
                        entities.Remove(entity);
                    }
                    entities.Add(Result);
                    SendMessageToStatusBar(this, new XrmToolBox.Extensibility.Args.StatusBarMessageEventArgs($"Entity {entityName} is now loaded."));
                }
                else
                {
                    if (!entityShitList.Contains(entityName))
                    {
                        entityShitList.Add(entityName);
                    }
                    SendMessageToStatusBar(this, new XrmToolBox.Extensibility.Args.StatusBarMessageEventArgs($"Entity {entityName} can't be found."));
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

        private void LoadSolutionsComponents(FilterSetting selectedfilter)
        {
            if (Service == null || selectedfilter.NoneEntitiesSelected || selectedfilter.ShowAllSolutions)
            {
                solutionentities = new List<Entity>();
                solutionattributes = new List<Guid>();
                return;
            }
            solutionentities = null;
            solutionattributes = null;
            var query = new QueryExpression("solutioncomponent");
            query.ColumnSet.AddColumns("objectid", "solutioncomponentid", "rootcomponentbehavior", "rootsolutioncomponentid", "ismetadata", "componenttype");
            if (selectedfilter.ShowSolution)
            {
                query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, selectedfilter.SolutionId);
            }
            else if (selectedfilter.ShowPublisher)
            {
                var query_solution = query.AddLink("solution", "solutionid", "solutionid");
                query_solution.LinkCriteria.AddCondition("publisherid", ConditionOperator.Equal, selectedfilter.PublisherId);
            }
            else if (selectedfilter.ShowUnmanagedSolutions)
            {
                var query_solution = query.AddLink("solution", "solutionid", "solutionid");
                query_solution.LinkCriteria.AddCondition("ismanaged", ConditionOperator.Equal, false);
                query_solution.LinkCriteria.AddCondition("uniquename", ConditionOperator.NotEqual, "Default");
                query_solution.LinkCriteria.AddCondition("isvisible", ConditionOperator.Equal, true);
            }
            else
            {
                query.Criteria.AddCondition("solutionid", ConditionOperator.Null);
            }
            var filtertype = new FilterExpression();
            query.Criteria.AddFilter(filtertype);
            filtertype.FilterOperator = LogicalOperator.Or;
            filtertype.AddCondition("componenttype", ConditionOperator.Equal, 1);
            filtertype.AddCondition("componenttype", ConditionOperator.Equal, 2);
            try
            {
                var result = RetrieveMultiple(query);
                solutionentities = result.Entities
                    .Where(c => c.GetAttributeValue<OptionSetValue>("componenttype").Value == 1).ToList();
                solutionattributes = result.Entities
                    .Where(c => c.GetAttributeValue<OptionSetValue>("componenttype").Value == 2)
                    .Select(c => c.GetAttributeValue<Guid>("objectid")).ToList();
            }
            catch (Exception ex)
            {
                solutionentities = null;
                solutionattributes = null;
                ShowErrorDialog(ex, "Loading Solutions Components");
            }
        }

        #endregion Private Methods
    }
}