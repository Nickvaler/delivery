using CSharpFunctionalExtensions;
using System.Diagnostics.CodeAnalysis;

namespace DeliveryApp.Core.Domain.Models.OrderAggregate
{
    public class OrderStatus : ValueObject
    {
        public static OrderStatus Created => new(nameof(Created).ToLowerInvariant());
        public static OrderStatus Assigned => new(nameof(Assigned).ToLowerInvariant());
        public static OrderStatus Completed => new(nameof(Completed).ToLowerInvariant());

        /// <summary>
        ///     Ctr
        /// </summary>
        [ExcludeFromCodeCoverage]
        private OrderStatus() { }

        /// <summary>
        ///     Ctr
        /// </summary>
        private OrderStatus(string name) : this()
        {
            Name = name;
        }

        public string Name { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
        }
    }
}
