// Trong thư mục Services/ISaleService.cs

using System.Threading.Tasks;

namespace GoCoffeeTea.Services
{
    public interface ISaleService
    {
        /// <summary>
        /// Trừ nguyên vật liệu từ tồn kho dựa trên công thức của sản phẩm.
        /// </summary>
        /// <param name="productId">ID của sản phẩm đã bán.</param>
        /// <param name="quantity">Số lượng sản phẩm đã bán.</param>
        /// <returns>True nếu trừ tồn kho thành công.</returns>
        Task<bool> DeductMaterialsAsync(int productId, int quantity);

        /// <summary>
        /// Cập nhật tổng chi tiêu (TotalSpent) và cấp độ thân thiết (LoyaltyTier) của khách hàng.
        /// </summary>
        /// <param name="customerId">ID của khách hàng.</param>
        /// <param name="amountSpent">Số tiền khách đã chi tiêu trong đơn hàng này.</param>
        Task UpdateCustomerLoyaltyAsync(int customerId, decimal amountSpent);
    }
}