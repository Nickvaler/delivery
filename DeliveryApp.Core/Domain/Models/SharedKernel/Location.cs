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
        private Location(int x, int y) : this()
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Координата X
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Factory Method
        /// </summary>
        /// <param name="x">Координата по оси X</param>
        /// <param name="y">Координата по оси Y</param>
        /// <returns>Результат</returns>
        public static Result<Location, Error> Create(int x, int y)
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
            var randomX = Random.Shared.Next(MinValue, MaxValue + 1);
            var randomY = Random.Shared.Next(MinValue, MaxValue + 1);
            return Create(randomX, randomY);
        }

        public int CalculateDistance(Location to)
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
