namespace Utils.Pool
{
    public interface IPool<T>
    {
        T Spawn();
        void Despawn(T element);
    }
}