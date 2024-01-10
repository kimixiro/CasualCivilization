namespace ROZ.MVP.Options
{
    public interface IHasData<in TData>
        where TData : struct
    {
        void SetData(TData data);
    }
}