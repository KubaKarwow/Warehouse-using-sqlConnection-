using APBD6.Models.DTOs;

namespace APBD6.Repositories;

public interface IWarehouseRepository
{
    public Task<int> ServeRequest(ClientRequest clientRequest);
}