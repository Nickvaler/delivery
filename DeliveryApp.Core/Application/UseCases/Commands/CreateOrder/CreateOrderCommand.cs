using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<UnitResult<Error>>
    {
        /// <summary>
        ///     Ctr
        /// </summary>
        private CreateOrderCommand() { }
        private CreateOrderCommand(Guid basketId, string street)
        {
            BasketId = basketId;
            Street = street;
        }

        /// <summary>
        ///     Идентификатор корзины
        /// </summary>
        /// <remarks>Id корзины берется за основу при создании Id заказа, они совпадают</remarks>
        public Guid BasketId { get; }

        /// <summary>
        ///     Улица
        /// </summary>
        public string Street { get; }

        public static Result<CreateOrderCommand, Error> Create(Guid basketId, string street)
        {
            if (Guid.Empty == basketId)
            {
                return GeneralErrors.ValueIsInvalid(nameof(basketId));
            }
            if (string.IsNullOrWhiteSpace(street))
            {
                return GeneralErrors.ValueIsInvalid(nameof(street));
            }
            return new CreateOrderCommand(basketId, street);
        }
    }
}
