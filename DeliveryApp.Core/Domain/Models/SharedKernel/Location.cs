using CSharpFunctionalExtensions;
using Primitives;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Models.SharedKernel
{
    /// <summary>
    /// Координата
    /// </summary>
    public class Location : ValueObject
    {
        /// <summary>
        /// Константа с минимальным значением 
        /// </summary>
        private const int MinValue = 1;

        /// <summary>
        /// Константа с максимальным значением 
        /// </summary>
        private const int MaxValue = 10;

        /// <summary>
        /// Сообщение при ошибке валидации значения
        /// </summary>
        private const string ErrorMessage = "Значение для {0} может быть в диапазоне от 1 до 10 включительно";

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        [ExcludeFromCodeCoverage]
        private Location() { }


        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="x">Координата по оси X</param>
        /// <param name="y">Координата по оси Y</param>
        private Location(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Координата X
        /// </summary>
        public byte X { get; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public byte Y { get; }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="x">Координата по оси X</param>
        /// <param name="y">Координата по оси Y</param>
        /// <returns>Результат</returns>
        public static Result<Location, Error> Create(byte x, byte y)
        {

            if (x < MinValue || x > MaxValue)
            {
                return GeneralErrors.ValueIsInvalid(string.Format(ErrorMessage, nameof(x)));
            }

            if (y < MinValue || y > MaxValue)
            {
                return GeneralErrors.ValueIsInvalid(string.Format(ErrorMessage, nameof(y)));
            }

            return new Location(x, y);
        }

        /// <summary>
        /// Создать случайную координату
        /// </summary>
        public static Result<Location, Error> CreateRandom()
        {
            var randomX = (byte)Random.Shared.Next(MinValue, MaxValue + 1);
            var randomY = (byte)Random.Shared.Next(MinValue, MaxValue + 1);
            return Create(randomX, randomY);
        }

        public Result<int, Error> CalculateDistance(Location to)
        {
            return Math.Abs(to.X - X) + Math.Abs(to.Y - Y);
        }

        [ExcludeFromCodeCoverage]
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }
    }
}
