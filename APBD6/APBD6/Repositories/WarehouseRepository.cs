using APBD6.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace APBD6.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    
    public async Task<int> ServeRequest(ClientRequest clientRequest)
    {
        await using SqlConnection sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));

        await sqlConnection.OpenAsync();

        var transaction = await sqlConnection.BeginTransactionAsync();

        await using SqlCommand command = sqlConnection.CreateCommand();

        command.Connection = sqlConnection;
        command.Transaction = transaction as SqlTransaction;

        command.CommandText = "SELECT IDPRODUCT,PRICE FROM PRODUCT WHERE IDPRODUCT=@IDPRODUCT";
        command.Parameters.AddWithValue("@IDPRODUCT", clientRequest.IdProduct);
        try
        {
            int productId = 0;
            Decimal price = 0;
            using( var productReader = await command.ExecuteReaderAsync())
            {
                while (await productReader.ReadAsync())
                {
                    productId = (int)productReader["IdProduct"];
                    price = (Decimal)productReader["Price"];
                }
                if (productId == 0)
                {
                    return -1;
                } 
            }
            
            command.Parameters.Clear();
            command.CommandText = "SELECT COUNT(1) AS COUNT FROM WAREHOUSE WHERE IDWAREHOUSE=@WAREHOUSEID";
            command.Parameters.AddWithValue("@WAREHOUSEID", clientRequest.IdWarehouse);
            var warehousesCountToConvert = await command.ExecuteScalarAsync();
            
               if (warehousesCountToConvert == null)
               {
                   return -2;
               }
               int warehousesCount = Convert.ToInt32(warehousesCountToConvert); 
            
            
            
           
            command.Parameters.Clear();
            command.CommandText =
                "SELECT IDORDER FROM \"ORDER\" WHERE IDPRODUCT=@PRODUCTID AND AMOUNT=@AMOUNT AND CREATEDAT<@CREATEDAT";
            

            command.Parameters.AddWithValue("@AMOUNT", clientRequest.Amount);
            command.Parameters.AddWithValue("@PRODUCTID", clientRequest.IdProduct);
            command.Parameters.AddWithValue("@CREATEDAT", clientRequest.CreatedAt);
            
            var orderToConvert=await command.ExecuteScalarAsync();

            if (orderToConvert == null)
            {
                return -3;
            }
            var orderID = Convert.ToInt32(orderToConvert);
            
            command.Parameters.Clear();
            command.CommandText = "SELECT COUNT(1) FROM PRODUCT_WAREHOUSE WHERE IDORDER=@ORDERID";
            command.Parameters.AddWithValue("@ORDERID", orderID);

            var countToConvert = await command.ExecuteScalarAsync();
            
            if (countToConvert == null)
            {
                return -4;
            }

            var countProduct_Warehouse = Convert.ToInt32(countToConvert);
            if (countProduct_Warehouse != 0)
            {
                return -4;
            }
            
            command.Parameters.Clear();
            command.CommandText = "UPDATE \"ORDER\" SET FULFILLEDAT=@FULLFILLEDAT WHERE IDORDER=@IDORDER";
            command.Parameters.AddWithValue("@FULLFILLEDAT", DateTime.Now);
            command.Parameters.AddWithValue("@IDORDER", orderID);
            await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();
            command.CommandText =
                "INSERT INTO PRODUCT_WAREHOUSE" +
                " VALUES(@WAREHOUSEID,@PRODUCTID,@IDORDER,@AMOUNT,@PRICE,@CREATEDAT)  SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@WAREHOUSEID", clientRequest.IdWarehouse);
            command.Parameters.AddWithValue("@PRODUCTID", clientRequest.IdProduct);
            command.Parameters.AddWithValue("@AMOUNT", clientRequest.Amount);
            command.Parameters.AddWithValue("@CREATEDAT", clientRequest.CreatedAt);
            command.Parameters.AddWithValue("@IDORDER", orderID);
            command.Parameters.AddWithValue("@PRICE", price * clientRequest.Amount);
            var newID = await command.ExecuteScalarAsync();
            int newIdValue = 0;
            if (newID != null)
            {
                newIdValue = Convert.ToInt32(newID);
            }

            await transaction.CommitAsync();
            return newIdValue;
        }
        catch (SqlException exc)
        {
            Console.WriteLine(exc);
            await transaction.RollbackAsync();
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc);
            await transaction.RollbackAsync();
        }

        return -5;
    }
}