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
        private const string VolumeZeroOrLessError = "Допустимый объём не может быть меньше или равен нулю";
        private const string OrderCannotBeEmptyGuidError = "Месту хранения не может быть присвоен пустой GUID";
        private const string CurrentStoragePlaceHasOrderError = "У места хранения уже назначен заказ";
        private const string CannotPlaceVolumeMoreThanTotalVolumeError = "Нельзя поместить заказ объём которого превышает допустимый объём места хранения";
        private const string CannotExtractNullOrderError = "Нельзя извлечь заказ из места хранения без заказа";

        private const string DefaultStorageName = "Сумка";
        private const int DefaultStorageVolume = 10;

        /// <summary>
        /// Место хранение по умолчаню
        /// </summary>
        public static StoragePlace DefaultStoragePlace => new(DefaultStorageName, DefaultStorageVolume);

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

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Объём
        /// </summary>
        public int TotalVolume { get; init; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid? OrderId { get; private set; }

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
                return GeneralErrors.ValueIsInvalid(VolumeZeroOrLessError);
            }

            return new StoragePlace(name, totalVolume);
        }

        /// <summary>
        /// Проверить пустой ли заказ
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return OrderId == null;
        }

        /// <summary>
        /// Поместить заказ в место хранения
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public UnitResult<Error> SetOrder(Guid orderId, int volume)
        {
            if (orderId.Equals(Guid.Empty))
            {
                return GeneralErrors.ValueIsInvalid(OrderCannotBeEmptyGuidError);
            }

            if (!IsEmpty())
            {
                return GeneralErrors.ValueIsInvalid(CurrentStoragePlaceHasOrderError);
            }

            if (volume <= 0)
            {
                return GeneralErrors.ValueIsInvalid(VolumeZeroOrLessError);
            }

            if (TotalVolume < volume)
            {
                return GeneralErrors.ValueIsInvalid(CannotPlaceVolumeMoreThanTotalVolumeError);
            }

            OrderId = orderId;
            return new UnitResult<Error>();
        }

        /// <summary>
        /// Освободить место хранения
        /// </summary>
        /// <returns></returns>
        public Result<Guid, Error> Extract()
        {
            if (IsEmpty())
            {
                return GeneralErrors.ValueIsInvalid(CannotExtractNullOrderError);
            }

            var orderId = OrderId.Value;
            OrderId = null;
            return orderId;
        }
    }
}
