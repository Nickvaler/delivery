using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate
{
    public class Order : Aggregate<Guid>
    {
        private const string CompleteOnlyAssignedError = "Завершить можно только назначенный ранее заказ";
        private const string EmptyValueError = "Значение {0} не может быть пустым";

        /// <summary>
        ///     Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Order() { }

        /// <summary>
        ///     Ctr
        /// </summary>
        private Order(Guid orderId, Location location, int volume) : this()
        {
            Id = orderId;
            Location = location;
            Volume = volume;
            Status = OrderStatus.Created;
        }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Объём заказа
        /// </summary>
        public int Volume { get; private set; }

        public OrderStatus Status { get; private set; }

        public Guid? CourierId { get; private set; }

        /// <summary>
        /// Фабричный метод
        /// </summary>
        /// <returns></returns>
        public static Result<Order, Error> Create(Guid orderId, Location location, int volume)
        {
            if (orderId == Guid.Empty)
            {
                return GeneralErrors.ValueIsInvalid(string.Concat(EmptyValueError, nameof(orderId)));
            }

            return new Order(orderId, location, volume);
        }

        public UnitResult<Error> Assign(Courier courier)
        {
            CourierId = courier.Id;
            Status = OrderStatus.Assigned;
            return new UnitResult<Error>();
        }

        public UnitResult<Error> Complete()
        {
            if (Status != OrderStatus.Assigned)
            {
                return GeneralErrors.ValueIsInvalid(CompleteOnlyAssignedError);
            }

            Status = OrderStatus.Completed;
            return new UnitResult<Error>();
        }
    }
}
