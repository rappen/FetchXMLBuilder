import {
  type FetchConditionSelection,
  type FetchFilterGroup,
  type FetchLinkEntitySelection,
  type FetchOrderSelection,
  type FetchQueryModel,
  readFetchQueryModel,
  writeFetchQueryModel,
} from "@fetchxmlbuilder/core";
import type {
  AttributeSummary,
  EntitySummary,
  RelationshipSummary,
} from "@fetchxmlbuilder/dataverse";
import {
  AlertTriangle,
  ArrowDownAZ,
  ArrowUpAZ,
  BadgeCheck,
  Boxes,
  Copy,
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
import { type ReactNode, useEffect, useMemo, useRef, useState } from "react";
import { XmlEditor } from "./XmlEditor";

interface QueryBuilderPanelProps {
  fetchXml: string;
  entities: EntitySummary[];
  attributesByEntity: Record<string, AttributeSummary[]>;
  relationshipsByEntity: Record<string, RelationshipSummary[]>;
  loadingAttributeEntity: string;
  loadingRelationshipEntity: string;
  onChange: (fetchXml: string) => void;
  onEntitySelected: (entityName: string) => void;
  onRelationshipsNeeded: (entityName: string) => void;
  onWarningsChange?: (warningCount: number) => void;
}

type SelectedNode =
  | { type: "fetch" }
  | { type: "entity"; linkId?: string }
  | { type: "attributes"; linkId?: string }
  | { type: "filters"; linkId?: string; filterId?: string }
  | { type: "orders"; linkId?: string };

interface DraftLink {
  parentLinkId?: string;
  link: FetchLinkEntitySelection;
}

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
  relationshipsByEntity,
  loadingAttributeEntity,
  loadingRelationshipEntity,
  onChange,
  onEntitySelected,
  onRelationshipsNeeded,
  onWarningsChange,
}: QueryBuilderPanelProps) {
  const baseModel = useMemo(
    () => normalizeModel(safeReadModel(fetchXml)),
    [fetchXml],
  );
  const [draftLinks, setDraftLinks] = useState<DraftLink[]>([]);
  const model = useMemo(
    () => ({
      ...baseModel,
      links: mergeDraftLinks(baseModel.links, draftLinks),
    }),
    [baseModel, draftLinks],
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
  const [relationshipSearch, setRelationshipSearch] = useState("");
  const [entitySearchFocusLinkId, setEntitySearchFocusLinkId] = useState("");
  const [isPrimaryEntityPickerOpen, setIsPrimaryEntityPickerOpen] =
    useState(false);
  const [attributePicker, setAttributePicker] = useState<null | {
    linkId?: string;
  }>(null);
  const [draftRuleNodes, setDraftRuleNodes] = useState<Set<string>>(
    () => new Set(),
  );
  const [warningRuleNodes, setWarningRuleNodes] = useState<Set<string>>(
    () => new Set(),
  );
  const [invalidRelationshipLinkIds, setInvalidRelationshipLinkIds] = useState<
    Set<string>
  >(() => new Set());
  const [showCompositor, setShowCompositor] = useState(true);
  const [showInspector, setShowInspector] = useState(true);
  const [isFetchCopied, setIsFetchCopied] = useState(false);

  const selectedLink =
    selectedNode.type !== "fetch" && selectedNode.linkId
      ? findLinkById(model.links, selectedNode.linkId)
      : undefined;
  const selectedFilter =
    selectedNode.type === "filters"
      ? getSelectedFilterGroup(
          model,
          selectedNode.linkId,
          selectedNode.filterId,
        )
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
  const parentEntityName = selectedLink
    ? (findParentEntityName(model, selectedLink.id) ?? model.entity)
    : model.entity;
  const selectedRelationships = selectedLink
    ? getRelationshipsBetweenEntities(
        parentEntityName,
        selectedLink.name,
        relationshipsByEntity,
      )
    : [];
  const isLoadingRelationships =
    Boolean(selectedLink) &&
    (loadingRelationshipEntity === parentEntityName ||
      loadingRelationshipEntity === selectedLink?.name);

  useEffect(() => {
    if (!selectedLink) return;
    onRelationshipsNeeded(parentEntityName);
    if (selectedLink.name) {
      onRelationshipsNeeded(selectedLink.name);
    }
  }, [onRelationshipsNeeded, parentEntityName, selectedLink]);

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
        return (
          getRuleNodeCount(model, node.type, node.linkId, node.filterId) === 0
        );
      }),
    [model, warningRuleNodes],
  );
  const visibleWarningRuleNodeSet = useMemo(
    () => new Set(visibleWarningRuleNodes),
    [visibleWarningRuleNodes],
  );
  const visibleInvalidRelationshipLinkIds = useMemo(
    () =>
      new Set(
        Array.from(invalidRelationshipLinkIds).filter((linkId) =>
          Boolean(findLinkById(model.links, linkId)),
        ),
      ),
    [invalidRelationshipLinkIds, model.links],
  );
  const blockingRuleNodes = useMemo(() => {
    const keys = new Set(visibleWarningRuleNodes);
    for (const key of draftRuleNodes) {
      const node = parseRuleNodeKey(key);
      if (!node) continue;
      if (node.linkId && !findLinkById(model.links, node.linkId)) {
        continue;
      }
      if (
        getRuleNodeCount(model, node.type, node.linkId, node.filterId) === 0
      ) {
        keys.add(key);
      }
    }
    if (
      isRuleNode(selectedNode) &&
      getRuleNodeCount(
        model,
        selectedNode.type,
        selectedNode.linkId,
        selectedNode.filterId,
      ) === 0
    ) {
      keys.add(
        makeRuleNodeKey(
          selectedNode.type,
          selectedNode.linkId,
          selectedNode.filterId,
        ),
      );
    }
    return keys;
  }, [draftRuleNodes, model, selectedNode, visibleWarningRuleNodes]);

  useEffect(() => {
    onWarningsChange?.(
      blockingRuleNodes.size + visibleInvalidRelationshipLinkIds.size,
    );
  }, [
    blockingRuleNodes.size,
    onWarningsChange,
    visibleInvalidRelationshipLinkIds.size,
  ]);

  useEffect(() => {
    if (!isFetchCopied) return;
    const timeout = window.setTimeout(() => setIsFetchCopied(false), 1600);
    return () => window.clearTimeout(timeout);
  }, [isFetchCopied]);

  function selectNode(nextNode: SelectedNode) {
    setSelectedNode((currentNode) => {
      if (isSameSelectedNode(currentNode, nextNode)) return nextNode;
      if (
        isRuleNode(currentNode) &&
        getRuleNodeCount(
          model,
          currentNode.type,
          currentNode.linkId,
          currentNode.filterId,
        ) === 0
      ) {
        const key = makeRuleNodeKey(
          currentNode.type,
          currentNode.linkId,
          currentNode.filterId,
        );
        setWarningRuleNodes((current) => new Set(current).add(key));
      }
      return nextNode;
    });
  }

  function update(nextModel: FetchQueryModel) {
    onChange(writeFetchQueryModel(normalizeModel(nextModel)));
  }

  function updateModel(nextModel: FetchQueryModel) {
    setDraftLinks(collectIncompleteLinks(nextModel.links));
    update(nextModel);
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
    setInvalidRelationshipLinkIds((current) => {
      const next = new Set(current);
      next.delete(linkId);
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
    setIsPrimaryEntityPickerOpen(false);
    setInvalidRelationshipLinkIds((current) =>
      getInvalidDirectRelationshipLinksAfterPrimaryChange(
        model,
        nextEntity.logicalName,
        relationshipsByEntity,
        current,
      ),
    );
    updateModel({
      ...model,
      entity: nextEntity.logicalName,
      attributes: nextEntity.attributes.slice(0, 3).map((attribute) => ({
        name: attribute.logicalName,
      })),
      conditions: [],
      filters: [],
      orders: [],
    });
  }

  function updateLink(
    linkId: string,
    patch: Partial<FetchLinkEntitySelection>,
  ) {
    updateModel({
      ...model,
      links: updateLinkById(model.links, linkId, (link) =>
        normalizeLink({ ...link, ...patch }),
      ),
    });
  }

  function setLinkEntity(linkId: string, entityName: string) {
    onEntitySelected(entityName);
    onRelationshipsNeeded(entityName);
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
      filters: [],
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

  function updateFilterNode(
    linkId: string | undefined,
    filterId: string | undefined,
    updater: (filter: FetchFilterGroup) => FetchFilterGroup,
  ) {
    updateModel(updateFilterGroupInModel(model, linkId, filterId, updater));
  }

  function addCondition(linkId?: string, filterId?: string) {
    const attributes = getBuilderEntity(
      linkId
        ? (findLinkById(model.links, linkId)?.name ?? model.entity)
        : model.entity,
      entities,
      attributesByEntity,
    ).attributes;
    const condition = makeCondition(attributes[0]?.logicalName ?? "name");

    updateFilterNode(linkId, filterId, (filter) => ({
      ...filter,
      conditions: [...filter.conditions, condition],
    }));
    clearRuleNodeWarning("filters", linkId);
    selectNode(makeSelectedNode("filters", linkId, filterId));
  }

  function updateCondition(
    conditionId: string,
    patch: Partial<FetchConditionSelection>,
    linkId?: string,
    filterId?: string,
  ) {
    updateFilterNode(linkId, filterId, (filter) => ({
      ...filter,
      conditions: filter.conditions.map((condition) =>
        condition.id === conditionId ? { ...condition, ...patch } : condition,
      ),
    }));
  }

  function addFilterGroup(linkId?: string, filterId?: string) {
    const parentFilter = getSelectedFilterGroup(model, linkId, filterId);
    const group = makeFilterGroup(
      getNextFilterGroupId(filterId, parentFilter?.filters.length ?? 0),
    );
    updateFilterNode(linkId, filterId, (filter) => ({
      ...filter,
      filters: [...filter.filters, group],
    }));
    clearRuleNodeWarning("filters", linkId);
    selectNode(makeSelectedNode("filters", linkId, group.id));
  }

  function updateFilterType(
    filterType: "and" | "or",
    linkId?: string,
    filterId?: string,
  ) {
    updateFilterNode(linkId, filterId, (filter) => ({
      ...filter,
      type: filterType,
    }));
  }

  function removeFilterGroup(filterId: string, linkId?: string) {
    updateModel(removeFilterGroupFromModel(model, linkId, filterId));
    if (selectedNode.type === "filters" && selectedNode.filterId === filterId) {
      selectNode(makeSelectedNode("filters", linkId));
    }
  }

  function removeCondition(
    conditionId: string,
    linkId?: string,
    filterId?: string,
  ) {
    updateFilterNode(linkId, filterId, (filter) => ({
      ...filter,
      conditions: filter.conditions.filter(
        (condition) => condition.id !== conditionId,
      ),
    }));
  }

  function clearFilters(linkId?: string) {
    if (linkId) {
      updateLink(linkId, { conditions: [], filters: [] });
      clearRuleNodeWarning("filters", linkId);
      selectNode({ type: "entity", linkId });
      return;
    }

    update({ ...model, conditions: [], filters: [] });
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
    updateModel({
      ...model,
      links: removeLinkById(model.links, linkId),
    });
    clearLinkRuleNodeState(linkId);
    if ("linkId" in selectedNode && selectedNode.linkId === linkId) {
      selectNode({ type: "entity" });
    }
  }

  function addLink() {
    const parentEntityName = selectedLink?.name ?? model.entity;
    if (parentEntityName) {
      onRelationshipsNeeded(parentEntityName);
    }
    const parentLinkId = selectedLink?.id;
    const nextLinkId = getNextLinkId(model.links, parentLinkId);
    const nextLink = normalizeLink({
      id: nextLinkId,
      name: "",
      from: "",
      to: "",
      alias: "",
      linkType: "outer",
      attributes: [],
      filterType: "and",
      conditions: [],
      filters: [],
      orders: [],
      links: [],
    });
    setEntitySearch("");
    setRelationshipSearch("");
    setDraftLinks((current) => [
      ...current,
      {
        ...(parentLinkId ? { parentLinkId } : {}),
        link: nextLink,
      },
    ]);
    setEntitySearchFocusLinkId(nextLink.id);
    selectNode({ type: "entity", linkId: nextLink.id });
  }

  async function copyFetchXml() {
    await copyToClipboard(fetchXml);
    setIsFetchCopied(true);
  }

  return (
    <section
      className={[
        "builder-workbench",
        showCompositor ? "" : "hide-compositor",
        showInspector ? "" : "hide-inspector",
      ]
        .filter(Boolean)
        .join(" ")}
      aria-label="Visual query builder"
    >
      <section className="fetch-preview-panel panel" aria-label="FetchXML">
        <div className="panel-heading builder-heading">
          <div>
            <h2>FetchXML</h2>
            <span>Editable query</span>
          </div>
          <div className="builder-heading-actions">
            <button
              type="button"
              title={isFetchCopied ? "Copied" : "Copy FetchXML"}
              aria-label="Copy FetchXML"
              onClick={copyFetchXml}
            >
              <Copy size={16} />
            </button>
            <button
              className={showCompositor ? "active" : ""}
              type="button"
              title={showCompositor ? "Hide compositor" : "Show compositor"}
              aria-label={
                showCompositor ? "Hide compositor" : "Show compositor"
              }
              aria-pressed={showCompositor}
              onClick={() => setShowCompositor(!showCompositor)}
            >
              <Layers3 size={16} />
            </button>
            <button
              className={showInspector ? "active" : ""}
              type="button"
              title={showInspector ? "Hide inspector" : "Show inspector"}
              aria-label={showInspector ? "Hide inspector" : "Show inspector"}
              aria-pressed={showInspector}
              onClick={() => setShowInspector(!showInspector)}
            >
              <Settings2 size={16} />
            </button>
          </div>
        </div>
        <XmlEditor value={fetchXml} onChange={onChange} />
      </section>

      {showCompositor ? (
        <div className="composition-panel panel">
          <div className="panel-heading builder-heading">
            <div>
              <h2>Compositor</h2>
              <span>Graphic map of what the FetchXML will do</span>
            </div>
          </div>
          <div className="query-tree" role="tree">
            <TreeButton
              active={selectedNode.type === "fetch"}
              icon={<Settings2 size={17} />}
              label="Fetch"
              meta={getFetchNodeMeta(model)}
              tone="fetch"
              onClick={() => selectNode({ type: "fetch" })}
            />
            <div className="tree-children">
              <EntityBranch
                entityName={model.entity}
                displayName={primaryEntity.displayName}
                attributes={model.attributes.length}
                conditions={model.conditions.length}
                filters={model.filters}
                filterType={model.filterType}
                links={model.links}
                orders={model.orders.length}
                entities={entities}
                attributesByEntity={attributesByEntity}
                selectedNode={selectedNode}
                draftRuleNodes={draftRuleNodes}
                warningRuleNodes={visibleWarningRuleNodeSet}
                invalidRelationshipLinkIds={visibleInvalidRelationshipLinkIds}
                onClearAttributes={clearAttributes}
                onClearFilters={clearFilters}
                onClearOrders={clearOrders}
                onRemoveFilterGroup={removeFilterGroup}
                onRemoveEntity={removeLink}
                onSelect={selectNode}
              />
            </div>
          </div>
        </div>
      ) : null}

      {showInspector ? (
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
              <FetchInspector
                model={model}
                onAddLink={addLink}
                onCreateFilter={() => addCondition()}
                onCreateOrder={() => addOrder()}
                onOpenAttributes={() => selectNode({ type: "attributes" })}
                onOpenEntity={() => selectNode({ type: "entity" })}
                onUpdate={update}
              />
            ) : null}
            {selectedNode.type === "entity" ? (
              <EntityInspector
                entity={selectedEntity}
                entityOptions={entityOptions}
                entitySearch={entitySearch}
                autoFocusEntitySearch={
                  selectedNode.linkId === entitySearchFocusLinkId ||
                  (selectedNode.type === "entity" &&
                    !selectedNode.linkId &&
                    isPrimaryEntityPickerOpen)
                }
                isEntityPickerOpen={
                  selectedNode.linkId
                    ? !selectedLink?.name
                    : isPrimaryEntityPickerOpen
                }
                relationshipSearch={relationshipSearch}
                relationships={selectedRelationships}
                isPrimary={!selectedNode.linkId}
                isLoadingAttributes={isLoadingAttributes}
                isLoadingRelationships={isLoadingRelationships}
                model={model}
                parentEntityName={parentEntityName}
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
                onChangePrimaryEntity={() => {
                  setEntitySearch("");
                  setIsPrimaryEntityPickerOpen(true);
                }}
                onSearchChange={setEntitySearch}
                onEntitySearchFocused={() => {
                  setEntitySearchFocusLinkId("");
                }}
                onRelationshipSearchChange={setRelationshipSearch}
                onClearLinkedEntity={() => {
                  if (!selectedNode.linkId) return;
                  setEntitySearch("");
                  setRelationshipSearch("");
                  updateLink(selectedNode.linkId, {
                    name: "",
                    from: "",
                    to: "",
                    alias: "",
                    attributes: [],
                    conditions: [],
                    filters: [],
                    orders: [],
                    links: [],
                  });
                  setEntitySearchFocusLinkId(selectedNode.linkId);
                }}
                onSelectEntity={(entityName) =>
                  selectedNode.linkId
                    ? setLinkEntity(selectedNode.linkId, entityName)
                    : setPrimaryEntity(entityName)
                }
                onSelectRelationship={(relationship) => {
                  if (!selectedNode.linkId) return;
                  updateLink(
                    selectedNode.linkId,
                    getJoinPatchFromRelationship(
                      parentEntityName,
                      relationship,
                    ),
                  );
                  setInvalidRelationshipLinkIds((current) => {
                    const next = new Set(current);
                    if (selectedNode.linkId) next.delete(selectedNode.linkId);
                    return next;
                  });
                }}
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
                conditions={selectedFilter?.conditions ?? []}
                filterType={selectedFilter?.type ?? "and"}
                groups={selectedFilter?.filters ?? []}
                {...(selectedNode.linkId
                  ? { linkId: selectedNode.linkId }
                  : {})}
                {...(selectedNode.filterId
                  ? { filterId: selectedNode.filterId }
                  : {})}
                onAdd={() =>
                  addCondition(selectedNode.linkId, selectedNode.filterId)
                }
                onAddGroup={() =>
                  addFilterGroup(selectedNode.linkId, selectedNode.filterId)
                }
                onDelete={() => {
                  if (selectedNode.filterId) {
                    removeFilterGroup(
                      selectedNode.filterId,
                      selectedNode.linkId,
                    );
                    return;
                  }
                  clearFilters(selectedNode.linkId);
                }}
                onFilterTypeChange={(filterType) =>
                  updateFilterType(
                    filterType,
                    selectedNode.linkId,
                    selectedNode.filterId,
                  )
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
                onDelete={() => clearOrders(selectedNode.linkId)}
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
                          orderIndex === index
                            ? { ...order, ...patch }
                            : order,
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
      ) : null}

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
  filters,
  filterType,
  links,
  orders,
  entities,
  attributesByEntity,
  selectedNode,
  draftRuleNodes,
  warningRuleNodes,
  invalidRelationshipLinkIds,
  onClearAttributes,
  onClearFilters,
  onClearOrders,
  onRemoveFilterGroup,
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
  filters: FetchFilterGroup[];
  filterType: "and" | "or";
  links: FetchLinkEntitySelection[];
  orders: number;
  entities: EntitySummary[];
  attributesByEntity: Record<string, AttributeSummary[]>;
  selectedNode: SelectedNode;
  draftRuleNodes: Set<string>;
  warningRuleNodes: Set<string>;
  invalidRelationshipLinkIds: Set<string>;
  onClearAttributes: (linkId?: string) => void;
  onClearFilters: (linkId?: string) => void;
  onClearOrders: (linkId?: string) => void;
  onRemoveFilterGroup: (filterId: string, linkId?: string) => void;
  onRemoveEntity?: (linkId: string) => void;
  onSelect: (node: SelectedNode) => void;
}) {
  const attributesNode = makeSelectedNode("attributes", linkId);
  const filterGroup = makeRootFilterGroup(filterType, conditions, filters);
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
    filters.length > 0 ||
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
  const hasInvalidRelationshipJoin = linkId
    ? invalidRelationshipLinkIds.has(linkId)
    : false;
  const entityMeta = hasInvalidRelationshipJoin
    ? "Relationship join needs review"
    : [
        entityName,
        alias ? `alias ${alias}` : "",
        linkType ? `${linkType} join` : "primary",
      ]
        .filter(Boolean)
        .join(" · ");

  return (
    <div className="entity-branch">
      <TreeButton
        active={
          selectedNode.type === "entity" && selectedNode.linkId === linkId
        }
        icon={
          hasInvalidRelationshipJoin ? (
            <AlertTriangle size={17} />
          ) : linkId ? (
            <GitFork size={17} />
          ) : (
            <Boxes size={17} />
          )
        }
        label={displayName || entityName || "New linked entity"}
        meta={entityMeta}
        tone={
          hasInvalidRelationshipJoin ? "warning" : linkId ? "link" : "entity"
        }
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
            <FilterTreeBranch
              filter={filterGroup}
              label="Filters"
              {...(linkId ? { linkId } : {})}
              selectedNode={selectedNode}
              warning={warningRuleNodes.has(filterKey)}
              onDelete={() => onClearFilters(linkId)}
              onRemoveGroup={onRemoveFilterGroup}
              onSelect={onSelect}
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
                filters={link.filters ?? []}
                filterType={link.filterType ?? "and"}
                links={link.links ?? []}
                orders={(link.orders ?? []).length}
                entities={entities}
                attributesByEntity={attributesByEntity}
                selectedNode={selectedNode}
                draftRuleNodes={draftRuleNodes}
                warningRuleNodes={warningRuleNodes}
                invalidRelationshipLinkIds={invalidRelationshipLinkIds}
                onClearAttributes={onClearAttributes}
                onClearFilters={onClearFilters}
                onClearOrders={onClearOrders}
                onRemoveFilterGroup={onRemoveFilterGroup}
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

function FilterTreeBranch({
  filter,
  label,
  linkId,
  selectedNode,
  warning,
  onDelete,
  onRemoveGroup,
  onSelect,
}: {
  filter: FetchFilterGroup;
  label: string;
  linkId?: string;
  selectedNode: SelectedNode;
  warning: boolean;
  onDelete: () => void;
  onRemoveGroup: (filterId: string, linkId?: string) => void;
  onSelect: (node: SelectedNode) => void;
}) {
  const isRootFilter = filter.id === "root-filter";
  const filterNode = makeSelectedNode(
    "filters",
    linkId,
    isRootFilter ? undefined : filter.id,
  );
  const isActive = isSameSelectedNode(selectedNode, filterNode);

  return (
    <div className="filter-branch">
      <TreeButton
        active={isActive}
        icon={warning ? <AlertTriangle size={16} /> : <Filter size={16} />}
        label={label}
        meta={
          warning ? "Missing conditions or groups" : getFilterGroupMeta(filter)
        }
        tone={warning ? "warning" : "leaf"}
        deleteTitle={isRootFilter ? "Remove filter node" : "Remove subgroup"}
        onDelete={
          isRootFilter ? onDelete : () => onRemoveGroup(filter.id, linkId)
        }
        onClick={() => onSelect(filterNode)}
      />
      {filter.filters.length > 0 ? (
        <div className="tree-children slim">
          {filter.filters.map((childFilter, index) => (
            <FilterTreeBranch
              key={childFilter.id}
              filter={childFilter}
              label={`Subgroup ${index + 1}`}
              {...(linkId ? { linkId } : {})}
              selectedNode={selectedNode}
              warning={false}
              onDelete={onDelete}
              onRemoveGroup={onRemoveGroup}
              onSelect={onSelect}
            />
          ))}
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
            : entity.logicalName || "Choose an entity"}
        </span>
      </div>
    </div>
  );
}

function FetchInspector({
  model,
  onAddLink,
  onCreateFilter,
  onCreateOrder,
  onOpenAttributes,
  onOpenEntity,
  onUpdate,
}: {
  model: FetchQueryModel;
  onAddLink: () => void;
  onCreateFilter: () => void;
  onCreateOrder: () => void;
  onOpenAttributes: () => void;
  onOpenEntity: () => void;
  onUpdate: (model: FetchQueryModel) => void;
}) {
  const updateNumericProperty = (
    property: "top" | "count" | "page",
    value: string,
  ) => onUpdate({ ...model, [property]: value.replace(/\D/g, "") });

  return (
    <div className="inspector-stack">
      <label>
        <span>Top</span>
        <input
          inputMode="numeric"
          placeholder="All rows"
          value={model.top}
          onChange={(event) => updateNumericProperty("top", event.target.value)}
        />
      </label>
      <div className="fetch-switch-grid">
        <label className="switch-row">
          <input
            type="checkbox"
            checked={model.distinct}
            onChange={(event) =>
              onUpdate({ ...model, distinct: event.target.checked })
            }
          />
          <span>Distinct</span>
        </label>
        <label className="switch-row">
          <input
            type="checkbox"
            checked={model.returnTotalRecordCount}
            onChange={(event) =>
              onUpdate({
                ...model,
                returnTotalRecordCount: event.target.checked,
              })
            }
          />
          <span>Total record count</span>
        </label>
        <label className="switch-row">
          <input
            type="checkbox"
            checked={model.orderByRawValue}
            onChange={(event) =>
              onUpdate({ ...model, orderByRawValue: event.target.checked })
            }
          />
          <span>Order by Raw value</span>
        </label>
      </div>
      <fieldset className="fetch-paging-group">
        <legend>Paging</legend>
        <div className="form-grid compact-form">
          <label>
            <span>Count</span>
            <input
              inputMode="numeric"
              value={model.count}
              onChange={(event) =>
                updateNumericProperty("count", event.target.value)
              }
            />
          </label>
          <label>
            <span>Page</span>
            <input
              inputMode="numeric"
              value={model.page}
              onChange={(event) =>
                updateNumericProperty("page", event.target.value)
              }
            />
          </label>
        </div>
        <label>
          <span>Paging Cookie</span>
          <input
            value={model.pagingCookie}
            onChange={(event) =>
              onUpdate({ ...model, pagingCookie: event.target.value })
            }
          />
        </label>
      </fieldset>
      <div className="action-grid">
        <button type="button" onClick={onOpenEntity}>
          <Boxes size={16} />
          <span>Primary entity</span>
        </button>
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
    </div>
  );
}

function EntityInspector({
  entity,
  entityOptions,
  entitySearch,
  autoFocusEntitySearch,
  isEntityPickerOpen,
  relationshipSearch,
  relationships,
  isPrimary,
  isLoadingAttributes,
  isLoadingRelationships,
  link,
  model,
  parentEntityName,
  onAddLink,
  onCreateFilter,
  onCreateOrder,
  onOpenAttributes,
  onRemoveLink,
  onChangePrimaryEntity,
  onClearLinkedEntity,
  onEntitySearchFocused,
  onRelationshipSearchChange,
  onSearchChange,
  onSelectEntity,
  onSelectRelationship,
  onUpdateLink,
}: {
  entity: BuilderEntity;
  entityOptions: EntitySummary[];
  entitySearch: string;
  autoFocusEntitySearch: boolean;
  isEntityPickerOpen: boolean;
  relationshipSearch: string;
  relationships: RelationshipSummary[];
  isPrimary: boolean;
  isLoadingAttributes: boolean;
  isLoadingRelationships: boolean;
  link?: FetchLinkEntitySelection;
  model: FetchQueryModel;
  parentEntityName: string;
  onAddLink: () => void;
  onCreateFilter: () => void;
  onCreateOrder: () => void;
  onOpenAttributes: () => void;
  onRemoveLink: () => void;
  onChangePrimaryEntity: () => void;
  onClearLinkedEntity: () => void;
  onEntitySearchFocused: () => void;
  onRelationshipSearchChange: (value: string) => void;
  onSearchChange: (value: string) => void;
  onSelectEntity: (entityName: string) => void;
  onSelectRelationship: (relationship: RelationshipSummary) => void;
  onUpdateLink: (patch: Partial<FetchLinkEntitySelection>) => void;
}) {
  const entitySearchRef = useRef<HTMLInputElement>(null);
  const matches = filterEntities(entityOptions, entitySearch).slice(0, 18);
  const selectedRelationship = link
    ? findMatchingRelationship(parentEntityName, link, relationships)
    : undefined;
  const relationshipValue = selectedRelationship
    ? getRelationshipKey(selectedRelationship)
    : "custom";
  const visibleRelationships = includeRelationship(
    filterRelationships(
      parentEntityName,
      relationships,
      relationshipSearch,
    ).slice(0, 40),
    selectedRelationship,
  );
  const relationshipGroups = groupRelationshipsByCardinality(
    parentEntityName,
    visibleRelationships,
  );
  const showEntityPicker = isEntityPickerOpen;
  const selectedEntityCardTitle = isPrimary
    ? "Primary entity"
    : "Linked entity";

  useEffect(() => {
    if (!autoFocusEntitySearch) return;
    entitySearchRef.current?.focus();
    onEntitySearchFocused();
  }, [autoFocusEntitySearch, onEntitySearchFocused]);

  return (
    <div className="inspector-stack">
      {showEntityPicker ? (
        <>
          <label>
            <span>{isPrimary ? "Primary entity" : "Linked entity"}</span>
            <div className="search-box inspector-search">
              <Search size={15} />
              <input
                ref={entitySearchRef}
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
        </>
      ) : (
        <div className="selected-entity-card">
          <div>
            <span>{selectedEntityCardTitle}</span>
            <strong>{entity.displayName || entity.logicalName}</strong>
            <small>{entity.logicalName}</small>
          </div>
          <button
            type="button"
            title={isPrimary ? "Change primary entity" : "Clear linked entity"}
            aria-label={
              isPrimary ? "Change primary entity" : "Clear linked entity"
            }
            onClick={isPrimary ? onChangePrimaryEntity : onClearLinkedEntity}
          >
            <Trash2 size={16} />
          </button>
        </div>
      )}

      {!isPrimary && link?.name ? (
        <>
          <div className="relationship-picker">
            <label>
              <span>Relationship</span>
              <div className="search-box inspector-search">
                <Search size={15} />
                <input
                  value={relationshipSearch}
                  placeholder="Search relationship or join keys"
                  onChange={(event) =>
                    onRelationshipSearchChange(event.target.value)
                  }
                />
              </div>
            </label>
            <select
              value={relationshipValue}
              onChange={(event) => {
                if (event.target.value === "custom") return;
                const relationship = relationships.find(
                  (item) => getRelationshipKey(item) === event.target.value,
                );
                if (relationship) onSelectRelationship(relationship);
              }}
            >
              <option value="custom">Custom</option>
              {relationshipGroups.map((group) => (
                <optgroup key={group.cardinality} label={group.label}>
                  {group.relationships.map((relationship) => (
                    <option
                      key={getRelationshipKey(relationship)}
                      value={getRelationshipKey(relationship)}
                    >
                      {formatRelationshipStatement(
                        parentEntityName,
                        relationship,
                      )}
                    </option>
                  ))}
                </optgroup>
              ))}
            </select>
            <small className="inspector-note">
              {isLoadingRelationships
                ? "Loading relationships"
                : `${relationships.length} matching relationship${relationships.length === 1 ? "" : "s"}`}
            </small>
          </div>
          <div className="form-grid compact-form">
            <label>
              <span>Alias</span>
              <input
                value={link.alias}
                onChange={(event) =>
                  onUpdateLink({ alias: event.target.value })
                }
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
        </>
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
  groups,
  linkId,
  filterId,
  onAdd,
  onAddGroup,
  onDelete,
  onFilterTypeChange,
  onRemove,
  onUpdate,
}: {
  attributes: AttributeSummary[];
  conditions: FetchConditionSelection[];
  filterType: "and" | "or";
  groups: FetchFilterGroup[];
  linkId?: string;
  filterId?: string;
  onAdd: () => void;
  onAddGroup: () => void;
  onDelete: () => void;
  onFilterTypeChange: (filterType: "and" | "or") => void;
  onRemove: (conditionId: string, linkId?: string, filterId?: string) => void;
  onUpdate: (
    conditionId: string,
    patch: Partial<FetchConditionSelection>,
    linkId?: string,
    filterId?: string,
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
        {conditions.length === 0 && groups.length === 0 ? (
          <p className="empty-state compact warning-empty">
            Add at least one condition or subgroup before leaving this filter.
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
                      filterId,
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
                      filterId,
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
                        filterId,
                      )
                    }
                  />
                </label>
              ) : null}
              <button
                className="icon-button danger-action"
                type="button"
                title="Remove condition"
                onClick={() => onRemove(condition.id, linkId, filterId)}
              >
                <Trash2 size={15} />
              </button>
            </div>
          );
        })}
        {groups.length > 0
          ? groups.map((group, index) => (
              <div className="filter-card filter-subgroup-card" key={group.id}>
                <span className="tree-node-icon">
                  <Filter size={16} />
                </span>
                <span className="filter-subgroup-copy">
                  <strong>Subgroup {index + 1}</strong>
                  <small>{getFilterGroupMeta(group)}</small>
                </span>
              </div>
            ))
          : null}
      </div>
      <div className="action-grid">
        <button type="button" onClick={onAdd}>
          <Plus size={16} />
          <span>Add condition</span>
        </button>
        <button type="button" onClick={onAddGroup}>
          <Filter size={16} />
          <span>Add subgroup</span>
        </button>
      </div>
      <button className="danger-action" type="button" onClick={onDelete}>
        <Trash2 size={16} />
        <span>{filterId ? "Remove subgroup" : "Remove filter"}</span>
      </button>
    </div>
  );
}

function OrderInspector({
  attributes,
  orders,
  onAdd,
  onDelete,
  onRemove,
  onUpdate,
}: {
  attributes: AttributeSummary[];
  orders: FetchOrderSelection[];
  onAdd: () => void;
  onDelete: () => void;
  onRemove: (index: number) => void;
  onUpdate: (index: number, patch: Partial<FetchOrderSelection>) => void;
}) {
  return (
    <div className="inspector-stack">
      {orders.map((order, index) => {
        const selectedAttribute = attributes.find(
          (attribute) => attribute.logicalName === order.attribute,
        );
        const directionLabels = getOrderDirectionLabels(selectedAttribute);
        return (
          <div className="order-card" key={`${order.attribute}-${index}`}>
            <div className="order-card-header">
              <span>Attribute</span>
              <button
                className="icon-button danger-action"
                type="button"
                title="Remove order"
                onClick={() => onRemove(index)}
              >
                <Trash2 size={13} />
              </button>
            </div>
            <label>
              <select
                value={order.attribute}
                onChange={(event) =>
                  onUpdate(index, { attribute: event.target.value })
                }
              >
                {attributes.map((attribute) => (
                  <option
                    key={attribute.logicalName}
                    value={attribute.logicalName}
                  >
                    {attribute.displayName || attribute.logicalName}
                  </option>
                ))}
              </select>
            </label>
            <div className="segmented order-direction-toggle">
              <button
                className={!order.descending ? "active" : ""}
                type="button"
                onClick={() => onUpdate(index, { descending: false })}
              >
                <ArrowUpAZ size={15} />
                <span className="order-direction-copy">
                  <strong>{directionLabels.ascending.title}</strong>
                  <small>{directionLabels.ascending.detail}</small>
                </span>
              </button>
              <button
                className={order.descending ? "active" : ""}
                type="button"
                onClick={() => onUpdate(index, { descending: true })}
              >
                <ArrowDownAZ size={15} />
                <span className="order-direction-copy">
                  <strong>{directionLabels.descending.title}</strong>
                  <small>{directionLabels.descending.detail}</small>
                </span>
              </button>
            </div>
          </div>
        );
      })}
      {orders.length === 0 ? (
        <p className="empty-state compact warning-empty">
          Add at least one sort rule before leaving this order.
        </p>
      ) : null}
      <button type="button" onClick={onAdd}>
        <Plus size={16} />
        <span>Add order</span>
      </button>
      <button className="danger-action" type="button" onClick={onDelete}>
        <Trash2 size={16} />
        <span>Remove order</span>
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

function filterRelationships(
  parentEntityName: string,
  relationships: RelationshipSummary[],
  query: string,
) {
  const normalizedQuery = query.trim().toLowerCase();
  if (!normalizedQuery) return relationships;
  return relationships.filter((relationship) =>
    [
      relationship.schemaName,
      relationship.referencedEntity,
      relationship.referencedAttribute,
      relationship.referencingEntity,
      relationship.referencingAttribute,
      getRelationshipCardinality(parentEntityName, relationship),
      formatRelationshipStatement(parentEntityName, relationship),
    ].some((value) => value.toLowerCase().includes(normalizedQuery)),
  );
}

function getRelationshipsBetweenEntities(
  parentEntityName: string,
  childEntityName: string,
  relationshipsByEntity: Record<string, RelationshipSummary[]>,
) {
  const relationships = [
    ...(relationshipsByEntity[parentEntityName] ?? []),
    ...(relationshipsByEntity[childEntityName] ?? []),
  ];
  const seen = new Set<string>();
  return relationships.filter((relationship) => {
    const connectsEntities =
      (relationship.referencedEntity === parentEntityName &&
        relationship.referencingEntity === childEntityName) ||
      (relationship.referencedEntity === childEntityName &&
        relationship.referencingEntity === parentEntityName);
    const key = getRelationshipKey(relationship);
    if (!connectsEntities || seen.has(key)) return false;
    seen.add(key);
    return true;
  });
}

function getJoinPatchFromRelationship(
  parentEntityName: string,
  relationship: RelationshipSummary,
): Pick<FetchLinkEntitySelection, "from" | "to"> {
  if (relationship.referencedEntity === parentEntityName) {
    return {
      from: relationship.referencingAttribute,
      to: relationship.referencedAttribute,
    };
  }
  return {
    from: relationship.referencedAttribute,
    to: relationship.referencingAttribute,
  };
}

function getRelationshipCardinality(
  parentEntityName: string,
  relationship: RelationshipSummary,
): "1:N" | "N:1" {
  return relationship.referencedEntity === parentEntityName ? "1:N" : "N:1";
}

function findMatchingRelationship(
  parentEntityName: string,
  link: FetchLinkEntitySelection,
  relationships: RelationshipSummary[],
) {
  return relationships.find((relationship) => {
    const join = getJoinPatchFromRelationship(parentEntityName, relationship);
    return join.from === link.from && join.to === link.to;
  });
}

function isRelationshipBackedJoin(
  parentEntityName: string,
  link: FetchLinkEntitySelection,
  relationshipsByEntity: Record<string, RelationshipSummary[]>,
) {
  if (!link.name || !link.from || !link.to) return false;
  return Boolean(
    findMatchingRelationship(
      parentEntityName,
      link,
      getRelationshipsBetweenEntities(
        parentEntityName,
        link.name,
        relationshipsByEntity,
      ),
    ),
  );
}

function getInvalidDirectRelationshipLinksAfterPrimaryChange(
  model: FetchQueryModel,
  nextPrimaryEntityName: string,
  relationshipsByEntity: Record<string, RelationshipSummary[]>,
  currentInvalidLinkIds: Set<string>,
) {
  const next = new Set(currentInvalidLinkIds);
  for (const link of model.links) {
    if (currentInvalidLinkIds.has(link.id)) {
      if (
        isRelationshipBackedJoin(
          nextPrimaryEntityName,
          link,
          relationshipsByEntity,
        )
      ) {
        next.delete(link.id);
      } else {
        next.add(link.id);
      }
      continue;
    }

    const wasRelationshipBacked = isRelationshipBackedJoin(
      model.entity,
      link,
      relationshipsByEntity,
    );
    if (!wasRelationshipBacked) continue;

    const isStillValid = isRelationshipBackedJoin(
      nextPrimaryEntityName,
      link,
      relationshipsByEntity,
    );
    if (isStillValid) {
      next.delete(link.id);
    } else {
      next.add(link.id);
    }
  }
  return next;
}

function getRelationshipKey(relationship: RelationshipSummary) {
  return [
    relationship.schemaName,
    relationship.referencedEntity,
    relationship.referencedAttribute,
    relationship.referencingEntity,
    relationship.referencingAttribute,
  ].join(":");
}

function formatRelationshipStatement(
  parentEntityName: string,
  relationship: RelationshipSummary,
) {
  if (relationship.referencedEntity === parentEntityName) {
    return `${relationship.referencingEntity}.${relationship.referencingAttribute} -> ${relationship.referencedEntity}.${relationship.referencedAttribute}`;
  }
  return `${relationship.referencedEntity}.${relationship.referencedAttribute} -> ${relationship.referencingEntity}.${relationship.referencingAttribute}`;
}

function groupRelationshipsByCardinality(
  parentEntityName: string,
  relationships: RelationshipSummary[],
) {
  return [
    {
      cardinality: "1:N" as const,
      label: "1:N relationships",
      relationships: relationships.filter(
        (relationship) =>
          getRelationshipCardinality(parentEntityName, relationship) === "1:N",
      ),
    },
    {
      cardinality: "N:1" as const,
      label: "N:1 relationships",
      relationships: relationships.filter(
        (relationship) =>
          getRelationshipCardinality(parentEntityName, relationship) === "N:1",
      ),
    },
  ].filter((group) => group.relationships.length > 0);
}

function includeRelationship(
  relationships: RelationshipSummary[],
  relationship?: RelationshipSummary,
) {
  if (
    !relationship ||
    relationships.some(
      (item) => getRelationshipKey(item) === getRelationshipKey(relationship),
    )
  ) {
    return relationships;
  }
  return [relationship, ...relationships];
}

function makeCondition(attribute: string): FetchConditionSelection {
  return {
    id: crypto.randomUUID(),
    attribute,
    operator: "eq",
    value: "",
  };
}

function makeFilterGroup(id: string): FetchFilterGroup {
  return {
    id,
    type: "and",
    conditions: [],
    filters: [],
  };
}

function getNextFilterGroupId(
  parentFilterId: string | undefined,
  index: number,
) {
  const nextIndex = index + 1;
  if (!parentFilterId) return `filter-${nextIndex}`;
  return `${parentFilterId}-${nextIndex}`;
}

function makeSelectedNode(
  type: "entity" | "attributes" | "filters" | "orders",
  linkId?: string,
  filterId?: string,
): SelectedNode {
  return {
    type,
    ...(linkId ? { linkId } : {}),
    ...(type === "filters" && filterId ? { filterId } : {}),
  };
}

function isRuleNode(
  node: SelectedNode,
): node is { type: "filters" | "orders"; linkId?: string; filterId?: string } {
  return node.type === "filters" || node.type === "orders";
}

function isSameSelectedNode(left: SelectedNode, right: SelectedNode) {
  const leftLinkId = "linkId" in left ? left.linkId : undefined;
  const rightLinkId = "linkId" in right ? right.linkId : undefined;
  const leftFilterId = "filterId" in left ? left.filterId : undefined;
  const rightFilterId = "filterId" in right ? right.filterId : undefined;
  return (
    left.type === right.type &&
    leftLinkId === rightLinkId &&
    leftFilterId === rightFilterId
  );
}

function makeRuleNodeKey(
  type: "filters" | "orders",
  linkId?: string,
  filterId?: string,
) {
  return `${type}:${linkId ?? "primary"}:${filterId ?? "root"}`;
}

function parseRuleNodeKey(key: string):
  | {
      type: "filters" | "orders";
      linkId?: string;
      filterId?: string;
    }
  | undefined {
  const [type, id, filterId] = key.split(":");
  if (type !== "filters" && type !== "orders") return undefined;
  return {
    type,
    ...(id && id !== "primary" ? { linkId: id } : {}),
    ...(filterId && filterId !== "root" ? { filterId } : {}),
  };
}

function getRuleNodeCount(
  model: FetchQueryModel,
  type: "filters" | "orders",
  linkId?: string,
  filterId?: string,
) {
  if (linkId) {
    const link = findLinkById(model.links, linkId);
    if (!link) return 0;
    if (type === "orders") return (link.orders ?? []).length;
    const filter = getSelectedFilterGroup(model, linkId, filterId);
    return filter ? getFilterRuleCount(filter) : 0;
  }

  if (type === "orders") return model.orders.length;
  const filter = getSelectedFilterGroup(model, undefined, filterId);
  return filter ? getFilterRuleCount(filter) : 0;
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

function makeRootFilterGroup(
  type: "and" | "or",
  conditionCount: number,
  filters: FetchFilterGroup[],
): FetchFilterGroup {
  return {
    id: "root-filter",
    type,
    conditions: Array.from({ length: conditionCount }, (_, index) => ({
      id: `summary-condition-${index}`,
      attribute: "",
      operator: "",
      value: "",
    })),
    filters,
  };
}

function getSelectedFilterGroup(
  model: FetchQueryModel,
  linkId?: string,
  filterId?: string,
): FetchFilterGroup | undefined {
  const owner = linkId ? findLinkById(model.links, linkId) : model;
  if (!owner) return undefined;
  const rootFilter: FetchFilterGroup = {
    id: "root-filter",
    type: owner.filterType ?? "and",
    conditions: owner.conditions ?? [],
    filters: owner.filters ?? [],
  };
  return filterId ? findFilterGroup(rootFilter.filters, filterId) : rootFilter;
}

function findFilterGroup(
  filters: FetchFilterGroup[],
  filterId: string,
): FetchFilterGroup | undefined {
  for (const filter of filters) {
    if (filter.id === filterId) return filter;
    const child = findFilterGroup(filter.filters ?? [], filterId);
    if (child) return child;
  }
  return undefined;
}

function updateFilterGroupInModel(
  model: FetchQueryModel,
  linkId: string | undefined,
  filterId: string | undefined,
  updater: (filter: FetchFilterGroup) => FetchFilterGroup,
): FetchQueryModel {
  if (linkId) {
    return {
      ...model,
      links: updateLinkById(model.links, linkId, (link) =>
        applyFilterUpdateToOwner(link, filterId, updater),
      ),
    };
  }
  return applyFilterUpdateToOwner(model, filterId, updater);
}

function applyFilterUpdateToOwner<
  T extends Pick<FetchQueryModel, "filterType" | "conditions" | "filters">,
>(
  owner: T,
  filterId: string | undefined,
  updater: (filter: FetchFilterGroup) => FetchFilterGroup,
): T {
  if (filterId) {
    return {
      ...owner,
      filters: updateFilterGroups(owner.filters ?? [], filterId, updater),
    };
  }

  const next = updater({
    id: "root-filter",
    type: owner.filterType ?? "and",
    conditions: owner.conditions ?? [],
    filters: owner.filters ?? [],
  });
  return {
    ...owner,
    filterType: next.type,
    conditions: next.conditions,
    filters: next.filters,
  };
}

function updateFilterGroups(
  filters: FetchFilterGroup[],
  filterId: string,
  updater: (filter: FetchFilterGroup) => FetchFilterGroup,
): FetchFilterGroup[] {
  return filters.map((filter) => {
    if (filter.id === filterId) return updater(filter);
    return {
      ...filter,
      filters: updateFilterGroups(filter.filters ?? [], filterId, updater),
    };
  });
}

function removeFilterGroupFromModel(
  model: FetchQueryModel,
  linkId: string | undefined,
  filterId: string,
): FetchQueryModel {
  if (linkId) {
    return {
      ...model,
      links: updateLinkById(model.links, linkId, (link) => ({
        ...link,
        filters: removeFilterGroupById(link.filters ?? [], filterId),
      })),
    };
  }
  return {
    ...model,
    filters: removeFilterGroupById(model.filters ?? [], filterId),
  };
}

function removeFilterGroupById(
  filters: FetchFilterGroup[],
  filterId: string,
): FetchFilterGroup[] {
  return filters
    .filter((filter) => filter.id !== filterId)
    .map((filter) => ({
      ...filter,
      filters: removeFilterGroupById(filter.filters ?? [], filterId),
    }));
}

function getFilterRuleCount(filter: FetchFilterGroup) {
  return filter.conditions.length + filter.filters.length;
}

function getFilterGroupMeta(filter: FetchFilterGroup) {
  const conditionCount = filter.conditions.length;
  const groupCount = filter.filters.length;
  const parts = [
    filter.type.toUpperCase(),
    conditionCount
      ? `${conditionCount} condition${conditionCount === 1 ? "" : "s"}`
      : "",
    groupCount ? `${groupCount} group${groupCount === 1 ? "" : "s"}` : "",
  ].filter(Boolean);
  return parts.length > 1
    ? parts.join(" · ")
    : `${filter.type.toUpperCase()} · empty`;
}

function getOrderDirectionLabels(attribute?: AttributeSummary) {
  switch (getOrderAttributeType(attribute)) {
    case "DateTime":
      return {
        ascending: { title: "Oldest", detail: "first" },
        descending: { title: "Newest", detail: "first" },
      };
    case "Integer":
    case "BigInt":
    case "Decimal":
    case "Double":
    case "Money":
      return {
        ascending: { title: "Lowest", detail: "first" },
        descending: { title: "Highest", detail: "first" },
      };
    case "Boolean":
      return {
        ascending: { title: "No", detail: "first" },
        descending: { title: "Yes", detail: "first" },
      };
    case "String":
    case "Memo":
      return {
        ascending: { title: "A", detail: "to Z" },
        descending: { title: "Z", detail: "to A" },
      };
    default:
      return {
        ascending: { title: "Asc", detail: "A-Z" },
        descending: { title: "Desc", detail: "Z-A" },
      };
  }
}

function getOrderAttributeType(attribute?: AttributeSummary) {
  if (!attribute) return "Unknown";
  if (attribute.type && attribute.type !== "Unknown") return attribute.type;

  const name = attribute.logicalName.toLowerCase();
  if (name.endsWith("on") || name.includes("date") || name.includes("time")) {
    return "DateTime";
  }
  if (name === "name" || name.endsWith("name")) return "String";
  return "Unknown";
}

function isCompleteLink(link: FetchLinkEntitySelection) {
  return Boolean(link.name && link.from && link.to);
}

function collectIncompleteLinks(
  links: FetchLinkEntitySelection[],
  parentLinkId?: string,
): DraftLink[] {
  return links.flatMap((link) => {
    if (!isCompleteLink(link)) {
      return [
        {
          ...(parentLinkId ? { parentLinkId } : {}),
          link,
        },
      ];
    }
    return collectIncompleteLinks(link.links ?? [], link.id);
  });
}

function mergeDraftLinks(
  links: FetchLinkEntitySelection[],
  drafts: DraftLink[],
) {
  return drafts.reduce(
    (currentLinks, draft) => {
      if (findLinkById(currentLinks, draft.link.id)) return currentLinks;
      if (!draft.parentLinkId) {
        currentLinks.push(draft.link);
        return currentLinks;
      }
      return updateLinkById(currentLinks, draft.parentLinkId, (link) => ({
        ...link,
        links: [...(link.links ?? []), draft.link],
      }));
    },
    [...links],
  );
}

function getNextLinkId(
  links: FetchLinkEntitySelection[],
  parentLinkId?: string,
) {
  if (!parentLinkId) return `link-${links.length + 1}`;
  const parentLink = findLinkById(links, parentLinkId);
  return `${parentLinkId}-${(parentLink?.links ?? []).length + 1}`;
}

function findParentEntityName(
  model: FetchQueryModel,
  linkId: string,
): string | undefined {
  return findParentEntityNameInLinks(model.links, linkId, model.entity);
}

function findParentEntityNameInLinks(
  links: FetchLinkEntitySelection[],
  linkId: string,
  parentEntityName: string,
): string | undefined {
  for (const link of links) {
    if (link.id === linkId) return parentEntityName;
    const nestedParent = findParentEntityNameInLinks(
      link.links ?? [],
      linkId,
      link.name,
    );
    if (nestedParent) return nestedParent;
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
    filters: (link.filters ?? []).map(normalizeFilterGroup),
    orders: link.orders ?? [],
    links: (link.links ?? []).map(normalizeLink),
  };
}

function normalizeFilterGroup(filter: FetchFilterGroup): FetchFilterGroup {
  return {
    ...filter,
    type: filter.type ?? "and",
    conditions: filter.conditions ?? [],
    filters: (filter.filters ?? []).map(normalizeFilterGroup),
  };
}

function normalizeModel(model: FetchQueryModel): FetchQueryModel {
  return {
    ...model,
    distinct: model.distinct ?? false,
    returnTotalRecordCount: model.returnTotalRecordCount ?? false,
    orderByRawValue: model.orderByRawValue ?? false,
    count: model.count ?? "",
    page: model.page ?? "",
    pagingCookie: model.pagingCookie ?? "",
    filterType: model.filterType ?? "and",
    filters: (model.filters ?? []).map(normalizeFilterGroup),
    links: (model.links ?? []).map(normalizeLink),
  };
}

function getFetchNodeMeta(model: FetchQueryModel) {
  return [
    model.top ? `Top ${model.top}` : "All rows",
    model.distinct ? "distinct" : "",
    model.returnTotalRecordCount ? "total count" : "",
    model.count || model.page
      ? `page ${model.page || "1"}${model.count ? ` x ${model.count}` : ""}`
      : "",
  ]
    .filter(Boolean)
    .join(" · ");
}

async function copyToClipboard(value: string) {
  if (navigator.clipboard) {
    try {
      await navigator.clipboard.writeText(value);
      return;
    } catch {
      // Fall back for browser contexts that expose Clipboard API but reject writes.
    }
  }

  const textArea = document.createElement("textarea");
  textArea.value = value;
  textArea.style.position = "fixed";
  textArea.style.opacity = "0";
  document.body.appendChild(textArea);
  textArea.select();
  document.execCommand("copy");
  textArea.remove();
}

function safeReadModel(fetchXml: string): FetchQueryModel {
  try {
    return readFetchQueryModel(fetchXml);
  } catch {
    return {
      entity: "account",
      top: "50",
      distinct: false,
      returnTotalRecordCount: false,
      orderByRawValue: false,
      count: "",
      page: "",
      pagingCookie: "",
      filterType: "and",
      attributes: [{ name: "name" }],
      conditions: [],
      filters: [],
      orders: [],
      links: [],
    };
  }
}
