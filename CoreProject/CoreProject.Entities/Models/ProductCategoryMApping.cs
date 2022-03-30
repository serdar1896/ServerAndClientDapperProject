using CoreProject.Entities.Infrastructure;

namespace CoreProject.Entities.Models
{
   public class ProductCategoryMapping : BaseEntity
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public Products Products { get; set; }
        public Categories Categories { get; set; }

    }
}
