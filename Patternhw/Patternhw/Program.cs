using System;
using System.IO;

namespace Lesson
{
    public interface ILogger
    {
        void WriteError(string message);
    }

    public class FileLogger : ILogger
    {
        public void WriteError(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Нет письма");

            File.AppendAllText("log.txt", message);
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void WriteError(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Нет письма");

            Console.WriteLine(message);
        }
    }

    public class FridayLoggerDecorator : ILogger
    {
        private readonly ILogger _innerLogger;

        public FridayLoggerDecorator(ILogger innerLogger)
        {
            if (innerLogger == null)
                throw new ArgumentNullException(nameof(innerLogger), "Нет логера");

            _innerLogger = innerLogger;
        }

        public void WriteError(string message)
        {

            if (message == null)
                throw new ArgumentNullException(nameof(message), "Нет письма");

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                _innerLogger.WriteError(message);
            }
        }
    }

    public class CompositeLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            if (loggers == null)
                throw new ArgumentNullException(nameof(loggers), "Нет логера");

            _loggers = loggers;
        }

        public void WriteError(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Нет письма");

            foreach (var logger in _loggers)
            {
                if (logger == null)
                    throw new ArgumentNullException(nameof(logger), "Нет логера");

                logger.WriteError(message);
            }

        }
    }

    public class Pathfinder
    {
        private readonly ILogger _logger;

        public Pathfinder(ILogger logger)
        {

            if (logger == null)
                throw new ArgumentNullException(nameof(logger), "Нет логера");

            _logger = logger;
        }

        public void Find()
        {
            _logger.WriteError("zxc");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            var pf1 = new Pathfinder(new FileLogger());

            var pf2 = new Pathfinder(new ConsoleLogger());

            var pf3 = new Pathfinder(new FridayLoggerDecorator(new FileLogger()));

            var pf4 = new Pathfinder(new FridayLoggerDecorator(new ConsoleLogger()));

            var pf5 = new Pathfinder(
                new CompositeLogger(
                    new ConsoleLogger(),
                    new FridayLoggerDecorator(new FileLogger())
                )
            );

            pf1.Find();
            pf2.Find();
            pf3.Find();
            pf4.Find();
            pf5.Find();
        }
    }
}
