using OrderDelivery.Domain;
using OrderDelivery.Domain.Entities;
using OrderDelivery.Domain.Enums;
using OrderDelivery.Domain.Interfaces;

namespace OrderDelivery.Application.Services
{
    /// <summary>
    /// Service for managing orders using the repository pattern and unit of work
    /// </summary>
    public class OrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Order> _orderRepository;
        private readonly IGenericRepository<Driver> _driverRepository;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderRepository = unitOfWork.Repository<Order>();
            _driverRepository = unitOfWork.Repository<Driver>();
        }

        /// <summary>
        /// Creates a new order with transaction management
        /// </summary>
        /// <param name="order">The order to create</param>
        /// <returns>The created order</returns>
        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Validate order data
                if (order.OrderValue <= 0)
                    throw new ArgumentException("Order value must be greater than zero.");

                if (string.IsNullOrWhiteSpace(order.CustomerName))
                    throw new ArgumentException("Customer name is required.");

                // Set initial status
                order.Status = OrderStatus.Pending;
                order.PlacedAt = DateTime.UtcNow;

                // Add the order
                var createdOrder = await _orderRepository.AddAsync(order);
                
                return createdOrder;
            });
        }

        /// <summary>
        /// Assigns a driver to an order with transaction management
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="driverId">The driver ID</param>
        /// <returns>The updated order</returns>
        public async Task<Order> AssignDriverToOrderAsync(Guid orderId, Guid driverId)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Get the order with includes
                var order = await _orderRepository.GetByIdWithIncludesAsync(orderId, o => o.Driver, o => o.Merchant);
                if (order == null)
                    throw new ArgumentException($"Order with ID {orderId} not found.");

                // Check if order is in a valid state for assignment
                if (order.Status != OrderStatus.Pending)
                    throw new InvalidOperationException($"Order is in {order.Status} status and cannot be assigned.");

                // Get the driver
                var driver = await _driverRepository.GetByIdAsync(driverId);
                if (driver == null)
                    throw new ArgumentException($"Driver with ID {driverId} not found.");

                // Assign driver to order
                order.DriverId = driverId;
                order.Status = OrderStatus.Assigned;
                order.AssignedAt = DateTime.UtcNow;

                // Update the order
                _orderRepository.Update(order);

                return order;
            });
        }

        /// <summary>
        /// Gets orders with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="merchantId">Optional merchant filter</param>
        /// <returns>Paginated orders with total count</returns>
        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersAsync(
            int pageNumber = 1, 
            int pageSize = 10, 
            OrderStatus? status = null,
            Guid? merchantId = null)
        {
            // Build predicate
            Expression<Func<Order, bool>>? predicate = null;
            
            if (status.HasValue || merchantId.HasValue)
            {
                predicate = order => 
                    (!status.HasValue || order.Status == status.Value) &&
                    (!merchantId.HasValue || order.MerchantId == merchantId.Value);
            }

            // Get paginated results with includes
            var result = await _orderRepository.GetPagedAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                predicate: predicate,
                orderBy: o => o.PlacedAt,
                ascending: false,
                o => o.Merchant,
                o => o.Driver);

            return (result.Items, result.TotalCount);
        }

        /// <summary>
        /// Updates order status with validation
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="newStatus">The new status</param>
        /// <returns>The updated order</returns>
        public async Task<Order> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    throw new ArgumentException($"Order with ID {orderId} not found.");

                // Validate status transition
                if (!IsValidStatusTransition(order.Status, newStatus))
                    throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}.");

                // Update status and timestamp
                order.Status = newStatus;
                UpdateOrderTimestamp(order, newStatus);

                _orderRepository.Update(order);

                return order;
            });
        }

        /// <summary>
        /// Gets order statistics
        /// </summary>
        /// <param name="merchantId">Optional merchant filter</param>
        /// <param name="startDate">Start date for filtering</param>
        /// <param name="endDate">End date for filtering</param>
        /// <returns>Order statistics</returns>
        public async Task<OrderStatistics> GetOrderStatisticsAsync(
            Guid? merchantId = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Build predicate for date and merchant filtering
            Expression<Func<Order, bool>>? predicate = null;
            
            if (merchantId.HasValue || startDate.HasValue || endDate.HasValue)
            {
                predicate = order =>
                    (!merchantId.HasValue || order.MerchantId == merchantId.Value) &&
                    (!startDate.HasValue || order.PlacedAt >= startDate.Value) &&
                    (!endDate.HasValue || order.PlacedAt <= endDate.Value);
            }

            // Get counts for different statuses
            var totalOrders = await _orderRepository.CountAsync(predicate ?? (o => true));
            var pendingOrders = await _orderRepository.CountAsync(predicate ?? (o => true) & (o => o.Status == OrderStatus.Pending));
            var completedOrders = await _orderRepository.CountAsync(predicate ?? (o => true) & (o => o.Status == OrderStatus.Delivered));
            var cancelledOrders = await _orderRepository.CountAsync(predicate ?? (o => true) & (o => o.Status == OrderStatus.Cancelled));

            return new OrderStatistics
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                CompletedOrders = completedOrders,
                CancelledOrders = cancelledOrders
            };
        }

        #region Private Helper Methods

        /// <summary>
        /// Validates if a status transition is allowed
        /// </summary>
        private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return newStatus switch
            {
                OrderStatus.Assigned => currentStatus == OrderStatus.Pending,
                OrderStatus.PickedUp => currentStatus == OrderStatus.Assigned,
                OrderStatus.InTransit => currentStatus == OrderStatus.PickedUp,
                OrderStatus.Delivered => currentStatus == OrderStatus.InTransit,
                OrderStatus.Cancelled => currentStatus is OrderStatus.Pending or OrderStatus.Assigned,
                _ => false
            };
        }

        /// <summary>
        /// Updates the appropriate timestamp based on the new status
        /// </summary>
        private static void UpdateOrderTimestamp(Order order, OrderStatus newStatus)
        {
            switch (newStatus)
            {
                case OrderStatus.PickedUp:
                    order.PickedUpAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CancelledAt = DateTime.UtcNow;
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Statistics for orders
    /// </summary>
    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
    }
} 