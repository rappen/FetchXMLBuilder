import {
  type FetchConditionSelection,
  type FetchLinkEntitySelection,
  type FetchOrderSelection,
  type FetchQueryModel,
  readFetchQueryModel,
  writeFetchQueryModel,
} from "@fetchxmlbuilder/core";
import type {
  AttributeSummary,
  EntitySummary,
} from "@fetchxmlbuilder/dataverse";
import {
  AlertTriangle,
  ArrowDownAZ,
  ArrowUpAZ,
  BadgeCheck,
  Boxes,
  Filter,
  GitBranch,
  GitFork,
  Layers3,
  ListChecks,
  Plus,
  Search,
  Settings2,
  Trash2,
  X,
} from "lucide-react";
import { type ReactNode, useEffect, useMemo, useState } from "react";
import { XmlEditor } from "./XmlEditor";

interface QueryBuilderPanelProps {
  fetchXml: string;
  entities: EntitySummary[];
  attributesByEntity: Record<string, AttributeSummary[]>;
  loadingAttributeEntity: string;
  onChange: (fetchXml: string) => void;
  onEntitySelected: (entityName: string) => void;
  onWarningsChange?: (warningCount: number) => void;
}

type SelectedNode =
  | { type: "fetch" }
  | { type: "entity"; linkId?: string }
  | { type: "attributes"; linkId?: string }
  | { type: "filters"; linkId?: string }
  | { type: "orders"; linkId?: string };

interface BuilderEntity {
  logicalName: string;
  displayName: string;
  entitySetName: string;
  attributes: AttributeSummary[];
}

const defaultOperators = [
  "eq",
  "ne",
  "like",
  "not-like",
  "gt",
  "ge",
  "lt",
  "le",
  "null",
  "not-null",
  "in",
];

const operatorsByType: Record<string, string[]> = {
  String: [
    "eq",
    "ne",
    "like",
    "not-like",
    "begins-with",
    "ends-with",
    "null",
    "not-null",
    "in",
  ],
  Memo: ["eq", "ne", "like", "not-like", "null", "not-null"],
  DateTime: [
    "on",
    "on-or-after",
    "on-or-before",
    "today",
    "yesterday",
    "last-x-days",
    "next-x-days",
    "null",
    "not-null",
  ],
  Integer: ["eq", "ne", "gt", "ge", "lt", "le", "null", "not-null", "in"],
  BigInt: ["eq", "ne", "gt", "ge", "lt", "le", "null", "not-null", "in"],
  Decimal: ["eq", "ne", "gt", "ge", "lt", "le", "null", "not-null", "in"],
  Double: ["eq", "ne", "gt", "ge", "lt", "le", "null", "not-null", "in"],
  Money: ["eq", "ne", "gt", "ge", "lt", "le", "null", "not-null", "in"],
  Boolean: ["eq", "ne", "null", "not-null"],
  Picklist: ["eq", "ne", "in", "not-in", "null", "not-null"],
  State: ["eq", "ne", "in", "not-in"],
  Status: ["eq", "ne", "in", "not-in"],
  Lookup: ["eq", "ne", "null", "not-null", "in"],
  Customer: ["eq", "ne", "null", "not-null", "in"],
  Owner: ["eq", "ne", "null", "not-null", "in"],
  Guid: ["eq", "ne", "null", "not-null", "in"],
};

