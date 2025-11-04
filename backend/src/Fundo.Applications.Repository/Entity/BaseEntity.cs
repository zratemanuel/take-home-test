namespace Fundo.Applications.Repository.Entity;

public static class BaseEntity
{
    public static string GenerateId()
    {
        return Guid.NewGuid().ToString();
    }
}
