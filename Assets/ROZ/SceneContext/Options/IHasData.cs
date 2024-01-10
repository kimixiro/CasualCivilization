namespace ROZ.SceneContext.Options
{
    public interface IHasData<in TData>
        where TData : struct
    {
        void SetData(TData data);
    }
}