using Core.Dto;

namespace Core.Import;

public static class MixedImporter
{
    public static void Load(string path)
    {
        foreach (string line in File.ReadLines(path))
        {
            string[] parts = line.Split(';');

            object result = parts switch
            {
                ["P", var id, var name, var price]
                    when decimal.TryParse(price, out decimal p)
                    => new ProductDto(id, name, p),

                ["W", var id, var name, var address]
                    => new WarehouseDto(id, name, address),

                _ => throw new FormatException("Невідомий формат рядка")
            };

            switch (result)
            {
                case ProductDto product:
                    Console.WriteLine(
                        $"Товар: {product.Id}, {product.Name}, {product.Price}");
                    break;

                case WarehouseDto warehouse:
                    Console.WriteLine(
                        $"Склад: {warehouse.Id}, {warehouse.Name}, {warehouse.Address}");
                    break;
            }
        }
    }
}