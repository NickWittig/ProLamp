public interface IDataHandler<T>
{
    public T GetData();
    public void SaveData(T data);

}