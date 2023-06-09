﻿#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
# endif
namespace AmoyFeels.ProjectInitialization
{
    /// <summary>
    /// Base of Manager
    /// </summary>
    public interface ICoreService
    {
#if !UNITASK_SUPPORT

        /// <summary>
        /// Use for after created an instance of Manager/Service script
        /// </summary>
        public void Initialize();
#else
        /// <summary>
        /// Use for after created an instance of Manager/Service script
        /// </summary>
        public UniTask InitializeAsync();
#endif
    }

}