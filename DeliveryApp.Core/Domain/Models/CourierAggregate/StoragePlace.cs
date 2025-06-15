using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate
{
    /// <summary>
    /// Место хранения
    /// </summary>
    public class StoragePlace : Entity<Guid>
    {
        private const string EmptyNameError = "Название места хранения не может быть пустым или пробелом";
        private const string TotalVolumeZeroOrLessError = "Допустимый объём не может быть меньше или равен нулю";
        private const string OrderCannotBeEmptyGuidError = "Месту хранения не может быть присвоен пустой GUID";
        private const string CurrentStoragePlaceHasOrderError = "У места хранения уже назначен заказ";
        private const string CannotPlaceVolumeMoreThanTotalVolumeError = "Нельзя поместить заказ объём которого превышает допустимый объём места хранения";
        private const string CannotExtractNullOrderError = "Нельзя извлечь заказ из места хранения без заказа";

        [ExcludeFromCodeCoverage]
        /// <summary>
        /// Ctr
        /// </summary>
        private StoragePlace() { }

        /// <summary>
        /// Ctr
        /// </summary>
        public StoragePlace(string name, int totalVolume) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            TotalVolume = totalVolume;
        }

        public string Name { get; init; }
        public int TotalVolume { get; init; }
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <returns></returns>
        public static Result<StoragePlace, Error> Create(string name, int totalVolume)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GeneralErrors.ValueIsInvalid(EmptyNameError);
            }

            if (totalVolume <= 0)
            {
                return GeneralErrors.ValueIsInvalid(TotalVolumeZeroOrLessError);
            }

            return new StoragePlace(name, totalVolume);
        }

        public string IsEmpty()
        {
            return OrderId == null ? "да" : "нет";
        }

        public Result<object, Error> SetOrder(Guid orderId, int volume)
        {
            if (orderId.Equals(Guid.Empty))
            {
                return GeneralErrors.ValueIsInvalid(OrderCannotBeEmptyGuidError);
            }

            if (OrderId != null)
            {
                return GeneralErrors.ValueIsInvalid(CurrentStoragePlaceHasOrderError);
            }

            if (TotalVolume < volume)
            {
                return GeneralErrors.ValueIsInvalid(CannotPlaceVolumeMoreThanTotalVolumeError);
            }

            OrderId = orderId;
            return new object();
        }

        public Result<Guid, Error> Extract()
        {
            if (OrderId == null)
            {
                return GeneralErrors.ValueIsInvalid(CannotExtractNullOrderError);
            }

            var orderId = OrderId.Value;
            OrderId = null;
            return orderId;
        }
    }
}
