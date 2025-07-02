using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Domain.Services
{
    public class DispatchService : IDispatchService
    {
        public Result<Courier, Error> Dispatch(Order order, List<Courier> couriers)
        {
            if (order == null)
            {
                return GeneralErrors.ValueIsRequired(nameof(order));
            }

            if (order.Status != OrderStatus.Created)
            {
                return Errors.OnlyCreatedStatusOrder(order);
            }

            if (couriers.Count == 0)
            {
                return Errors.EmptyCouriersList();
            }

            var canTakeOrderCouriers = couriers
                .Where(c => c.CanTakeVolumeForOrder(order.Volume).Value);

            Courier resultCourier = null;
            var minDistance = int.MaxValue;

            if (!canTakeOrderCouriers.Any()) return Errors.NoFoundCourier();

            foreach (var courier in canTakeOrderCouriers)
            {
                var steps = courier.CalculateStepsToLocation(order.Location);
                if (steps.IsSuccess && steps.Value < minDistance)
                {
                    minDistance = steps.Value;
                    resultCourier = courier;
                }
            }

            if (resultCourier == null) return Errors.NoFoundCourier();

            var assignResult = order.Assign(resultCourier);
            if (assignResult.IsFailure) return assignResult.Error;

            var takeOrderResult = resultCourier.TakeOrder(order);
            if (takeOrderResult.IsFailure) return takeOrderResult.Error;

            return resultCourier;
        }

        public static class Errors
        {
            public static Error OnlyCreatedStatusOrder(Order order)
            {
                return new Error($"{nameof(order).ToLowerInvariant()}.is not in Created status", "Назначать на курьера заказ можно только в статусе Created");
            }

            public static Error EmptyCouriersList()
            {
                return new Error($"{nameof(EmptyCouriersList).ToLowerInvariant()}", "Пустой список курьеров для назначения заказа");
            }

            public static Error NoFoundCourier()
            {
                return new Error($"{nameof(NoFoundCourier).ToLowerInvariant()}", "Не найдено подходящих курьеров для заказа");
            }
        }
    }
}
