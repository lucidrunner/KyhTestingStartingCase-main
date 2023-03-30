namespace ShopGeneral.Services;

public interface IFileService
{
    void SaveJson(string folder, string fileName, object classToSave);
}