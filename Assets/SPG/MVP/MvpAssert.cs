using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using SPG.MVP.Options;
using UnityEngine.Assertions;

namespace SPG.MVP
{
    public static class MvpAssert
    {
        [Conditional("UNITY_ASSERTIONS")]
        public static void IsModelSet<T>([NotNull] this IHasModel<T> presenter)
            where T : class
        {
            Assert.IsNotNull(presenter);
            Assert.IsNotNull(presenter.Model);
        }
    }
}