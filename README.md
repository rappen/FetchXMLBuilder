# FetchXML Builder for XrmToolBox

[![Join the chat at https://gitter.im/Cinteros/FetchXMLBuilder](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Cinteros/FetchXMLBuilder?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

_**A tool to create and run advanced FetchXML queries**_

*Written by Jonas Rapp at Cinteros AB, Stockholm, Sweden.*

This tool is a plugin to the [XrmToolBox](http://xrmtoolbox.codeplex.com/) by [Tanguy Touzard](https://www.codeplex.com/site/users/view/tanguy92).

## *> [Download here](http://github.com/cinteros/Cinteros.XrmToolbox.FetchXMLBuilder/releases) <*

### Background

As many other CRM developers, I used to use **Stunnware Tools** extensively to query the CRM database for information and to assist me when creating fetch xml to be used in our solutions.

Unfortunately, this tool was discontinued, and I have found no good substitute. So the time was ripe to do something about it myself!

The easiest way to get a jump start is – of course – to create it as a plugin to the XrmToolBox. Then I wouldn't have to deal with the hassles of connecting to CRM, storing configurations etc. And it would also provide a platform that is recognized throughout the CRM community, which of course would help spreading the tool.

### Project Description

*The goal is to create a tool that will assist you in the following areas:*

Constructing FetchXML in ways that Advanced Find cannot
- aggregates
- outer joins
- "has no"-queries
- attributes from linked entities
- join on other fields than relationships

Querying CRM for information not (easily) found in the CRM UI
- system entities
- "hidden" attributes

Developer assistance
- Generate C# QueryExpression code from fetch xml

The tool reads metadata from CRM to assist with selecting entities, attributes, relations and to perform validation of condition values.

To make it more appealing, there is also the possibility to have it show "Friendly names", which will replace the technical names of entities and attributes with their display names in the users' currently selected language, much like Advanced Find does.

### Information

Some screen shots are available under [Wiki](http://github.com/cinteros/Cinteros.XrmToolbox.FetchXMLBuilder/wiki). More info will be posted :) 

Feedback is much appreciated, post issues and suggestions under  [Issues](http://github.com/cinteros/Cinteros.XrmToolbox.FetchXMLBuilder/issues)!

### Credits

This tool was "inspired by" the [Sitemap](http://download-codeplex.sec.s-msft.com/Download?ProjectName=xrmtoolbox&DownloadId=776491) Editor and a few other tools written by Tanguy and inherits the [`PluginBase`](http://xrmtoolbox.codeplex.com/SourceControl/latest#XrmToolBox/XrmToolBox/PluginBase.cs) class by [Daryl LaBar](http://www.codeplex.com/site/users/view/hulk2484).
