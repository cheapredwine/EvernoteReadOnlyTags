# EvernoteReadOnlyTags

Homebrew utility to set "read-only" attributes (via contentClass) on Evernote notes, via "ReadOnly" (or other user-defined) tag. 

Currently uses dev tokens because the full OAuth path is broken in the Evernote C# SDK.

To remove read-only attributes, run the same console app and indicate which notes to unset.

How to use: 
0. Get an auth token and URL from Evernote and update the app.config. (See app.config for URL to request token.)
1. Mark any Evernote notes you want to protect with a tag of "ReadOnly" (or whatever else you specify in the app.config).
2. Run this app. Use the -i (interactive) switch to get basic output.

To unset read-only:
1. Run this app with the -i switch. Enter "Y" to any read-only notes you wish to unprotect.