export function QueryBuilderPanel({
  fetchXml,
  entities,
  attributesByEntity,
  loadingAttributeEntity,
  onChange,
  onEntitySelected,
  onWarningsChange,
}: QueryBuilderPanelProps) {
  const model = useMemo(
    () => normalizeModel(safeReadModel(fetchXml)),
    [fetchXml],
  );
  const entityOptions = entities;
  const primaryEntity = getBuilderEntity(
    model.entity,
    entities,
    attributesByEntity,
    model.attributes.map((attribute) => attribute.name),
  );
  const [selectedNode, setSelectedNode] = useState<SelectedNode>({
    type: "fetch",
  });
  const [entitySearch, setEntitySearch] = useState("");
  const [attributeSearch, setAttributeSearch] = useState("");
  const [attributePicker, setAttributePicker] = useState<null | {
    linkId?: string;
  }>(null);
  const [draftRuleNodes, setDraftRuleNodes] = useState<Set<string>>(
    () => new Set(),
  );
  const [warningRuleNodes, setWarningRuleNodes] = useState<Set<string>>(
    () => new Set(),
  );

  const selectedLink =
    selectedNode.type !== "fetch" && selectedNode.linkId
      ? findLinkById(model.links, selectedNode.linkId)
      : undefined;
  const selectedEntityName = selectedLink?.name ?? model.entity;
  const selectedEntity = getBuilderEntity(
    selectedEntityName,
    entities,
    attributesByEntity,
    selectedLink
      ? selectedLink.attributes.map((attribute) => attribute.name)
      : model.attributes.map((attribute) => attribute.name),
  );
  const isLoadingAttributes =
    loadingAttributeEntity === selectedEntity.logicalName;

  useEffect(() => {
    if (selectedNode.type !== "fetch" && selectedNode.linkId) {
      const exists = Boolean(findLinkById(model.links, selectedNode.linkId));
      if (!exists) setSelectedNode({ type: "fetch" });
    }
  }, [model.links, selectedNode]);

  const visibleWarningRuleNodes = useMemo(
    () =>
      Array.from(warningRuleNodes).filter((key) => {
        const node = parseRuleNodeKey(key);
        if (!node) return false;
        if (node.linkId && !findLinkById(model.links, node.linkId)) {
          return false;
        }
        return getRuleNodeCount(model, node.type, node.linkId) === 0;
      }),
    [model, warningRuleNodes],
  );
  const visibleWarningRuleNodeSet = useMemo(
    () => new Set(visibleWarningRuleNodes),
    [visibleWarningRuleNodes],
  );
  const blockingRuleNodes = useMemo(() => {
    const keys = new Set(visibleWarningRuleNodes);
    for (const key of draftRuleNodes) {
      const node = parseRuleNodeKey(key);
      if (!node) continue;
      if (node.linkId && !findLinkById(model.links, node.linkId)) {
        continue;
      }
      if (getRuleNodeCount(model, node.type, node.linkId) === 0) {
        keys.add(key);
      }
    }
    if (
      isRuleNode(selectedNode) &&
      getRuleNodeCount(model, selectedNode.type, selectedNode.linkId) === 0
    ) {
      keys.add(makeRuleNodeKey(selectedNode.type, selectedNode.linkId));
    }
    return keys;
  }, [draftRuleNodes, model, selectedNode, visibleWarningRuleNodes]);

  useEffect(() => {
    onWarningsChange?.(blockingRuleNodes.size);
  }, [blockingRuleNodes.size, onWarningsChange]);

  function selectNode(nextNode: SelectedNode) {
    setSelectedNode((currentNode) => {
      if (isSameSelectedNode(currentNode, nextNode)) return nextNode;
      if (
        isRuleNode(currentNode) &&
        getRuleNodeCount(model, currentNode.type, currentNode.linkId) === 0
      ) {
        const key = makeRuleNodeKey(currentNode.type, currentNode.linkId);
        setWarningRuleNodes((current) => new Set(current).add(key));
      }
      return nextNode;
    });
  }

  function update(nextModel: FetchQueryModel) {
    onChange(writeFetchQueryModel(normalizeModel(nextModel)));
  }

  function focusDraftRuleNode(type: "filters" | "orders", linkId?: string) {
    const key = makeRuleNodeKey(type, linkId);
    setDraftRuleNodes((current) => new Set(current).add(key));
    setWarningRuleNodes((current) => {
      const next = new Set(current);
      next.delete(key);
      return next;
    });
    selectNode(makeSelectedNode(type, linkId));
  }

  function clearRuleNodeWarning(type: "filters" | "orders", linkId?: string) {
    const key = makeRuleNodeKey(type, linkId);
    setDraftRuleNodes((current) => {
      const next = new Set(current);
      next.delete(key);
      return next;
    });
    setWarningRuleNodes((current) => {
      const next = new Set(current);
      next.delete(key);
      return next;
    });
  }

  function clearLinkRuleNodeState(linkId: string) {
    setDraftRuleNodes((current) => {
      const next = new Set(current);
      next.delete(makeRuleNodeKey("filters", linkId));
      next.delete(makeRuleNodeKey("orders", linkId));
      return next;
    });
    setWarningRuleNodes((current) => {
      const next = new Set(current);
      next.delete(makeRuleNodeKey("filters", linkId));
      next.delete(makeRuleNodeKey("orders", linkId));
      return next;
    });
  }

  function setPrimaryEntity(entityName: string) {
    onEntitySelected(entityName);
    const nextEntity = getBuilderEntity(
      entityName,
      entities,
      attributesByEntity,
    );
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

  function updateLink(
    linkId: string,
    patch: Partial<FetchLinkEntitySelection>,
  ) {
    update({
      ...model,
      links: updateLinkById(model.links, linkId, (link) =>
        normalizeLink({ ...link, ...patch }),
      ),
    });
  }

  function setLinkEntity(linkId: string, entityName: string) {
    onEntitySelected(entityName);
    const nextEntity = getBuilderEntity(
      entityName,
      entities,
      attributesByEntity,
    );
    updateLink(linkId, {
      name: nextEntity.logicalName,
      alias: nextEntity.logicalName,
      attributes: nextEntity.attributes.slice(0, 2).map((attribute) => ({
        name: attribute.logicalName,
      })),
      conditions: [],
      orders: [],
    });
  }

  function toggleAttribute(attributeName: string, linkId?: string) {
    if (linkId) {
      const link = findLinkById(model.links, linkId);
      if (!link) return;
      const exists = link.attributes.some(
        (attribute) => attribute.name === attributeName,
      );
      updateLink(linkId, {
        attributes: exists
          ? link.attributes.filter(
              (attribute) => attribute.name !== attributeName,
            )
          : [...link.attributes, { name: attributeName }],
      });
      return;
    }

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

  function clearAttributes(linkId?: string) {
    if (linkId) {
      updateLink(linkId, { attributes: [] });
      selectNode({ type: "entity", linkId });
      return;
    }

    update({ ...model, attributes: [] });
    selectNode({ type: "entity" });
  }

  function addCondition(linkId?: string) {
    const attributes = getBuilderEntity(
      linkId
        ? (findLinkById(model.links, linkId)?.name ?? model.entity)
        : model.entity,
      entities,
      attributesByEntity,
    ).attributes;
    const condition = makeCondition(attributes[0]?.logicalName ?? "name");

    if (linkId) {
      const link = findLinkById(model.links, linkId);
      if (!link) return;
      updateLink(linkId, {
        conditions: [...(link.conditions ?? []), condition],
      });
      clearRuleNodeWarning("filters", linkId);
      selectNode(makeSelectedNode("filters", linkId));
      return;
    }

    update({ ...model, conditions: [...model.conditions, condition] });
    clearRuleNodeWarning("filters");
    selectNode({ type: "filters" });
  }

  function updateCondition(
    conditionId: string,
    patch: Partial<FetchConditionSelection>,
    linkId?: string,
  ) {
    if (linkId) {
      const link = findLinkById(model.links, linkId);
      if (!link) return;
      updateLink(linkId, {
        conditions: (link.conditions ?? []).map((condition) =>
          condition.id === conditionId ? { ...condition, ...patch } : condition,
        ),
      });
      return;
    }

    update({
      ...model,
      conditions: model.conditions.map((condition) =>
        condition.id === conditionId ? { ...condition, ...patch } : condition,
      ),
    });
  }

  function removeCondition(conditionId: string, linkId?: string) {
    if (linkId) {
      const link = findLinkById(model.links, linkId);
      if (!link) return;
      updateLink(linkId, {
        conditions: (link.conditions ?? []).filter(
          (condition) => condition.id !== conditionId,
        ),
      });
      return;
    }

    update({
      ...model,
      conditions: model.conditions.filter(
        (condition) => condition.id !== conditionId,
      ),
    });
  }

  function clearFilters(linkId?: string) {
    if (linkId) {
      updateLink(linkId, { conditions: [] });
      clearRuleNodeWarning("filters", linkId);
      selectNode({ type: "entity", linkId });
      return;
    }

    update({ ...model, conditions: [] });
    clearRuleNodeWarning("filters");
    selectNode({ type: "entity" });
  }

  function addOrder(linkId?: string) {
    const attribute = selectedEntity.attributes[0]?.logicalName ?? "name";
    const order: FetchOrderSelection = { attribute, descending: false };
    if (linkId) {
      const link = findLinkById(model.links, linkId);
      if (!link) return;
      updateLink(linkId, { orders: [...(link.orders ?? []), order] });
      clearRuleNodeWarning("orders", linkId);
      selectNode(makeSelectedNode("orders", linkId));
      return;
    }
    update({ ...model, orders: [...model.orders, order] });
    clearRuleNodeWarning("orders");
    selectNode({ type: "orders" });
  }

  function clearOrders(linkId?: string) {
    if (linkId) {
      updateLink(linkId, { orders: [] });
      clearRuleNodeWarning("orders", linkId);
      selectNode({ type: "entity", linkId });
      return;
    }

    update({ ...model, orders: [] });
    clearRuleNodeWarning("orders");
    selectNode({ type: "entity" });
  }

  function removeLink(linkId: string) {
    update({
      ...model,
      links: removeLinkById(model.links, linkId),
    });
    clearLinkRuleNodeState(linkId);
    if ("linkId" in selectedNode && selectedNode.linkId === linkId) {
      selectNode({ type: "entity" });
    }
  }

  function addLink() {
    const fallback =
      entityOptions.find((entity) => entity.logicalName !== model.entity) ??
      entityOptions[0];
    const linkEntity = fallback?.logicalName ?? "contact";
    const parentEntityName = selectedLink?.name ?? model.entity;
    const parentJoinAttribute =
      selectedLink?.attributes[0]?.name ??
      model.attributes[0]?.name ??
      `${parentEntityName}id`;
    onEntitySelected(linkEntity);
    const nextLink = normalizeLink({
      id: crypto.randomUUID(),
      name: linkEntity,
      from: "parentcustomerid",
      to: parentJoinAttribute,
      alias: linkEntity,
      linkType: "outer",
      attributes: [],
      filterType: "and",
      conditions: [],
      orders: [],
      links: [],
    });
    if (selectedLink) {
      update({
        ...model,
        links: updateLinkById(model.links, selectedLink.id, (link) => ({
          ...link,
          links: [...(link.links ?? []), nextLink],
        })),
      });
    } else {
      update({ ...model, links: [...model.links, nextLink] });
    }
    selectNode({ type: "entity", linkId: nextLink.id });
  }

  return (
    <section className="builder-workbench" aria-label="Visual query builder">
      <aside
        className="inspector-panel panel"
        aria-label="Selected builder controls"
      >
        <InspectorHeader
          selectedNode={selectedNode}
          entity={selectedEntity}
          {...(selectedLink ? { link: selectedLink } : {})}
        />
        <div className="inspector-body">
          {selectedNode.type === "fetch" ? (
            <FetchInspector model={model} onUpdate={update} />
          ) : null}
          {selectedNode.type === "entity" ? (
            <EntityInspector
              entity={selectedEntity}
              entityOptions={entityOptions}
              entitySearch={entitySearch}
              isPrimary={!selectedNode.linkId}
              isLoadingAttributes={isLoadingAttributes}
              model={model}
              {...(selectedLink ? { link: selectedLink } : {})}
              onCreateFilter={() =>
                focusDraftRuleNode("filters", selectedNode.linkId)
              }
              onAddLink={addLink}
              onCreateOrder={() =>
                focusDraftRuleNode("orders", selectedNode.linkId)
              }
              onOpenAttributes={() =>
                setAttributePicker(
                  selectedNode.linkId ? { linkId: selectedNode.linkId } : {},
                )
              }
              onRemoveLink={() => {
                if (!selectedNode.linkId) return;
                removeLink(selectedNode.linkId);
              }}
              onSearchChange={setEntitySearch}
              onSelectEntity={(entityName) =>
                selectedNode.linkId
                  ? setLinkEntity(selectedNode.linkId, entityName)
                  : setPrimaryEntity(entityName)
              }
              onUpdateLink={(patch) =>
                selectedNode.linkId && updateLink(selectedNode.linkId, patch)
              }
            />
          ) : null}
          {selectedNode.type === "attributes" ? (
            <AttributesInspector
              attributes={selectedEntity.attributes}
              selectedAttributes={
                selectedNode.linkId
                  ? (selectedLink?.attributes ?? [])
                  : model.attributes
              }
              onOpenPicker={() =>
                setAttributePicker(
                  selectedNode.linkId ? { linkId: selectedNode.linkId } : {},
                )
              }
              onToggle={(attributeName) =>
                toggleAttribute(attributeName, selectedNode.linkId)
              }
            />
          ) : null}
          {selectedNode.type === "filters" ? (
            <FilterInspector
              attributes={selectedEntity.attributes}
              conditions={
                selectedNode.linkId
                  ? (selectedLink?.conditions ?? [])
                  : model.conditions
              }
              filterType={
                selectedNode.linkId
                  ? (selectedLink?.filterType ?? "and")
                  : model.filterType
              }
              {...(selectedNode.linkId ? { linkId: selectedNode.linkId } : {})}
              onAdd={() => addCondition(selectedNode.linkId)}
              onFilterTypeChange={(filterType) =>
                selectedNode.linkId
                  ? updateLink(selectedNode.linkId, { filterType })
                  : update({ ...model, filterType })
              }
              onRemove={removeCondition}
              onUpdate={updateCondition}
            />
          ) : null}
          {selectedNode.type === "orders" ? (
            <OrderInspector
              attributes={selectedEntity.attributes}
              orders={
                selectedNode.linkId
                  ? (selectedLink?.orders ?? [])
                  : model.orders
              }
              onAdd={() => addOrder(selectedNode.linkId)}
              onRemove={(index) => {
                if (selectedNode.linkId) {
                  updateLink(selectedNode.linkId, {
                    orders: (selectedLink?.orders ?? []).filter(
                      (_, orderIndex) => orderIndex !== index,
                    ),
                  });
                  return;
                }
                update({
                  ...model,
                  orders: model.orders.filter(
                    (_, orderIndex) => orderIndex !== index,
                  ),
                });
              }}
              onUpdate={(index, patch) => {
                if (selectedNode.linkId) {
                  updateLink(selectedNode.linkId, {
                    orders: (selectedLink?.orders ?? []).map(
                      (order, orderIndex) =>
                        orderIndex === index ? { ...order, ...patch } : order,
                    ),
                  });
                  return;
                }
                update({
                  ...model,
                  orders: model.orders.map((order, orderIndex) =>
                    orderIndex === index ? { ...order, ...patch } : order,
                  ),
                });
              }}
            />
          ) : null}
        </div>
      </aside>

      <div className="composition-panel panel">
        <div className="panel-heading builder-heading">
          <div>
            <h2>Composition</h2>
            <span>Graphic map of what the FetchXML will do</span>
          </div>
          <button type="button" title="Add linked entity" onClick={addLink}>
            <GitBranch size={16} />
            <span>Link</span>
          </button>
        </div>
        <div className="query-tree" role="tree">
          <TreeButton
            active={selectedNode.type === "fetch"}
            icon={<Settings2 size={17} />}
            label="Fetch"
            meta={`${model.top ? `Top ${model.top}` : "All rows"}${model.distinct ? " · distinct" : ""}`}
            tone="fetch"
            onClick={() => selectNode({ type: "fetch" })}
          />
          <div className="tree-children">
            <EntityBranch
              entityName={model.entity}
              displayName={primaryEntity.displayName}
              attributes={model.attributes.length}
              conditions={model.conditions.length}
              filterType={model.filterType}
              links={model.links}
              orders={model.orders.length}
              entities={entities}
              attributesByEntity={attributesByEntity}
              selectedNode={selectedNode}
              draftRuleNodes={draftRuleNodes}
              warningRuleNodes={visibleWarningRuleNodeSet}
              onClearAttributes={clearAttributes}
              onClearFilters={clearFilters}
              onClearOrders={clearOrders}
              onRemoveEntity={removeLink}
              onSelect={selectNode}
            />
          </div>
        </div>
      </div>

      <section className="fetch-preview-panel panel" aria-label="FetchXML">
        <div className="panel-heading builder-heading">
          <div>
            <h2>FetchXML</h2>
            <span>Live result</span>
          </div>
        </div>
        <XmlEditor value={fetchXml} onChange={onChange} />
      </section>

      {attributePicker ? (
        <AttributePickerDialog
          attributes={
            getBuilderEntity(
              attributePicker.linkId
                ? (findLinkById(model.links, attributePicker.linkId)?.name ??
                    model.entity)
                : model.entity,
              entities,
              attributesByEntity,
              attributePicker.linkId
                ? (findLinkById(
                    model.links,
                    attributePicker.linkId,
                  )?.attributes.map((attribute) => attribute.name) ?? [])
                : model.attributes.map((attribute) => attribute.name),
            ).attributes
          }
          query={attributeSearch}
          selectedAttributes={
            attributePicker.linkId
              ? (findLinkById(model.links, attributePicker.linkId)
                  ?.attributes ?? [])
              : model.attributes
          }
          onClose={() => {
            setAttributePicker(null);
            setAttributeSearch("");
          }}
          onQueryChange={setAttributeSearch}
          onToggle={(attributeName) =>
            toggleAttribute(attributeName, attributePicker.linkId)
          }
        />
      ) : null}
    </section>
  );
}

function EntityBranch({
  entityName,
  displayName,
  alias,
  linkType,
  linkId,
  attributes,
  conditions,
  filterType,
  links,
  orders,
  entities,
  attributesByEntity,
  selectedNode,
  draftRuleNodes,
  warningRuleNodes,
  onClearAttributes,
  onClearFilters,
  onClearOrders,
  onRemoveEntity,
  onSelect,
}: {
  entityName: string;
  displayName: string;
  alias?: string;
  linkType?: "inner" | "outer";
  linkId?: string;
  attributes: number;
  conditions: number;
  filterType: "and" | "or";
  links: FetchLinkEntitySelection[];
  orders: number;
  entities: EntitySummary[];
  attributesByEntity: Record<string, AttributeSummary[]>;
  selectedNode: SelectedNode;
  draftRuleNodes: Set<string>;
  warningRuleNodes: Set<string>;
  onClearAttributes: (linkId?: string) => void;
  onClearFilters: (linkId?: string) => void;
  onClearOrders: (linkId?: string) => void;
  onRemoveEntity?: (linkId: string) => void;
  onSelect: (node: SelectedNode) => void;
}) {
  const attributesNode = makeSelectedNode("attributes", linkId);
  const filtersNode = makeSelectedNode("filters", linkId);
  const ordersNode = makeSelectedNode("orders", linkId);
  const isAttributesActive = isSameSelectedNode(selectedNode, attributesNode);
  const isFiltersActive = isSameSelectedNode(selectedNode, filtersNode);
  const isOrdersActive = isSameSelectedNode(selectedNode, ordersNode);
  const filterKey = makeRuleNodeKey("filters", linkId);
  const orderKey = makeRuleNodeKey("orders", linkId);
  const showAttributes = attributes > 0 || isAttributesActive;
  const showFilters =
    conditions > 0 ||
    isFiltersActive ||
    draftRuleNodes.has(filterKey) ||
    warningRuleNodes.has(filterKey);
  const showOrders =
    orders > 0 ||
    isOrdersActive ||
    draftRuleNodes.has(orderKey) ||
    warningRuleNodes.has(orderKey);
  const showChildren =
    showAttributes || showFilters || showOrders || links.length > 0;

  return (
    <div className="entity-branch">
      <TreeButton
        active={
          selectedNode.type === "entity" && selectedNode.linkId === linkId
        }
        icon={linkId ? <GitFork size={17} /> : <Boxes size={17} />}
        label={displayName || entityName}
        meta={[
          entityName,
          alias ? `alias ${alias}` : "",
          linkType ? `${linkType} join` : "primary",
        ]
          .filter(Boolean)
          .join(" · ")}
        tone={linkId ? "link" : "entity"}
        {...(linkId && onRemoveEntity
          ? {
              deleteTitle: "Remove linked entity",
              onDelete: () => onRemoveEntity(linkId),
            }
          : {})}
        onClick={() => onSelect(makeSelectedNode("entity", linkId))}
      />
      {showChildren ? (
        <div className="tree-children slim">
          {showAttributes ? (
            <TreeButton
              active={isAttributesActive}
              icon={<ListChecks size={16} />}
              label="Attributes"
              meta={`${attributes} selected`}
              tone="leaf"
              deleteTitle="Clear attributes"
              onDelete={() => onClearAttributes(linkId)}
              onClick={() => onSelect(attributesNode)}
            />
          ) : null}
          {showFilters ? (
            <TreeButton
              active={isFiltersActive}
              icon={
                warningRuleNodes.has(filterKey) ? (
                  <AlertTriangle size={16} />
                ) : (
                  <Filter size={16} />
                )
              }
              label="Filters"
              meta={
                conditions
                  ? `${filterType.toUpperCase()} · ${conditions} condition${conditions === 1 ? "" : "s"}`
                  : warningRuleNodes.has(filterKey)
                    ? "Missing conditions"
                    : "New filter"
              }
              tone={warningRuleNodes.has(filterKey) ? "warning" : "leaf"}
              deleteTitle="Remove filter node"
              onDelete={() => onClearFilters(linkId)}
              onClick={() => onSelect(filtersNode)}
            />
          ) : null}
          {showOrders ? (
            <TreeButton
              active={isOrdersActive}
              icon={
                warningRuleNodes.has(orderKey) ? (
                  <AlertTriangle size={16} />
                ) : orders ? (
                  <ArrowDownAZ size={16} />
                ) : (
                  <ArrowUpAZ size={16} />
                )
              }
              label="Orders"
              meta={
                orders
                  ? `${orders} sort${orders === 1 ? "" : "s"}`
                  : warningRuleNodes.has(orderKey)
                    ? "Missing sort rules"
                    : "New order"
              }
              tone={warningRuleNodes.has(orderKey) ? "warning" : "leaf"}
              deleteTitle="Remove order node"
              onDelete={() => onClearOrders(linkId)}
              onClick={() => onSelect(ordersNode)}
            />
          ) : null}
          {links.map((link) => {
            const linkEntity = getBuilderEntity(
              link.name,
              entities,
              attributesByEntity,
              link.attributes.map((attribute) => attribute.name),
            );
            return (
              <EntityBranch
                key={link.id}
                entityName={link.name}
                displayName={linkEntity.displayName}
                alias={link.alias}
                linkType={link.linkType}
                linkId={link.id}
                attributes={link.attributes.length}
                conditions={(link.conditions ?? []).length}
                filterType={link.filterType ?? "and"}
                links={link.links ?? []}
                orders={(link.orders ?? []).length}
                entities={entities}
                attributesByEntity={attributesByEntity}
                selectedNode={selectedNode}
                draftRuleNodes={draftRuleNodes}
                warningRuleNodes={warningRuleNodes}
                onClearAttributes={onClearAttributes}
                onClearFilters={onClearFilters}
                onClearOrders={onClearOrders}
                {...(onRemoveEntity ? { onRemoveEntity } : {})}
                onSelect={onSelect}
              />
            );
          })}
        </div>
      ) : null}
    </div>
  );
}

function TreeButton({
  active,
  icon,
  label,
  meta,
  tone,
  deleteTitle,
  onClick,
  onDelete,
}: {
  active: boolean;
  icon: ReactNode;
  label: string;
  meta: string;
  tone: "fetch" | "entity" | "link" | "leaf" | "warning";
  deleteTitle?: string;
  onClick: () => void;
  onDelete?: () => void;
}) {
  return (
    <div className="tree-node-frame">
      <button
        className={`tree-node ${tone}${active ? " active" : ""}${onDelete ? " has-delete" : ""}`}
        type="button"
        onClick={onClick}
      >
        <span className="tree-node-icon">{icon}</span>
        <span className="tree-node-copy">
          <strong>{label}</strong>
          <small>{meta}</small>
        </span>
      </button>
      {onDelete ? (
        <button
          className="tree-node-delete"
          type="button"
          title={deleteTitle ?? "Delete node"}
          onClick={(event) => {
            event.stopPropagation();
            onDelete();
          }}
        >
          <Trash2 size={14} />
        </button>
      ) : null}
    </div>
  );
}

function InspectorHeader({
  selectedNode,
  link,
  entity,
}: {
  selectedNode: SelectedNode;
  link?: FetchLinkEntitySelection;
  entity: BuilderEntity;
}) {
  const title =
    selectedNode.type === "fetch"
      ? "Fetch properties"
      : selectedNode.type === "entity"
        ? link
          ? "Linked entity"
          : "Primary entity"
        : selectedNode.type === "attributes"
          ? "Attributes"
          : selectedNode.type === "filters"
            ? "Filters"
            : "Orders";
  return (
    <div className="panel-heading builder-heading">
      <div>
        <h2>{title}</h2>
        <span>
          {selectedNode.type === "fetch"
            ? "Result set behavior"
            : entity.logicalName}
        </span>
      </div>
    </div>
  );
}

function FetchInspector({
  model,
  onUpdate,
}: {
  model: FetchQueryModel;
  onUpdate: (model: FetchQueryModel) => void;
}) {
  return (
    <div className="inspector-stack">
      <label>
        <span>Top count</span>
        <input
          inputMode="numeric"
          placeholder="All rows"
          value={model.top}
          onChange={(event) =>
            onUpdate({ ...model, top: event.target.value.replace(/\D/g, "") })
          }
        />
      </label>
      <label className="switch-row">
        <input
          type="checkbox"
          checked={model.distinct}
          onChange={(event) =>
            onUpdate({ ...model, distinct: event.target.checked })
          }
        />
        <span>Distinct rows</span>
      </label>
    </div>
  );
}

function EntityInspector({
  entity,
  entityOptions,
  entitySearch,
  isPrimary,
  isLoadingAttributes,
  link,
  model,
  onAddLink,
  onCreateFilter,
  onCreateOrder,
  onOpenAttributes,
  onRemoveLink,
  onSearchChange,
  onSelectEntity,
  onUpdateLink,
}: {
  entity: BuilderEntity;
  entityOptions: EntitySummary[];
  entitySearch: string;
  isPrimary: boolean;
  isLoadingAttributes: boolean;
  link?: FetchLinkEntitySelection;
  model: FetchQueryModel;
  onAddLink: () => void;
  onCreateFilter: () => void;
  onCreateOrder: () => void;
  onOpenAttributes: () => void;
  onRemoveLink: () => void;
  onSearchChange: (value: string) => void;
  onSelectEntity: (entityName: string) => void;
  onUpdateLink: (patch: Partial<FetchLinkEntitySelection>) => void;
}) {
  const matches = filterEntities(entityOptions, entitySearch).slice(0, 18);
  return (
    <div className="inspector-stack">
      <label>
        <span>{isPrimary ? "Primary entity" : "Linked entity"}</span>
        <div className="search-box inspector-search">
          <Search size={15} />
          <input
            value={entitySearch}
            placeholder="Search logical or display name"
            onChange={(event) => onSearchChange(event.target.value)}
          />
        </div>
      </label>
      <div className="dynamic-results">
        {matches.map((item) => (
          <button
            className={
              item.logicalName === entity.logicalName
                ? "entity-result active"
                : "entity-result"
            }
            key={item.logicalName}
            type="button"
            onClick={() => onSelectEntity(item.logicalName)}
          >
            <strong>{item.displayName || item.logicalName}</strong>
            <small>{item.logicalName}</small>
          </button>
        ))}
      </div>

      {!isPrimary && link ? (
        <div className="form-grid compact-form">
          <label>
            <span>Alias</span>
            <input
              value={link.alias}
              onChange={(event) => onUpdateLink({ alias: event.target.value })}
            />
          </label>
          <label>
            <span>Link type</span>
            <select
              value={link.linkType}
              onChange={(event) =>
                onUpdateLink({
                  linkType: event.target.value as "inner" | "outer",
                })
              }
            >
              <option value="inner">Inner</option>
              <option value="outer">Outer</option>
            </select>
          </label>
          <label>
            <span>From</span>
            <input
              value={link.from}
              onChange={(event) => onUpdateLink({ from: event.target.value })}
            />
          </label>
          <label>
            <span>To</span>
            <input
              value={link.to}
              onChange={(event) => onUpdateLink({ to: event.target.value })}
            />
          </label>
        </div>
      ) : null}

      <div className="action-grid">
        <button type="button" onClick={onOpenAttributes}>
          <ListChecks size={16} />
          <span>Attributes</span>
        </button>
        <button type="button" onClick={onCreateFilter}>
          <Filter size={16} />
          <span>Create filter</span>
        </button>
        <button type="button" onClick={onCreateOrder}>
          <ArrowDownAZ size={16} />
          <span>Set order</span>
        </button>
        <button type="button" onClick={onAddLink}>
          <GitBranch size={16} />
          <span>Link entity</span>
        </button>
      </div>

      <div className="selection-summary">
        <BadgeCheck size={16} />
        <span>
          {entity.attributes.length} attributes available
          {isLoadingAttributes ? " · loading metadata" : ""}
          {isPrimary ? ` · ${model.links.length} linked` : ""}
        </span>
      </div>

      {!isPrimary ? (
        <button className="danger-action" type="button" onClick={onRemoveLink}>
          <Trash2 size={16} />
          <span>Remove link</span>
        </button>
      ) : null}
    </div>
  );
}

function AttributesInspector({
  attributes,
  selectedAttributes,
  onOpenPicker,
  onToggle,
}: {
  attributes: AttributeSummary[];
  selectedAttributes: Array<{ name: string }>;
  onOpenPicker: () => void;
  onToggle: (attributeName: string) => void;
}) {
  return (
    <div className="inspector-stack">
      <button className="primary-action" type="button" onClick={onOpenPicker}>
        <ListChecks size={16} />
        <span>Select attributes</span>
      </button>
      <div className="selected-chip-list">
        {selectedAttributes.map((attribute) => (
          <button
            key={attribute.name}
            type="button"
            onClick={() => onToggle(attribute.name)}
          >
            <span>{attribute.name}</span>
            <X size={14} />
          </button>
        ))}
        {selectedAttributes.length === 0 ? (
          <p className="empty-state compact">No attributes selected.</p>
        ) : null}
      </div>
      <small className="inspector-note">
        {attributes.length} attributes in metadata for this entity.
      </small>
    </div>
  );
}

function FilterInspector({
  attributes,
  conditions,
  filterType,
  linkId,
  onAdd,
  onFilterTypeChange,
  onRemove,
  onUpdate,
}: {
  attributes: AttributeSummary[];
  conditions: FetchConditionSelection[];
  filterType: "and" | "or";
  linkId?: string;
  onAdd: () => void;
  onFilterTypeChange: (filterType: "and" | "or") => void;
  onRemove: (conditionId: string, linkId?: string) => void;
  onUpdate: (
    conditionId: string,
    patch: Partial<FetchConditionSelection>,
    linkId?: string,
  ) => void;
}) {
  return (
    <div className="inspector-stack">
      <div className="segmented filter-type-toggle">
        <button
          className={filterType === "and" ? "active" : ""}
          type="button"
          onClick={() => onFilterTypeChange("and")}
        >
          AND group
        </button>
        <button
          className={filterType === "or" ? "active" : ""}
          type="button"
          onClick={() => onFilterTypeChange("or")}
        >
          OR group
        </button>
      </div>
      <div className="filter-list">
        {conditions.length === 0 ? (
          <p className="empty-state compact warning-empty">
            Add at least one condition before leaving this filter.
          </p>
        ) : null}
        {conditions.map((condition) => {
          const attribute = attributes.find(
            (item) => item.logicalName === condition.attribute,
          );
          const operators = attribute
            ? (operatorsByType[attribute.type] ?? defaultOperators)
            : defaultOperators;
          const operatorNeedsValue = ![
            "null",
            "not-null",
            "today",
            "yesterday",
          ].includes(condition.operator);
          return (
            <div className="filter-card" key={condition.id}>
              <label>
                <span>Attribute</span>
                <select
                  value={condition.attribute}
                  onChange={(event) => {
                    const nextAttribute = attributes.find(
                      (item) => item.logicalName === event.target.value,
                    );
                    const nextOperator = nextAttribute
                      ? (operatorsByType[nextAttribute.type]?.[0] ?? "eq")
                      : "eq";
                    onUpdate(
                      condition.id,
                      { attribute: event.target.value, operator: nextOperator },
                      linkId,
                    );
                  }}
                >
                  {attributes.map((item) => (
                    <option key={item.logicalName} value={item.logicalName}>
                      {item.displayName || item.logicalName}
                    </option>
                  ))}
                </select>
              </label>
              <label>
                <span>Operation</span>
                <select
                  value={condition.operator}
                  onChange={(event) =>
                    onUpdate(
                      condition.id,
                      { operator: event.target.value },
                      linkId,
                    )
                  }
                >
                  {operators.map((operator) => (
                    <option key={operator} value={operator}>
                      {operator}
                    </option>
                  ))}
                </select>
              </label>
              {operatorNeedsValue ? (
                <label>
                  <span>Value</span>
                  <input
                    value={condition.value}
                    onChange={(event) =>
                      onUpdate(
                        condition.id,
                        { value: event.target.value },
                        linkId,
                      )
                    }
                  />
                </label>
              ) : null}
              <button
                className="icon-button danger-action"
                type="button"
                title="Remove condition"
                onClick={() => onRemove(condition.id, linkId)}
              >
                <Trash2 size={15} />
              </button>
            </div>
          );
        })}
      </div>
      <button type="button" onClick={onAdd}>
        <Plus size={16} />
        <span>Add condition</span>
      </button>
    </div>
  );
}

function OrderInspector({
  attributes,
  orders,
  onAdd,
  onRemove,
  onUpdate,
}: {
  attributes: AttributeSummary[];
  orders: FetchOrderSelection[];
  onAdd: () => void;
  onRemove: (index: number) => void;
  onUpdate: (index: number, patch: Partial<FetchOrderSelection>) => void;
}) {
  return (
    <div className="inspector-stack">
      {orders.map((order, index) => (
        <div className="order-card" key={`${order.attribute}-${index}`}>
          <select
            value={order.attribute}
            onChange={(event) =>
              onUpdate(index, { attribute: event.target.value })
            }
          >
            {attributes.map((attribute) => (
              <option key={attribute.logicalName} value={attribute.logicalName}>
                {attribute.displayName || attribute.logicalName}
              </option>
            ))}
          </select>
          <label className="switch-row">
            <input
              type="checkbox"
              checked={order.descending}
              onChange={(event) =>
                onUpdate(index, { descending: event.target.checked })
              }
            />
            <span>{order.descending ? "Descending" : "Ascending"}</span>
          </label>
          <button
            className="icon-button danger-action"
            type="button"
            title="Remove order"
            onClick={() => onRemove(index)}
          >
            <Trash2 size={15} />
          </button>
        </div>
      ))}
      {orders.length === 0 ? (
        <p className="empty-state compact warning-empty">
          Add at least one sort rule before leaving this order.
        </p>
      ) : null}
      <button type="button" onClick={onAdd}>
        <Plus size={16} />
        <span>Add order</span>
      </button>
    </div>
  );
}

function AttributePickerDialog({
  attributes,
  query,
  selectedAttributes,
  onClose,
  onQueryChange,
  onToggle,
}: {
  attributes: AttributeSummary[];
  query: string;
  selectedAttributes: Array<{ name: string }>;
  onClose: () => void;
  onQueryChange: (value: string) => void;
  onToggle: (attributeName: string) => void;
}) {
  const selectedNames = new Set(
    selectedAttributes.map((attribute) => attribute.name),
  );
  const visibleAttributes = filterAttributes(attributes, query).slice(0, 80);

  return (
    <div className="modal-backdrop" role="presentation">
      <dialog className="attribute-dialog" aria-label="Select attributes" open>
        <div className="panel-heading builder-heading">
          <div>
            <h2>Select attributes</h2>
            <span>{selectedAttributes.length} selected</span>
          </div>
          <button
            className="icon-button"
            type="button"
            title="Close"
            onClick={onClose}
          >
            <X size={17} />
          </button>
        </div>
        <div className="attribute-dialog-search">
          <div className="search-box">
            <Search size={15} />
            <input
              value={query}
              placeholder="Search attributes"
              onChange={(event) => onQueryChange(event.target.value)}
            />
          </div>
        </div>
        <div className="attribute-picker-list">
          {visibleAttributes.map((attribute) => (
            <label className="attribute-picker-row" key={attribute.logicalName}>
              <input
                type="checkbox"
                checked={selectedNames.has(attribute.logicalName)}
                onChange={() => onToggle(attribute.logicalName)}
              />
              <span>
                <strong>
                  {attribute.displayName || attribute.logicalName}
                </strong>
                <small>{attribute.logicalName}</small>
              </span>
              <em>{attribute.type}</em>
            </label>
          ))}
          {visibleAttributes.length === 0 ? (
            <p className="empty-state compact">No matching attributes.</p>
          ) : null}
        </div>
      </dialog>
    </div>
  );
}

function getBuilderEntity(
  logicalName: string,
  entities: EntitySummary[],
  attributesByEntity: Record<string, AttributeSummary[]>,
  selectedAttributeNames: string[] = [],
): BuilderEntity {
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

  return {
    logicalName,
    displayName: logicalName,
    entitySetName: "",
    attributes: selectedAttributeNames.map((attributeName) => ({
      logicalName: attributeName,
      displayName: attributeName,
      type: "Unknown",
    })),
  };
}

function filterEntities(entities: EntitySummary[], query: string) {
  const normalizedQuery = query.trim().toLowerCase();
  if (!normalizedQuery) return entities;
  return entities.filter((entity) =>
    [entity.logicalName, entity.displayName].some((value) =>
      (value ?? "").toLowerCase().includes(normalizedQuery),
    ),
  );
}

function filterAttributes(attributes: AttributeSummary[], query: string) {
  const normalizedQuery = query.trim().toLowerCase();
  if (!normalizedQuery) return attributes;
  return attributes.filter((attribute) =>
    [attribute.logicalName, attribute.displayName, attribute.type].some(
      (value) => (value ?? "").toLowerCase().includes(normalizedQuery),
    ),
  );
}

function makeCondition(attribute: string): FetchConditionSelection {
  return {
    id: crypto.randomUUID(),
    attribute,
    operator: "eq",
    value: "",
  };
}

function makeSelectedNode(
  type: "entity" | "attributes" | "filters" | "orders",
  linkId?: string,
): SelectedNode {
  return linkId ? { type, linkId } : { type };
}

function isRuleNode(
  node: SelectedNode,
): node is { type: "filters" | "orders"; linkId?: string } {
  return node.type === "filters" || node.type === "orders";
}

function isSameSelectedNode(left: SelectedNode, right: SelectedNode) {
  const leftLinkId = "linkId" in left ? left.linkId : undefined;
  const rightLinkId = "linkId" in right ? right.linkId : undefined;
  return left.type === right.type && leftLinkId === rightLinkId;
}

function makeRuleNodeKey(type: "filters" | "orders", linkId?: string) {
  return `${type}:${linkId ?? "primary"}`;
}

function parseRuleNodeKey(key: string):
  | {
      type: "filters" | "orders";
      linkId?: string;
    }
  | undefined {
  const [type, id] = key.split(":");
  if (type !== "filters" && type !== "orders") return undefined;
  return id && id !== "primary" ? { type, linkId: id } : { type };
}

function getRuleNodeCount(
  model: FetchQueryModel,
  type: "filters" | "orders",
  linkId?: string,
) {
  if (linkId) {
    const link = findLinkById(model.links, linkId);
    if (!link) return 0;
    return type === "filters"
      ? (link.conditions ?? []).length
      : (link.orders ?? []).length;
  }

  return type === "filters" ? model.conditions.length : model.orders.length;
}

function findLinkById(
  links: FetchLinkEntitySelection[],
  linkId: string,
): FetchLinkEntitySelection | undefined {
  for (const link of links) {
    if (link.id === linkId) return link;
    const child = findLinkById(link.links ?? [], linkId);
    if (child) return child;
  }
  return undefined;
}

function updateLinkById(
  links: FetchLinkEntitySelection[],
  linkId: string,
  updater: (link: FetchLinkEntitySelection) => FetchLinkEntitySelection,
): FetchLinkEntitySelection[] {
  return links.map((link) => {
    if (link.id === linkId) return updater(link);
    return {
      ...link,
      links: updateLinkById(link.links ?? [], linkId, updater),
    };
  });
}

function removeLinkById(
  links: FetchLinkEntitySelection[],
  linkId: string,
): FetchLinkEntitySelection[] {
  return links
    .filter((link) => link.id !== linkId)
    .map((link) => ({
      ...link,
      links: removeLinkById(link.links ?? [], linkId),
    }));
}

function normalizeLink(
  link: FetchLinkEntitySelection,
): FetchLinkEntitySelection {
  return {
    ...link,
    filterType: link.filterType ?? "and",
    conditions: link.conditions ?? [],
    orders: link.orders ?? [],
    links: (link.links ?? []).map(normalizeLink),
  };
}

function normalizeModel(model: FetchQueryModel): FetchQueryModel {
  return {
    ...model,
    distinct: model.distinct ?? false,
    filterType: model.filterType ?? "and",
    links: (model.links ?? []).map(normalizeLink),
  };
}

function safeReadModel(fetchXml: string): FetchQueryModel {
  try {
    return readFetchQueryModel(fetchXml);
  } catch {
    return {
      entity: "account",
      top: "50",
      distinct: false,
      filterType: "and",
      attributes: [{ name: "name" }],
      conditions: [],
      orders: [],
      links: [],
    };
  }
}
