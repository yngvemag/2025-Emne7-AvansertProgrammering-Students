namespace StudentBloggAPI.Features.Common.Interfaces;

public interface IMapper<TDto, TModel>
    where TModel : class
    where TDto : class
{
    TDto MapToDto(TModel model);
    TModel MapToModel(TDto dto);
}