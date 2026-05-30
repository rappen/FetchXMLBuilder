import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";
import { Search } from "lucide-react";
import { useMemo, useState } from "react";

interface MetadataBrowserProps {
  selectedEntity: string;
  entities: EntitySummary[];
  attributesByEntity: Record<string, AttributeSummary[]>;
  loadingAttributeEntity: string;
  onEntitySelected: (entityName: string) => void;
}

export function MetadataBrowser({
  selectedEntity,
  entities,
  attributesByEntity,
  loadingAttributeEntity,
  onEntitySelected,
}: MetadataBrowserProps) {
  const [query, setQuery] = useState("");
  const activeEntity =
    entities.find((entity) => entity.logicalName === selectedEntity) ??
    entities[0];
  const activeAttributes: AttributeSummary[] = activeEntity
    ? (attributesByEntity[activeEntity.logicalName] ?? [])
    : [];
  const attributes = useMemo(() => {
    const needle = query.trim().toLowerCase();
    if (!needle) return activeAttributes;
    return activeAttributes.filter(
      (attribute) =>
        attribute.logicalName.toLowerCase().includes(needle) ||
        attribute.displayName.toLowerCase().includes(needle) ||
        attribute.type.toLowerCase().includes(needle),
    );
  }, [activeAttributes, query]);
  const isLoadingAttributes =
    activeEntity?.logicalName === loadingAttributeEntity;

  return (
    <section className="panel side-panel" aria-label="Metadata browser">
      <div className="panel-heading">
        <h2>Metadata</h2>
        <div className="search-box">
          <Search size={15} />
          <input
            value={query}
            onChange={(event) => setQuery(event.target.value)}
            placeholder="Search"
          />
        </div>
      </div>

      <div className="metadata-layout">
        <div className="entity-list">
          {entities.length === 0 ? (
            <p className="empty-state">
              Connect to Dataverse to load metadata.
            </p>
          ) : null}
          {entities.map((entity) => (
            <button
              className={
                entity.logicalName === activeEntity?.logicalName
                  ? "entity-item active"
                  : "entity-item"
              }
              key={entity.logicalName}
              type="button"
              onClick={() => onEntitySelected(entity.logicalName)}
            >
              <strong>{entity.logicalName}</strong>
              <small>{entity.entitySetName}</small>
            </button>
          ))}
        </div>
        <div className="attribute-list">
          {!activeEntity ? (
            <p className="empty-state">No entity selected.</p>
          ) : null}
          {isLoadingAttributes ? (
            <p className="empty-state">Loading...</p>
          ) : null}
          {attributes.map((attribute) => (
            <div className="attribute-row" key={attribute.logicalName}>
              <span>{attribute.logicalName}</span>
              <small>{attribute.type}</small>
              <em>{attribute.displayName}</em>
            </div>
          ))}
          {!isLoadingAttributes && attributes.length === 0 ? (
            <p className="empty-state">No attributes loaded for this entity.</p>
          ) : null}
        </div>
      </div>
    </section>
  );
}
