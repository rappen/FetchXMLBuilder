# FetchXML Builder for XrmToolbox

_Written by Jonas Rapp at Cinteros AB, Stockholm, Sweden._

## A tool to create and run advanced FetchXML queries

This tool is a plugin to the XrmToolbox by Tanguy Touzard.

### Background

As many other CRM developers, I used to use Stunnware Tools extensively to query the CRM database for information and to assist me when creating fetch xml to be used in our solutions.

Unfortunately, this tool was discontinued, and I have found no good substitute. So the time was ripe to do something about it myself!

The easiest way to get a jump start is – of course – to create it as a plugin to the XrmToolbox. Then I wouldn't have to deal with the hassles of connecting to CRM, storing configurations etc. And it would also provide a platform that is recognized throughout the CRM community, which of course would help spreading the tool.

### Project Description

The goal is to create a tool that will assist you in two primary areas:

Constructing FetchXML in ways that Advanced Find cannot
- aggregates
- outer joins
- "has no"-queries
- attributes from linked entities
- join on other fields than relationships
Querying CRM for information not (easily) found in the CRM UI
- system entities
- "hidden" attributes
The tool reads metadata from CRM to assist with selecting entities, attributes, relations and to perform validation of condition values.

To make it more appealing, there is also the possibility to have it show "Friendly names", which will replace the technical names of entities and attributes with their display names in the users' currently selected language, much like Advanced Find does.

### Information

Some screen shots are available under Documentation. More info will be posted :) 
Feedback is much appreciated, start a topic under Discussions!
It's a work in progress… future fixes and features are available under Issues.
Feel free to add or vote up issues you would like to see!

### Credits

This tool was "inspired by" the Sitemap Editor and a few other tools written by Tanguy and inherits the PluginBase class by Daryl LaBar.


jjjjj