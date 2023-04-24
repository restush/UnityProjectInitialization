using System;

namespace AmoyFeels.ProjectInitialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class InitializeAtRuntimeAttribute : Attribute
    {
        public int InitializationPriority { get; }
        public InitializeAtRuntimeAttribute(int initializationPriority = 0)
        {
            InitializationPriority = initializationPriority;
        }
    } 
}
