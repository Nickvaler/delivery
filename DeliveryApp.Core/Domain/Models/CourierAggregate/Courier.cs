using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Models.CourierAggregate
{
    public class Courier : Aggregate<Guid>
    {
        private const string ValueZeroOrLessError = "Значение {0} не может быть меньше или равен нулю";
        private const string NotFoundOrderError = "Заказ: {0} не найден у курьера";
        private const string EmptyValueError = "Значение {0} не может быть пустым";

        /// <summary>
        ///     Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private Courier() { }

        /// <summary>
        ///     Ctr
        /// </summary>
        private Courier(string name, int speed, Location location, StoragePlace storagePlace) : this()
        {
            Id = Guid.NewGuid();
            Name = name;
            Speed = speed;
            Location = location;
            StoragePlaces = [storagePlace];
        }

        /// <summary>
        /// Идентификатор курьера
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Имя курьера
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Скорость курьера
        /// </summary>
        public int Speed { get; private set; }

        /// <summary>
        /// Местоположение курьера
        /// </summary>
        public Location Location { get; private set; }

        /// <summary>
        /// Места хранения курьера
        /// </summary>
        public List<StoragePlace> StoragePlaces { get; private set; }

        /// <summary>
        /// Фабричный метод
        /// </summary>
        /// <param name="name"></param>
        /// <param name="speed"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Result<Courier, Error> Create(string name, int speed, Location location)
        {
            if (string.IsNullOrEmpty(name))
            {
                return GeneralErrors.ValueIsInvalid(string.Concat(EmptyValueError, nameof(name)));
            }

            if (speed <= 0)
            {
                return GeneralErrors.ValueIsInvalid(string.Concat(ValueZeroOrLessError, nameof(speed)));
            }

            if (location == null)
            {
                return GeneralErrors.ValueIsRequired(nameof(location));
            }

            return new Courier(name, speed, location, StoragePlace.DefaultStoragePlace);
        }

        /// <summary>
        /// Добавить место хранения
        /// </summary>
        /// <param name="name"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public UnitResult<Error> AddStoragePlace(string name, int volume)
        {
            StoragePlaces.Add(new StoragePlace(name, volume));
            return new UnitResult<Error>();
        }

        /// <summary>
        /// Проверить можно ли взять заказ
        /// </summary>
        /// <param name="volume"></param>
        /// <returns></returns>
        public Result<bool, Error> CanTakeVolumeForOrder(int volume)
        {
            if (volume <= 0)
            {
                return GeneralErrors.ValueIsInvalid(string.Concat(ValueZeroOrLessError, nameof(volume)));
            }

            if (StoragePlaces.Exists(sp => sp.IsEmpty() && sp.TotalVolume >= volume))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Взять заказ
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public Result<Error> TakeOrder(Order order)
        {
            var canTake = CanTakeVolumeForOrder(order.Volume);
            if (canTake.IsFailure)
            {
                return GeneralErrors.ValueIsInvalid(canTake.Error.Message);
            }
            else
            {
                var storagePlace = StoragePlaces.First(sp => sp.IsEmpty() && sp.TotalVolume >= order.Volume);
                storagePlace.SetOrder(order.Id, order.Volume);
                return new Result<Error>();
            }
        }

        /// <summary>
        /// Завершить заказ
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public UnitResult<Error> FinishOrder(Order order)
        {
            var storagePlaceWithOrder = StoragePlaces.Find(sp => sp.OrderId == order.Id);
            if (storagePlaceWithOrder == null)
            {
                return GeneralErrors.ValueIsInvalid(string.Concat(NotFoundOrderError, order));
            }

            var result = storagePlaceWithOrder.Extract();
            if (result.IsFailure)
            {
                return GeneralErrors.ValueIsInvalid(result.Error.Message);
            }

            return new UnitResult<Error>();
        }

        /// <summary>
        /// Рассчитать количество шагов до местоположения
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Result<int, Error> CalculateStepsToLocation(Location target)
        {
            if (target == null)
            {
                return GeneralErrors.ValueIsRequired(nameof(target));
            }

            var distance = target.CalculateDistance(Location);
            var steps = (int)Math.Ceiling(distance / (double)Speed);

            return steps;
        }

        /// <summary>
        ///     Изменить местоположение
        /// </summary>
        /// <param name="target">Целевое местоположение</param>
        /// <returns>Местоположение после сдвига</returns>
        public UnitResult<Error> Move(Location target)
        {
            if (target == null) return GeneralErrors.ValueIsRequired(nameof(target));

            var difX = target.X - Location.X;
            var difY = target.Y - Location.Y;
            var cruisingRange = Speed;

            var moveX = Math.Clamp(difX, -cruisingRange, cruisingRange);
            cruisingRange -= Math.Abs(moveX);

            var moveY = Math.Clamp(difY, -cruisingRange, cruisingRange);

            var locationCreateResult = Location.Create(Location.X + moveX, Location.Y + moveY);
            if (locationCreateResult.IsFailure) return locationCreateResult.Error;
            Location = locationCreateResult.Value;

            return UnitResult.Success<Error>();
        }

    }
}
