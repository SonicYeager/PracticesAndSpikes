﻿using AutoMapper;
using HotChocolate.Checker.GraphQL.Types;
using HotChocolate.Checker.Persistence.Entities;

namespace HotChocolate.Checker.AutoMapper;

public sealed class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<UserEntity, User>();
        CreateMap<BookEntity, Book>();
    }
}
