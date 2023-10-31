using AutoMapper;
using EKO.ConnectFour.Domain.GameDomain;
using EKO.ConnectFour.Domain.GameDomain.Contracts;
using EKO.ConnectFour.Domain.GridDomain;

namespace EKO.ConnectFour.Api.Models;

public class MoveInputModel
{
    public MoveType Type { get; set; }
    public DiscType DiscType { get; set; }
    public int Column { get; set; }

    private class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MoveInputModel, IMove>().ConstructUsing(model => new Move(model.Column, model.Type, model.DiscType)!);
        }
    }
}