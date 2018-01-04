-------------------------------------------
Embedded Release Notes for FetchXML Builder
Jonas Rapp, 2018-01-04
-------------------------------------------

Welcome form will be displayed when FXB is started and a new version is detected.
This is detected by comparing assembly version with version stored in settings file.

Release Notes can be stored in this ReleaseNotes folder, by creating a Word document
and saving it as rtf.
The file shall be named [version].rtf
The file must be marked with property Build Action = Embedded Resource to be available
at run time.

When loading release notes, the resource (file) with highest version number equal or
lower than current assembly version will be loaded and displayed in the release notes
richtextcontrol on the form.

This design makes it possible to implement features to read older release notes in the
future.