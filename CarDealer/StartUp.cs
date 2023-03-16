namespace CarDealer;

using Data;
using Models;
using DTOs.Import;

using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Microsoft.EntityFrameworkCore;

using System.Globalization;

public class StartUp
{
    public static void Main()
    {
        using CarDealerContext context = new CarDealerContext();
    }

    //Problem 1

    public static string ImportSuppliers(CarDealerContext context, string inputJson)
    {
        ImportSupplierDto[] supplierDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson)!;

        IMapper mapper = CreateMapper();

        Supplier[] suppliers = mapper.Map<Supplier[]>(supplierDtos);

        context.Suppliers!.AddRange(suppliers);
        context.SaveChanges();

        return $"Successfully imported {suppliers.Length}.";
    }

    //Problem 2

    public static string ImportParts(CarDealerContext context, string inputJson)
    {
        ImportPartDto[] partDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson)!;

        ICollection<Part> validParts = new HashSet<Part>();

        IMapper mapper = CreateMapper();

        foreach (ImportPartDto partDto in partDtos)
        {
            if (context.Suppliers!.AsNoTracking().Any(s => s.Id == partDto.SupplierId))
            {
                Part validPart = mapper.Map<Part>(partDto);

                validParts.Add(validPart);
            }
        }

        context.Parts!.AddRange(validParts);
        context.SaveChanges();

        return $"Successfully imported {validParts.Count}.";
    }

    //Problem 3

    public static string ImportCars(CarDealerContext context, string inputJson)
    {
        ImportCarDto[] carDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson)!;

        IMapper mapper = CreateMapper();

        Car[] cars = mapper.Map<Car[]>(carDtos);

        context.Cars!.AddRange(cars);
        context.SaveChanges();

        List<PartCar> carParts = new List<PartCar>();

        for (int i = 0; i < carDtos.Length; i++)
        {
            if (context.Parts!.AsNoTracking().Any(p => carDtos[i].PartsId.Any(id => p.Id == id)))
            {
                foreach (int partId in carDtos[i].PartsId)
                {
                    PartCar partCar = new PartCar()
                    {
                        PartId = partId,
                        CarId = i + 1
                    };

                    carParts.Add(partCar);
                }
            }
        }

        context.PartsCars!.AddRange(carParts);
        context.SaveChanges();

        return $"Successfully imported {cars.Length}.";
    }

    //Problem 4

    public static string ImportCustomers(CarDealerContext context, string inputJson)
    {
        ImportCustomerDto[] customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        })!;

        IMapper mapper = CreateMapper();

        Customer[] customers = mapper.Map<Customer[]>(customerDtos);

        context.Customers?.AddRange(customers);
        context.SaveChanges();

        return $"Successfully imported {customers.Length}.";
    }

    //Problem 5

    public static string ImportSales(CarDealerContext context, string inputJson)
    {
        ImportSaleDto[] saleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson)!;

        IMapper mapper = CreateMapper();

        Sale[] sales = mapper.Map<Sale[]>(saleDtos);

        context.AddRange(sales);
        context.SaveChanges();

        return $"Successfully imported {sales.Length}.";
    }

    //Problem 7

    public static string GetOrderedCustomers(CarDealerContext context)
    {
        var customers = context.Customers!
            .AsNoTracking()
            .OrderBy(c => c.BirthDate)
            .ThenBy(c => c.IsYoungDriver)
            .Select(c => new
            {
                c.Name,
                BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                c.IsYoungDriver
            })
            .ToArray();

        string customersJson = JsonConvert.SerializeObject(customers, Formatting.Indented);

        return customersJson;
    }

    //Problem 8

    public static string GetCarsFromMakeToyota(CarDealerContext context)
    {
        var toyotaCars = context.Cars!
            .AsNoTracking()
            .Where(c => c.Make == "Toyota")
            .OrderBy(c => c.Model)
            .ThenByDescending(c => c.TraveledDistance)
            .Select(c => new
            {
                c.Id,
                c.Make,
                c.Model,
                c.TraveledDistance
            })
            .ToArray();

        string toyotaCarsJson = JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);

        return toyotaCarsJson;
    }

    //Problem 9

    public static string GetLocalSuppliers(CarDealerContext context)
    {
        var localSuppliers = context.Suppliers!
            .AsNoTracking()
            .Where(s => !s.IsImporter)
            .Select(s => new
            {
                s.Id,
                s.Name,
                PartsCount = s.Parts.Count
            })
            .ToArray();

        string localSuppliersJson = JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);

        return localSuppliersJson;
    }

    //Problem 10 

    public static string GetCarsWithTheirListOfParts(CarDealerContext context)
    {
        var carsWithParts = context.Cars!
            .AsNoTracking()
            .Include(c => c.PartsCars)
            .Select(c => new
            {
                car = new
                {
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                },
                parts = c.PartsCars
                        .Select(pc => new
                        {
                            pc.Part.Name,
                            Price = $"{pc.Part.Price:f2}"
                        })
                        .ToArray()

            })
            .ToArray();

        string carsWithPartsJson = JsonConvert.SerializeObject(carsWithParts);

        return carsWithPartsJson;
    }

    //Problem 11

    public static string GetTotalSalesByCustomer(CarDealerContext context)
    {
        var customers = context.Customers!
            .AsNoTracking()
            .Where(c => c.Sales.Count >= 1)
            .Select(c => new
            {
                FullName = c.Name,
                BoughtCars = c.Sales.Count,
                SalesPrices = c.Sales.SelectMany(s => s.Car.PartsCars.Select(pc => pc.Part.Price))
            })
            .OrderByDescending(c => c.SalesPrices.Sum())
            .ThenByDescending(c => c.BoughtCars)
            .ToArray();

        var jsonWrapper = customers
            .Select(c => new
            {
                c.FullName,
                c.BoughtCars,
                SpentMoney = c.SalesPrices.Sum()
            });

        string customersJson = JsonConvert.SerializeObject(jsonWrapper, new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() }
        });

        return customersJson;
    }

    //Problem 12 

    public static string GetSalesWithAppliedDiscount(CarDealerContext context)
    {
        var sales = context.Sales!
            .AsNoTracking()
            .Take(10)
            .Select(s => new
            {
                car = new
                {
                    s.Car.Make,
                    s.Car.Model,
                    s.Car.TraveledDistance
                },
                customerName = s.Customer.Name,
                discount = $"{s.Discount:f2}",
                price = $"{s.Car.PartsCars.Sum(pc => pc.Part.Price):f2}",
                priceWithDiscount = $"{s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100):f2}"
            })
            .ToArray();

        string salesJson = JsonConvert.SerializeObject(sales, Formatting.Indented);

        return salesJson;
    }

    private static IMapper CreateMapper()
    {
        IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CarDealerProfile>();
        }));

        return mapper;
    }
}