import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";
import { Search } from "lucide-react";
import { useMemo, useState } from "react";
import { type MockEntity, mockEntities } from "../data/mockMetadata";

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
  const entityOptions = entities.length > 0 ? entities : mockEntities;
  const activeEntity =
    entityOptions.find((entity) => entity.logicalName === selectedEntity) ??
    entityOptions[0];
  const activeAttributes: AttributeSummary[] = isMockEntity(activeEntity)
    ? activeEntity.attributes
    : activeEntity
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
          {entityOptions.map((entity) => (
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

function isMockEntity(
  entity: EntitySummary | MockEntity | undefined,
): entity is MockEntity {
  return Boolean(entity && "relationships" in entity);
}
