using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Windows.Forms;

namespace Cinteros.Xrm.FetchXmlBuilder.Settings
{
    public class FXBConnectionSettings
    {
        public FilterSetting FilterSetting { get; set; } = new FilterSetting();
        public ShowMetaTypesEntity ShowEntities { get; set; } = new ShowMetaTypesEntity();
        public ShowMetaTypesAttribute ShowAttributes { get; set; } = new ShowMetaTypesAttribute();
        public string FetchXML { get; set; } = QueryOptions.DefaultNewQuery;
    }

    public class QueryOptions
    {
        internal static string DefaultNewQuery = "<fetch top=\"50\"><entity name=\"\"/></fetch>";
        public bool ShowQuickActions { get; set; } = true;
        public bool UseSingleQuotation { get; set; }
        public string NewQueryTemplate { get; set; } = DefaultNewQuery;
        public bool ShowAllAttributes { get; set; } = false;
    }

    public class FilterSetting
    {
        public bool ShowAllSolutions { get; set; } = true;
        public bool ShowUnmanagedSolutions { get; set; } = false;
        public bool ShowSolution { get; set; } = false;
        public bool ShowPublisher { get; set; } = false;
        public Guid SolutionId { get; set; } = Guid.Empty;
        public Guid PublisherId { get; set; } = Guid.Empty;
        public bool FilterByMetadata { get; set; } = true;
        public bool AlwaysPrimary { get; set; } = true;
        public bool AlwaysAddresses { get; set; } = true;

        public bool NoneEntitiesSelected =>
                 !ShowAllSolutions &&
                 !ShowUnmanagedSolutions &&
                 (!ShowSolution || SolutionId.Equals(Guid.Empty)) &&
                 (!ShowPublisher || PublisherId.Equals(Guid.Empty));
    }

    public abstract class ShowMetaTypes
    {
        public CheckState IsManaged { get; set; } = CheckState.Indeterminate;
        public CheckState IsCustom { get; set; } = CheckState.Indeterminate;
        public CheckState IsCustomizable { get; set; } = CheckState.Indeterminate;
        public CheckState IsValidForAdvancedFind { get; set; } = CheckState.Indeterminate;
        public CheckState IsAuditEnabled { get; set; } = CheckState.Indeterminate;
        public CheckState IsLogical { get; set; } = CheckState.Unchecked;
    }

    public class ShowMetaTypesEntity : ShowMetaTypes
    {
        public CheckState IsIntersect { get; set; } = CheckState.Indeterminate;  //E
        public CheckState IsActivity { get; set; } = CheckState.Indeterminate;  //E
        public CheckState IsActivityParty { get; set; } = CheckState.Indeterminate; //E
        public CheckState Virtual { get; set; } = CheckState.Indeterminate;  //E
        public int OwnershipType { get; set; } = 0;   //E

        public OwnershipTypes[] Ownerships
        {
            get
            {
                switch (OwnershipType)
                {
                    case 1:
                        return new OwnershipTypes[] { OwnershipTypes.OrganizationOwned };

                    case 2:
                        return new OwnershipTypes[] { OwnershipTypes.UserOwned, OwnershipTypes.TeamOwned };

                    case 3:
                        return new OwnershipTypes[] { OwnershipTypes.BusinessOwned, OwnershipTypes.BusinessParented };

                    case 4:
                        return new OwnershipTypes[] { OwnershipTypes.None };
                }
                return null;
            }
        }
    }

    public class ShowMetaTypesAttribute : ShowMetaTypes
    {
        public CheckState IsValidForRead { get; set; } = CheckState.Indeterminate;  //A
        public CheckState IsFiltered { get; set; } = CheckState.Indeterminate;  //A
        public CheckState IsRetrievable { get; set; } = CheckState.Indeterminate;   //A
        public CheckState IsValidForGrid { get; set; } = CheckState.Indeterminate;  //A
        public CheckState AttributeOf { get; set; } = CheckState.Unchecked; //A
        public CheckState CalculationOf { get; set; } = CheckState.Unchecked;
    }
}