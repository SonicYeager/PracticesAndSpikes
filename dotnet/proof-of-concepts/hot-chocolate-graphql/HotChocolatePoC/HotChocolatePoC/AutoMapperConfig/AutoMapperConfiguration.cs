using AutoMapper;
using HotChocolatePoC.Database.Entities;
using HotChocolatePoC.Types;

namespace HotChocolatePoC.AutoMapperConfig
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            CreateMap<ArticleDto, ArticleEntity>()
                .ForMember(dest => dest.CustomsTariffNumber, opt => opt.MapFrom<CustomsTariffRateResolver>())
                .ReverseMap();
            CreateMap<ArticleAddDto, ArticleEntity>()
                .ReverseMap();
            CreateMap<ImageDto, ImageEntity>()
                .ReverseMap();
            CreateMap<CommentDto, ArticleCommentEntity>()
                .ReverseMap();
            CreateMap<SupplierPropertiesDto, ArticleSupplierPropertiesEntity>()
                .ForMember(dst => dst.OuterBoxHeight, o => o.MapFrom(src => src.OuterBox!.Dimensions!.Height))
                .ForMember(dst => dst.OuterBoxLength, o => o.MapFrom(src => src.OuterBox!.Dimensions!.Length))
                .ForMember(dst => dst.OuterBoxWidth, o => o.MapFrom(src => src.OuterBox!.Dimensions!.Width))
                .ForMember(dst => dst.OuterBoxWeight, o => o.MapFrom(src => src.OuterBox!.BoxWeight))
                .ForMember(dst => dst.OuterBoxWrappingBoardWeight, o => o.MapFrom(src => src.OuterBox!.WrappingBoardWeight))
                .ForMember(dst => dst.OuterBoxWrappingPlasticWeight, o => o.MapFrom(src => src.OuterBox!.WrappingPlasticWeight))
                .ForMember(dst => dst.InnerBoxesPerOuterBox, o => o.MapFrom(src => src.OuterBox!.InnerBoxesPerOuterBox))
                .ForMember(dst => dst.InnerBoxHeight, o => o.MapFrom(src => src.InnerBox!.Dimensions!.Height))
                .ForMember(dst => dst.InnerBoxLength, o => o.MapFrom(src => src.InnerBox!.Dimensions!.Length))
                .ForMember(dst => dst.InnerBoxWidth, o => o.MapFrom(src => src.InnerBox!.Dimensions!.Width))
                .ForMember(dst => dst.InnerBoxWeight, o => o.MapFrom(src => src.InnerBox!.BoxWeight))
                .ForMember(dst => dst.InnerBoxWrappingBoardWeight, o => o.MapFrom(src => src.InnerBox!.WrappingBoardWeight))
                .ForMember(dst => dst.InnerBoxWrappingPlasticWeight, o => o.MapFrom(src => src.InnerBox!.WrappingPlasticWeight))
                .ReverseMap();
        }
    }
}