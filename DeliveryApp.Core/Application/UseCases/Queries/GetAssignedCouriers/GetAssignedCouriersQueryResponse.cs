
namespace DeliveryApp.Core.Application.UseCases.Queries.GetAssignedCouriers
{
    public class GetAssignedCouriersQueryResponse
    {
        public GetAssignedCouriersQueryResponse(List<Courier> couriers)
        {
            Couriers.AddRange(couriers);
        }

        public List<Courier> Couriers { get; set; } = [];

        public class Courier
        {
            /// <summary>
            ///     Идентификатор
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            ///     Имя
            /// </summary>
            public string Name { get; set; }

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
