namespace Utils.Pool
{
    public interface IPoolElement<in TPool>
    {
        void Spawned(TPool pool);
        void SetActive(bool isActive);
        void Despawn();
    }
}