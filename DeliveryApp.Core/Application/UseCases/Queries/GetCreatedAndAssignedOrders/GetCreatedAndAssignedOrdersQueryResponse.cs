namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    public class GetCreatedAndAssignedOrdersQueryResponse
    {
        public IReadOnlyList<OrderDto> Orders { get; }

        public GetCreatedAndAssignedOrdersQueryResponse(IEnumerable<OrderDto> orders)
        {
            ArgumentNullException.ThrowIfNull(orders);
            Orders = orders.ToList().AsReadOnly();
        }

        public class OrderDto
        {
            /// <summary>
            ///     Идентификатор
            /// </summary>
            public Guid Id { get; set; }

            /// <summary> 
            ///     Идентификатор курьера
            /// </summary>
            public Guid CourierId { get; set; }

            /// <summary>
            ///     Горизонталь
            /// </summary>
            public int LocationX { get; set; }

            /// <summary>
            ///     Вертикаль
            /// </summary>
            public int LocationY { get; set; }
        }
    }
}
