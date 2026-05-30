import { describe, expect, it } from "vitest";
import {
  formatFetchXml,
  readFetchQueryModel,
  toCSharpFetchXml,
  toJavaScriptFetchXml,
  toODataUrl,
  toPowerAutomateParameters,
  validateFetchXml,
  writeFetchQueryModel,
} from "../src";

const accountQuery = `<fetch>
  <entity name="account">
    <attribute name="name" />
    <attribute name="accountid" />
    <filter type="and">
      <condition attribute="name" operator="like" value="%Contoso%" />
      <condition attribute="statecode" operator="eq" value="0" />
      <condition attribute="createdon" operator="on-or-after" value="2024-01-01" />
    </filter>
  </entity>
</fetch>`;

describe("toPowerAutomateParameters", () => {
  it("replaces condition value attributes with Power Automate parameter tokens", () => {
    expect(toPowerAutomateParameters(accountQuery)).toMatchSnapshot();
  });

  it("supports nested condition value elements and duplicate attributes", () => {
    const query = `<fetch><entity name="contact"><filter><condition attribute="emailaddress1" operator="in"><value>a@example.com</value><value>b@example.com</value></condition></filter></entity></fetch>`;

    expect(toPowerAutomateParameters(query)).toMatchSnapshot();
  });

  it("accepts custom parameter names", () => {
    const result = toPowerAutomateParameters(accountQuery, {
      parameterNames: {
        name: "accountName",
        statecode: "state",
      },
    });

    expect(result.parameters.map((parameter) => parameter.name)).toEqual([
      "accountName",
      "state",
      "createdon",
    ]);
  });
});

describe("core offline workbench functions", () => {
  it("formats and validates FetchXML", () => {
    expect(
      formatFetchXml(
        `<fetch><entity name="account"><attribute name="name"/></entity></fetch>`,
      ),
    ).toMatchSnapshot();
    expect(validateFetchXml(accountQuery)).toEqual([]);
  });

  it("generates first-release converter outputs", () => {
    expect(toODataUrl(accountQuery)).toMatchInlineSnapshot(
      `"/account?$select=name,accountid&$filter=contains(name,%20'Contoso')%20and%20statecode%20eq%200"`,
    );
    expect(toJavaScriptFetchXml(accountQuery)).toContain("const fetchXml");
    expect(toCSharpFetchXml(accountQuery)).toContain("var fetchXml");
  });

  it("round-trips the visual builder query model", () => {
    const model = readFetchQueryModel(accountQuery);
    expect(model).toMatchObject({
      entity: "account",
      top: "",
      attributes: [{ name: "name" }, { name: "accountid" }],
      conditions: [
        { attribute: "name", operator: "like", value: "%Contoso%" },
        { attribute: "statecode", operator: "eq", value: "0" },
        {
          attribute: "createdon",
          operator: "on-or-after",
          value: "2024-01-01",
        },
      ],
    });

    expect(writeFetchQueryModel(model)).toContain('<entity name="account">');
  });

  it("round-trips fetch root properties in the visual builder query model", () => {
    const model =
      readFetchQueryModel(`<fetch top="25" distinct="true" returntotalrecordcount="true" useraworderby="true" count="10" page="2" paging-cookie="cookie-value">
  <entity name="account" />
</fetch>`);

    expect(model).toMatchObject({
      top: "25",
      distinct: true,
      returnTotalRecordCount: true,
      orderByRawValue: true,
      count: "10",
      page: "2",
      pagingCookie: "cookie-value",
    });

    const xml = writeFetchQueryModel(model);
    expect(xml).toContain('returntotalrecordcount="true"');
    expect(xml).toContain('useraworderby="true"');
    expect(xml).toContain('count="10"');
    expect(xml).toContain('page="2"');
    expect(xml).toContain('paging-cookie="cookie-value"');
  });

  it("preserves nested linked entities in the visual builder query model", () => {
    const model = readFetchQueryModel(`<fetch>
  <entity name="account">
    <link-entity name="contact" from="parentcustomerid" to="accountid" link-type="outer" alias="primarycontact">
      <link-entity name="aaduser" from="systemuserid" to="ownerid" link-type="inner" alias="owner" />
    </link-entity>
  </entity>
</fetch>`);

    expect(model.links[0]?.links[0]).toMatchObject({
      name: "aaduser",
      from: "systemuserid",
      to: "ownerid",
      alias: "owner",
    });
    expect(writeFetchQueryModel(model)).toContain(
      '<link-entity name="aaduser" from="systemuserid" to="ownerid"',
    );
  });

  it("round-trips nested filter groups in the visual builder query model", () => {
    const model = readFetchQueryModel(`<fetch>
  <entity name="account">
    <filter type="and">
      <condition attribute="name" operator="like" value="%Contoso%" />
      <filter type="or">
        <condition attribute="statecode" operator="eq" value="0" />
      </filter>
    </filter>
  </entity>
</fetch>`);

    expect(model.conditions).toHaveLength(1);
    expect(model.filters[0]).toMatchObject({
      type: "or",
      conditions: [{ attribute: "statecode", operator: "eq", value: "0" }],
    });

    const xml = writeFetchQueryModel(model);
    expect(xml).toContain('<filter type="or">');
    expect(xml).toContain(
      '<condition attribute="statecode" operator="eq" value="0" />',
    );
  });
});
