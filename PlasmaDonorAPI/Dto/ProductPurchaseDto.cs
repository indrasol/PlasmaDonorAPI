namespace NewPlasmaDonorsAPI.Dto
{
    public class ProductPurchaseDto
    {
        public long UserId { get; set; }
        public List<ProductDetails> ProductsDetails { get; set; } = new List<ProductDetails>();

        public ProductPurchaseDto()
        {
        }

        public ProductPurchaseDto(long userId, List<ProductDetails> productsDetails)
        {
            UserId = userId;
            ProductsDetails = productsDetails ?? new List<ProductDetails>();
        }
    }
}
