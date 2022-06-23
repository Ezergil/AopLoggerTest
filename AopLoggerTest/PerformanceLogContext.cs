using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace AopLoggerTest
{
	public static class PerformanceLogContextFactory
	{
		private static readonly Dictionary<string, PerformanceLogContext> _contexts =
			new();

		public static PerformanceLogContext GetPerformanceLogContext(string name)
		{
			if (!_contexts.ContainsKey(name))
				_contexts[name] = new PerformanceLogContext(name);
			return _contexts[name];
		}
	}
	
    public class PerformanceLogContext
	{
		private readonly string _name;
		private readonly Stopwatch _sw;
		private List<PerformanceData> _performanceInfos;
		private int _counter;

		public PerformanceLogContext(string name = null)
		{
			_name = name;
			_performanceInfos = new List<PerformanceData>();
			_sw = Stopwatch.StartNew();
		}

		public PerformanceLoggerRecorder CreateRecorder(string name)
		{
			var number = Interlocked.Increment(ref _counter);
			return new PerformanceLoggerRecorder(name, this, number);
		}

		public void AddRecord(string key, long elapsedMilliseconds, int? number = null)
		{
			var count = number ?? Interlocked.Increment(ref _counter);
			_performanceInfos.Add(new PerformanceData(key, elapsedMilliseconds, count));
		}

		public void Flush()
		{
			_sw.Stop();
			_performanceInfos.Add(new PerformanceData($"{_name}", _sw.ElapsedMilliseconds, 0));
			_performanceInfos = _performanceInfos.OrderBy(x => x.Number).ToList();
			writeToLog();
		}
		
		private void writeToLog()
		{
			var logObj = new
			{
				Name = _name,
				TotalEllapsedMs = _sw.ElapsedMilliseconds,
				Details = _performanceInfos
			};
			var serializedObject = JsonConvert.SerializeObject(logObj);
			Console.WriteLine(serializedObject);
		}

		private class PerformanceData
		{
			public PerformanceData(string name, long elapsedMilliseconds, int number)
			{
				Name = name;
				ElapsedMilliseconds = elapsedMilliseconds;
				Number = number;
			}

			/// <summary>
			/// Назнавание метода или дейсвия
			/// </summary>
			public string Name { get; }
			
			/// <summary>
			/// Время выполнения в миллисекундах
			/// </summary>
			public long ElapsedMilliseconds { get; }
			
			/// <summary>
			/// Номер записи. Служит для вывода логов в нужном порядке
			/// </summary>
			[JsonIgnore]
			public int Number { get; }
		}
	}

    /// <summary>
	/// Класс для записи информации о времени выполнения кода в промежутках между созданием класса и вызовом метода Dispose.
	/// </summary>
	public class PerformanceLoggerRecorder
	{
		private readonly PerformanceLogContext _ctx;
		private readonly int _number;
		private Stopwatch _sw;
		private readonly string _name;
		public PerformanceLoggerRecorder(string name, PerformanceLogContext context, int number)
		{
			_ctx = context;
			_number = number;
			_sw = Stopwatch.StartNew();
			_name = name;
		}
		public void Exit()
		{
			_sw.Stop();
			_ctx.AddRecord(_name, _sw.ElapsedMilliseconds, _number);
			_sw = null;
		}
	}
}