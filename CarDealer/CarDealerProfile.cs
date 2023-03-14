namespace CarDealer;

using DTOs.Import;
using Models;

using AutoMapper;

public class CarDealerProfile : Profile
{
    public CarDealerProfile()
    {
        CreateMap<ImportSupplierDto, Supplier>();

        CreateMap<ImportPartDto, Part>();
    }
}
