# visuallogparser
A copy of the code at https://archive.codeplex.com/?p=visuallogparser so I can keep it around

From the original site:

Visual LogParser GUI is a free Visual tool for the Microsoft LogParser engine. It enable user to query any log files or data source with an advanced SQL syntax.

# SQL queries against a variety of log files and other system data sources
As an application developper you often need to write some logs for your application, and hopefully there is actually many framework to ease your pain: Log4net, Entlib Logging Application Block, etc. But when it come to read those logs, extract statistics or to do some alert or action on them, things become harder. Actually a free command line tool from Microsoft name LogParser and it is quite awesome: you can query any log and data sources (database, event log, iis logs, file system, registry, etc.) with a complexe SQL query (I mean with functions, group by, etc.). More here: LogParser

On the down side, using it from the command line become quickly boring as you need to type your sql query in a DOS prompt. As we failed to find any intuitive GUI for logParser, we just start to build our own, by assembling the following pieces: the logparser engine, a "visual studio" like MDI dockable interface, syntax highlighting...
## The application is a composition of the following bricks:
- Framework .Net 2.0 http://msdn2.microsoft.com/en-us/netframework/default.aspx
- LogParser (Logs Parsing) http://www.logparser.com
- ICSharpDevelop.TextEditor (User Interface Query) http://www.sharpdevelop.com/OpenSource/SD/Default.aspx
- Composite Application block http://www.gotdotnet.com/codegallery/codegallery.aspx?id=22f72167-af95-44ce-a6ca-f2eafbf2653c
- SandDock (Freeware Version) http://www.divil.co.uk/net/controls/sanddock/

Discussion Forum : http://groups-beta.google.com/group/visual-log-parser

<img src="https://www.carehart.org/images/visual_logparser_screenshot_at_github.jpg"/>
