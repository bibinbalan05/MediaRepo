namespace MediaAPI.Modules.FileStorage
{
    public interface IFileStorage
    {
        void SaveFile(string fileName, byte[] content);
        byte[] GetFile(string fileName);
        IEnumerable<object> GetFiles();
    }

}
