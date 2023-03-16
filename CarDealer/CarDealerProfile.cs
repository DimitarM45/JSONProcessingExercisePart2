namespace CarDealer;

using Models;
using DTOs.Import;

using AutoMapper;

public class CarDealerProfile : Profile
{
    public CarDealerProfile()
    {
        CreateMap<ImportSupplierDto, Supplier>();

        CreateMap<ImportPartDto, Part>();

        CreateMap<ImportCarDto, Car>();

        CreateMap<ImportCustomerDto, Customer>();

        CreateMap<ImportSaleDto, Sale>();
    }
}
