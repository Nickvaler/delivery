namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    public class GetCreatedAndAssignedOrdersQueryResponse
    {
        public GetCreatedAndAssignedOrdersQueryResponse(List<Order> orders)
        {
            Orders.AddRange(orders);
        }

        public List<Order> Orders { get; set; } = [];
        public class Order
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
