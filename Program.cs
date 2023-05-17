using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Xml;
using System.Xml.Linq;

class Program
{
    static void Main(string[] args)
    {
        string logType = "Microsoft-Windows-PrintService/Operational";

        EventLogQuery eventsQuery = new EventLogQuery(logType, PathType.LogName, "*[System/EventID=805]");

        EventLogWatcher logWatcher = new EventLogWatcher(eventsQuery);

        logWatcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(LogEvent);

        logWatcher.Enabled = true;

        Console.WriteLine("Press any key to exit...");
        Console.ReadLine();

        logWatcher.Dispose();
    }

    static void LogEvent(object obj, EventRecordWrittenEventArgs arg)
    {
        if (arg.EventRecord != null)
        {
            Console.WriteLine("Event {0} was published.", arg.EventRecord.Id);

            string xml = arg.EventRecord.ToXml();

            XDocument doc = XDocument.Parse(xml);
            XNamespace ns = "http://manifests.microsoft.com/win/2005/08/windows/printing/spooler/core/events";

            var renderJobDiagNodes = doc.Descendants(ns + "RenderJobDiag");

            foreach (var node in renderJobDiagNodes)
            {
                Console.WriteLine("Job ID: " + node.Element(ns + "JobId").Value);
                Console.WriteLine("Copies: " + node.Element(ns + "Copies").Value);
            }
            // Your code here to handle the event and extract information
        }
        else
        {
            Console.WriteLine("The event instance was null.");
        }
    }
}