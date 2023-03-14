namespace CarDealer;

using AutoMapper;
using CarDealer.Models;
using Data;
using DTOs.Import;

using Newtonsoft.Json;

public class StartUp
{
	public static void Main()
	{
		using CarDealerContext context = new CarDealerContext();

		string inputJson = File.ReadAllText(@"../../../Datasets/parts.json");

        Console.WriteLine(ImportParts(context, inputJson));
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

		IMapper mapper = CreateMapper();

		Part[] parts = mapper.Map<Part[]>(partDtos);

		context.Parts!.AddRange(parts);
		context.SaveChanges();

		return $"Successfully imported {parts.Length}.";
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