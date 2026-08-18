using Shipping.Api.Requests_Responses;

namespace Shipping.Api.Services
{
    public class TestHedjing
    {
        public async Task<List<ProductResponse>> GetProductsAsync()
        {
            return [
                       new ProductResponse{Id = 1,
                            Name = "Product 1 From Alternative Source",
                            Price = 20.00m,
                            Stock = 33
                        },
                        new ProductResponse{
                            Id = 2,
                            Name = "Product 2 From Alternative Source",
                            Price = 30.50m,
                            Stock = 22
                        },
                        new ProductResponse{
                            Id = 3,
                            Name = "Product 3 From Alternative Source",
                            Price = 40.00m,
                            Stock = 15
                        }
                    ];
        }
    }
}
