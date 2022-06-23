using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cauldron.Interception;

namespace AopLoggerTest
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class PerformanceLogRecordAttribute : Attribute, IMethodInterceptor
    {
        private PerformanceLoggerRecorder _logRecorder;

        public void OnEnter(Type declaringType, object instance, MethodBase methodbase, object[] values)
        {
            StackTrace stackTrace = new StackTrace();
            var topLevelMethod = stackTrace.GetFrames().Last().GetMethod();
            var containerName = $"{topLevelMethod.DeclaringType.Name}.{topLevelMethod.Name}";
            _logRecorder = PerformanceLogContextFactory.GetPerformanceLogContext(containerName).CreateRecorder(methodbase.Name);
        }

        public bool OnException(Exception e)
        {
            return false;
        }

        public void OnExit()
        {
            _logRecorder?.Exit();
        }
    }
}