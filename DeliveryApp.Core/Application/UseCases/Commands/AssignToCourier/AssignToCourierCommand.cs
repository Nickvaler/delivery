using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignToCourier
{
    public class AssignToCourierCommand : IRequest<UnitResult<Error>>;
}
