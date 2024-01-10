using System.Diagnostics.CodeAnalysis;

namespace ROZ.MVP.Options
{
    public interface IHasModel<TModel>
        where TModel : class
    {
        TModel Model { get; }

        void SetModel([NotNull] TModel model);
    }
}