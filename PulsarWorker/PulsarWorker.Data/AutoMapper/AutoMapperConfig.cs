using AutoMapper;
using PulsarWorker.Data.Entities;
using PulsarWorker.Data.PulsarMessages;

namespace PulsarWorker.Data.AutoMapper;

public sealed class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<BaseMessage, PulsarMessageEntity>()
            .ReverseMap();
    }
}
