import {
  type FetchConditionSelection,
  type FetchLinkEntitySelection,
  type FetchQueryModel,
  readFetchQueryModel,
  writeFetchQueryModel,
} from "@fetchxmlbuilder/core";
import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";
import { Plus, Trash2 } from "lucide-react";
import { useMemo } from "react";
import { getEntity, mockEntities } from "../data/mockMetadata";

interface QueryBuilderPanelProps {
  fetchXml: string;
  entities: EntitySummary[];
  attributesByEntity: Record<string, AttributeSummary[]>;
  loadingAttributeEntity: string;
  onChange: (fetchXml: string) => void;
  onEntitySelected: (entityName: string) => void;
}

const operators = [
  "eq",
  "ne",
  "like",
  "not-like",
  "gt",
  "ge",
  "lt",
  "le",
  "on-or-after",
  "on-or-before",
  "null",
  "not-null",
  "in",
];

export function QueryBuilderPanel({
  fetchXml,
  entities,
  attributesByEntity,
  loadingAttributeEntity,
  onChange,
  onEntitySelected,
}: QueryBuilderPanelProps) {
  const model = useMemo(() => safeReadModel(fetchXml), [fetchXml]);
  const entityOptions = entities.length > 0 ? entities : mockEntities;
  const entity = getBuilderEntity(model.entity, entities, attributesByEntity);
  const isLoadingAttributes = loadingAttributeEntity === entity.logicalName;

  function update(nextModel: FetchQueryModel) {
    onChange(writeFetchQueryModel(nextModel));
  }

  function setEntity(entityName: string) {
    const nextEntity = getBuilderEntity(
      entityName,
      entities,
      attributesByEntity,
    );
    onEntitySelected(nextEntity.logicalName);
    update({
      ...model,
      entity: nextEntity.logicalName,
      attributes: nextEntity.attributes.slice(0, 3).map((attribute) => ({
        name: attribute.logicalName,
      })),
      conditions: [],
      orders: [],
      links: [],
    });
  }

  function toggleAttribute(attributeName: string) {
    const exists = model.attributes.some(
      (attribute) => attribute.name === attributeName,
    );
    update({
      ...model,
      attributes: exists
        ? model.attributes.filter(
            (attribute) => attribute.name !== attributeName,
          )
        : [...model.attributes, { name: attributeName }],
    });
  }

  function updateCondition(
    id: string,
    patch: Partial<FetchConditionSelection>,
  ) {
    update({
      ...model,
      conditions: model.conditions.map((condition) =>
        condition.id === id ? { ...condition, ...patch } : condition,
      ),
    });
  }

  function updateLink(id: string, patch: Partial<FetchLinkEntitySelection>) {
    update({
      ...model,
      links: model.links.map((link) =>
        link.id === id ? { ...link, ...patch } : link,
      ),
    });
  }

  return (
    <section className="panel side-panel" aria-label="Visual query builder">
      <div className="panel-heading">
        <h2>Builder</h2>
        <button
          type="button"
          title="Add condition"
          onClick={() =>
            update({
              ...model,
              conditions: [
                ...model.conditions,
                {
                  id: crypto.randomUUID(),
                  attribute: entity.attributes[0]?.logicalName ?? "name",
                  operator: "eq",
                  value: "",
                },
              ],
            })
          }
        >
          <Plus size={16} />
          <span>Condition</span>
        </button>
      </div>

      <div className="form-grid">
        <label>
          <span>Entity</span>
          <select
            value={model.entity}
            onChange={(event) => setEntity(event.target.value)}
          >
            {entityOptions.map((item) => (
              <option key={item.logicalName} value={item.logicalName}>
                {item.displayName}
              </option>
            ))}
          </select>
        </label>
        <label>
          <span>Top</span>
          <input
            inputMode="numeric"
            value={model.top}
            onChange={(event) =>
              update({ ...model, top: event.target.value.replace(/\D/g, "") })
            }
          />
        </label>
      </div>

      <div className="builder-section">
        <div className="section-title-row">
          <h3>Attributes</h3>
          {isLoadingAttributes ? <small>Loading...</small> : null}
        </div>
        <div className="check-grid">
          {entity.attributes.map((attribute) => (
            <label className="check-row" key={attribute.logicalName}>
              <input
                type="checkbox"
                checked={model.attributes.some(
                  (item) => item.name === attribute.logicalName,
                )}
                onChange={() => toggleAttribute(attribute.logicalName)}
              />
              <span>{attribute.logicalName}</span>
              <small>{attribute.type}</small>
            </label>
          ))}
          {entity.attributes.length === 0 ? (
            <p className="empty-state">No attributes loaded for this entity.</p>
          ) : null}
        </div>
      </div>

      <div className="builder-section">
        <h3>Filters</h3>
        <div className="stack">
          {model.conditions.map((condition) => (
            <div className="condition-row" key={condition.id}>
              <select
                value={condition.attribute}
                onChange={(event) =>
                  updateCondition(condition.id, {
                    attribute: event.target.value,
                  })
                }
              >
                {entity.attributes.map((attribute) => (
                  <option
                    key={attribute.logicalName}
                    value={attribute.logicalName}
                  >
                    {attribute.logicalName}
                  </option>
                ))}
              </select>
              <select
                value={condition.operator}
                onChange={(event) =>
                  updateCondition(condition.id, {
                    operator: event.target.value,
                  })
                }
              >
                {operators.map((operator) => (
                  <option key={operator} value={operator}>
                    {operator}
                  </option>
                ))}
              </select>
              <input
                value={condition.value}
                onChange={(event) =>
                  updateCondition(condition.id, { value: event.target.value })
                }
              />
              <button
                className="icon-button"
                type="button"
                title="Remove condition"
                onClick={() =>
                  update({
                    ...model,
                    conditions: model.conditions.filter(
                      (item) => item.id !== condition.id,
                    ),
                  })
                }
              >
                <Trash2 size={15} />
              </button>
            </div>
          ))}
        </div>
      </div>

      <div className="builder-section">
        <h3>Order</h3>
        <div className="condition-row order-row">
          <select
            value={model.orders[0]?.attribute ?? ""}
            onChange={(event) =>
              update({
                ...model,
                orders: event.target.value
                  ? [
                      {
                        attribute: event.target.value,
                        descending: model.orders[0]?.descending ?? false,
                      },
                    ]
                  : [],
              })
            }
          >
            <option value="">None</option>
            {entity.attributes.map((attribute) => (
              <option key={attribute.logicalName} value={attribute.logicalName}>
                {attribute.logicalName}
              </option>
            ))}
          </select>
          <label className="toggle-row">
            <input
              type="checkbox"
              checked={model.orders[0]?.descending ?? false}
              onChange={(event) =>
                update({
                  ...model,
                  orders: model.orders[0]
                    ? [
                        {
                          ...model.orders[0],
                          descending: event.target.checked,
                        },
                      ]
                    : [],
                })
              }
            />
            <span>Desc</span>
          </label>
        </div>
      </div>

      <div className="builder-section">
        <div className="section-title-row">
          <h3>Links</h3>
          <button
            type="button"
            title="Add link"
            onClick={() =>
              update({
                ...model,
                links: [
                  ...model.links,
                  {
                    id: crypto.randomUUID(),
                    name: "contact",
                    from: "parentcustomerid",
                    to: "accountid",
                    alias: "contact",
                    linkType: "outer",
                    attributes: [{ name: "fullname" }],
                  },
                ],
              })
            }
          >
            <Plus size={16} />
            <span>Link</span>
          </button>
        </div>
        <div className="stack">
          {model.links.map((link) => (
            <div className="link-row" key={link.id}>
              <select
                value={link.name}
                onChange={(event) =>
                  updateLink(link.id, { name: event.target.value })
                }
              >
                {entityOptions.map((item) => (
                  <option key={item.logicalName} value={item.logicalName}>
                    {item.logicalName}
                  </option>
                ))}
              </select>
              <input
                value={link.from}
                onChange={(event) =>
                  updateLink(link.id, { from: event.target.value })
                }
              />
              <input
                value={link.to}
                onChange={(event) =>
                  updateLink(link.id, { to: event.target.value })
                }
              />
              <input
                value={link.alias}
                onChange={(event) =>
                  updateLink(link.id, { alias: event.target.value })
                }
              />
              <button
                className="icon-button"
                type="button"
                title="Remove link"
                onClick={() =>
                  update({
                    ...model,
                    links: model.links.filter((item) => item.id !== link.id),
                  })
                }
              >
                <Trash2 size={15} />
              </button>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

function getBuilderEntity(
  logicalName: string,
  entities: EntitySummary[],
  attributesByEntity: Record<string, AttributeSummary[]>,
) {
  const liveEntity = entities.find(
    (entity) => entity.logicalName === logicalName,
  );
  if (liveEntity) {
    return {
      logicalName: liveEntity.logicalName,
      displayName: liveEntity.displayName,
      entitySetName: liveEntity.entitySetName ?? "",
      attributes: attributesByEntity[liveEntity.logicalName] ?? [],
    };
  }

  return getEntity(logicalName);
}

function safeReadModel(fetchXml: string) {
  try {
    return readFetchQueryModel(fetchXml);
  } catch {
    return {
      entity: "account",
      top: "50",
      attributes: [{ name: "name" }],
      conditions: [],
      orders: [],
      links: [],
    };
  }
}
