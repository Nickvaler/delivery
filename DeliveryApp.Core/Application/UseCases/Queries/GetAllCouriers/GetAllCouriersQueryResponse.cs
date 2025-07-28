namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers
{
    public sealed class GetAllCouriersQueryResponse
    {
        public IReadOnlyList<CourierDto> Couriers { get; }

        public GetAllCouriersQueryResponse(IEnumerable<CourierDto> couriers)
        {
            ArgumentNullException.ThrowIfNull(couriers);
            Couriers = couriers.ToList().AsReadOnly();
        }

        public class CourierDto
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
