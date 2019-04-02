using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LoggingSample_BLL.LogTargets
{
    [Target("XMLTarget")]
    public sealed class XMLTarget : AsyncTaskTarget
    {
        private readonly object _sync = new object();

        public XMLTarget()
        {
            this.Host = Environment.MachineName;
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            var path = LogManager.Configuration.Variables["filePath"].Render(logEvent) + ".xml";

            lock (_sync)
            {
                if (!File.Exists(path))
                {
                    XmlWriter writer = XmlWriter.Create(path);
                    writer.WriteStartDocument();
                    writer.WriteStartElement("logs");
                    writer.Dispose();
                }

                var doc = XDocument.Load(path);

                var log = new XElement("log",

                    new XAttribute("MachineName", this.Host),

                    new XAttribute("Exception", logEvent.Exception?.ToString() ?? "null"),

                    new XAttribute("LoggerName", logEvent.LoggerName),

                    new XAttribute("Level", logEvent.Level.ToString()),

                    new XAttribute("Message", logEvent.Message),

                    new XAttribute("MessageSource", logEvent.CallerFilePath),

                    new XAttribute("TimeStamp", logEvent.TimeStamp));

                doc.Root?.Add(log);

                doc.Save(path);
            }
        }
    }
}
