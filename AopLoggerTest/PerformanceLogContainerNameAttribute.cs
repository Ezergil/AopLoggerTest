using System;

namespace AopLoggerTest
{
    public class PerformanceLogContainerNameAttribute : Attribute
    {
        public PerformanceLogContainerNameAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}