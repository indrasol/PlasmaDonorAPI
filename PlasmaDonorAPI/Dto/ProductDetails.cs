namespace NewPlasmaDonorsAPI.Dto
{
    public class ProductDetails
    {
        public long ProductId { get; set; }
        public long Quantity { get; set; }

        public ProductDetails() { }

        public ProductDetails(long productId, long quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
