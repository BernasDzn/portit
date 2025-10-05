namespace Api.Infrastructure.Utilities;


internal interface IDTOAble<T>
{
    T ToDTO();
}