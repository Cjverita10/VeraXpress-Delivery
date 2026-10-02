using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace VeraXpressDelivery
{
    public class Product
    {
        public string Name { get; set; }
        public double PriceCOP { get; set; }
        public int Stock { get; set; }
        public string SpecSummary { get; set; }
        public string AsciiArt { get; set; }

        public Product(string name, double priceCOP, int stock, string specSummary, string asciiArt = null)
        {
            Name = name;
            PriceCOP = priceCOP;
            Stock = stock;
            SpecSummary = specSummary;
            AsciiArt = asciiArt;
        }
    }

    public class DeliveryOrder
    {
        public string VehicleName { get; set; }
        public string VehicleArt { get; set; }
        public string ApplianceName { get; set; }
        public int Quantity { get; set; }
        public string Destination { get; set; }
        public double UnitPriceCOP { get; set; }

        public virtual double GetTotalPrice() => UnitPriceCOP * Quantity;
        public virtual string GetDescription() => $"Transporte: [{VehicleName}] -> Producto: {Quantity}x {ApplianceName} | Destino: {Destination}";

        public virtual void DisplayReceipt()
        {
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("                DETALLE DE LA COMPRA Y ENVÍO - VERAXPRESS                 ");
            Console.WriteLine("==========================================================================");
            Console.WriteLine($"  RESUMEN:  {GetDescription()}");
            Console.WriteLine($"  TOTAL:    ${GetTotalPrice():N0} COP");
            Console.WriteLine("==========================================================================\n");
        }
    }

    public class DeliveryOrderBuilder
    {
        private readonly DeliveryOrder _order = new DeliveryOrder();

        public DeliveryOrderBuilder SetVehicle(string vehicleName, string vehicleArt)
        {
            _order.VehicleName = vehicleName;
            _order.VehicleArt = vehicleArt;
            return this;
        }

        public DeliveryOrderBuilder SetAppliance(string appliance, double unitPriceCOP, int quantity)
        {
            _order.ApplianceName = appliance;
            _order.UnitPriceCOP = unitPriceCOP;
            _order.Quantity = quantity;
            return this;
        }

        public DeliveryOrderBuilder SetDestination(string destination)
        {
            _order.Destination = destination;
            return this;
        }

        public DeliveryOrder Build() => _order;
    }

    // Decorator base (antes estaba duplicado y anidado, por eso no compilaba)
    public abstract class OrderDecorator : DeliveryOrder
    {
        protected readonly DeliveryOrder _baseOrder;

        protected OrderDecorator(DeliveryOrder order)
        {
            _baseOrder = order ?? throw new ArgumentNullException(nameof(order));

            // Copiamos los datos de la orden base a esta instancia
            VehicleName = _baseOrder.VehicleName;
            VehicleArt = _baseOrder.VehicleArt;
            ApplianceName = _baseOrder.ApplianceName;
            Quantity = _baseOrder.Quantity;
            Destination = _baseOrder.Destination;
            UnitPriceCOP = _baseOrder.UnitPriceCOP;
        }

        public override double GetTotalPrice() => _baseOrder.GetTotalPrice();
        public override string GetDescription() => _baseOrder.GetDescription();
    }

    public class BubbleWrapDecorator : OrderDecorator
    {
        public BubbleWrapDecorator(DeliveryOrder order) : base(order) { }

        public override double GetTotalPrice() => _baseOrder.GetTotalPrice() + (20000 * _baseOrder.Quantity);
        public override string GetDescription() => _baseOrder.GetDescription() + " + [Empaquetado con Burbujas de Protección]";
    }

    public interface IApplianceInfo
    {
        void DisplaySummary();
    }

    public class RealApplianceData : IApplianceInfo
    {
        private readonly Product _product;

        public RealApplianceData(Product product)
        {
            _product = product;
        }

        public void DisplaySummary()
        {
            Console.WriteLine($"  --> [FICHA TÉCNICA]: {_product.SpecSummary}");
        }
    }

    public class ApplianceCacheProxy : IApplianceInfo
    {
        private static readonly Dictionary<string, RealApplianceData> _cache = new Dictionary<string, RealApplianceData>();
        private readonly Product _product;

        public ApplianceCacheProxy(Product product) { _product = product; }

        public void DisplaySummary()
        {
            if (!_cache.ContainsKey(_product.Name))
            {
                _cache[_product.Name] = new RealApplianceData(_product);
            }
            _cache[_product.Name].DisplaySummary();
        }
    }

    public class InventorySubsystem
    {
        public void ReserveStock(Product product, int qty)
        {
            product.Stock -= qty;
            Console.WriteLine($" [Inventario] Se han descontado {qty} unidad(es) de '{product.Name}'. Stock disponible actual: {product.Stock} ud(s).");
        }
    }

    public class PaymentSubsystem
    {
        public void ProcessPayment(double amount) =>
            Console.WriteLine($" [ContraEntrega] Cobro verificado con éxito por un monto total de ${amount:N0} COP.");
    }

    public class NavigationSubsystem
    {
        public void LaunchDelivery(string vehicleName, string vehicleArt)
        {
            Console.WriteLine($"\n [Vehículo Seleccionado]: {vehicleName}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(vehicleArt);
            Console.ResetColor();

            Console.Write(" [Waze] Vehículo enrutando hacia el destino");
            for (int i = 0; i < 4; i++)
            {
                Thread.Sleep(350);
                Console.Write(".");
            }
            Console.WriteLine(" ¡Entrega Concretada!");
        }
    }

    public class DispatchFacade
    {
        private readonly InventorySubsystem _inventory = new InventorySubsystem();
        private readonly PaymentSubsystem _payment = new PaymentSubsystem();
        private readonly NavigationSubsystem _navigation = new NavigationSubsystem();

        public void ProcessAndDispatch(DeliveryOrder order, Product product)
        {
            Console.WriteLine("\n==========================================================================");
            Console.WriteLine("                    DESPACHO EN PROGRESO - VERAXPRESS DELIVERY            ");
            Console.WriteLine("==========================================================================");

            _inventory.ReserveStock(product, order.Quantity);
            Thread.Sleep(400);
            _payment.ProcessPayment(order.GetTotalPrice());
            Thread.Sleep(400);
            _navigation.LaunchDelivery(order.VehicleName, order.VehicleArt);
            Thread.Sleep(400);

            Console.WriteLine("\n ¡EL DOMICILIO HA SIDO ENTREGADO CON ÉXITO - GRACIAS POR SU COMPRA!");
            Console.WriteLine("==========================================================================\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Para que se vean bien tildes, ñ y el arte ASCII
            Console.OutputEncoding = Encoding.UTF8;

            string motoArt =
                "             ,------------.\n" +
                "            | VeraXpress  |\n" +
                "   __o      |   MOTO      |\n" +
                " _`\\<,_     '------------'\n" +
                "(*)/ (*)======[EXPRESS DELIVERY]\n" +
                "===================================";

            string camionArt =
                "       ________________________\n" +
                "      | VERAXPRESS CARGA Heavy |--.\n" +
                "      |                        |  |_____\n" +
                "      |= = = = = = = = = = = = |  | ___ \\\n" +
                "      \"(@)------------------(@)\"--'\"(@)\"--'";

            string tvArt =
                "       .-----------------------------------.\n" +
                "       |  _______________________________  |\n" +
                "       | |                               | |\n" +
                "       | |    KALLEY SMART TV 55\" 4K     | |\n" +
                "       | |     [ UHD / HDR / 120Hz ]     | |\n" +
                "       | |_______________________________| |\n" +
                "       |___________________________________|\n" +
                "                      |   |                 \n" +
                "                    __|___|__               \n" +
                "                   [_________]";

            string airFryerArt =
                "       ,-----------------.\n" +
                "      /   [ DIGITAL ]     \\\n" +
                "     |    (180°C) (15m)    |\n" +
                "     |---------------------|\n" +
                "     |    O   O   O   O    |\n" +
                "     |=====================|\n" +
                "     |                     |  [Air Fryer Oster Pro]\n" +
                "     |   .-------------.   |\n" +
                "     |   |             |   |\n" +
                "     |   '-------------'   |\n" +
                "     |                     |\n" +
                "     |   /=============\\   |\n" +
                "     |  ||   [MANIJA]  ||  |\n" +
                "     |   \\=============/   |\n" +
                "     |                     |\n" +
                "     '---------------------'";

            List<Product> catalog = new List<Product>
            {
                new Product("Televisor Kalley 55 UHD 4K Smart", 1500000, 3, "Pantalla 55\", 4K UHD, HDR10, Smart TV.", tvArt),
                new Product("Air Fryer Oster Pro 4L", 250000, 5, "Digital, 1500W, Antiadherente Bioceramic.", airFryerArt),
                new Product("Licuadora Black+Decker 10Vel", 200000, 0, "Cuchillas de acero inox, Jarra de vidrio 1.5L.", null)
            };

            bool keepShopping = true;

            while (keepShopping)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("==========================================================================");
                Console.WriteLine("   VERAXPRESS DELIVERY - DISTRIBUIDORA DE ELECTRODOMÉSTICOS Y DOMICILIOS XPRESS");
                Console.WriteLine("==========================================================================");
                Console.ResetColor();

                Console.WriteLine("\n--- CATÁLOGO DE ELECTRODOMÉSTICOS DISPONIBLES ---");
                for (int i = 0; i < catalog.Count; i++)
                {
                    var p = catalog[i];
                    string stockStr = p.Stock > 0 ? $"[Stock: {p.Stock} uds]" : "Stock: 0 uds";
                    Console.WriteLine($"{i + 1}) {p.Name} - ${p.PriceCOP:N0} COP | {stockStr}");
                }

                Product selectedProduct = null;
                while (selectedProduct == null)
                {
                    Console.Write($"\nSelecciona un producto (1-{catalog.Count}): ");
                    string input = Console.ReadLine()?.Trim();
                    if (int.TryParse(input, out int choice) && choice >= 1 && choice <= catalog.Count)
                    {
                        Product chosen = catalog[choice - 1];
                        if (chosen.Stock <= 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Producto agotado. Selecciona otra opción.");
                            Console.ResetColor();
                        }
                        else
                        {
                            selectedProduct = chosen;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Opción no válida. Intenta nuevamente.");
                    }
                }

                Console.WriteLine("\n VISTA PREVIA DEL PRODUCTO:");
                if (!string.IsNullOrEmpty(selectedProduct.AsciiArt))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(selectedProduct.AsciiArt);
                    Console.ResetColor();
                }

                IApplianceInfo proxy = new ApplianceCacheProxy(selectedProduct);
                proxy.DisplaySummary();

                int quantity = 0;
                while (quantity <= 0 || quantity > selectedProduct.Stock)
                {
                    Console.Write($"\n¿Cuántas unidades deseas adquirir? (Disponibles: {selectedProduct.Stock}): ");
                    if (int.TryParse(Console.ReadLine()?.Trim(), out quantity))
                    {
                        if (quantity <= 0)
                        {
                            Console.WriteLine("La cantidad debe ser mayor a 0.");
                        }
                        else if (quantity > selectedProduct.Stock)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Cantidad no disponible en inventario. Máximo: {selectedProduct.Stock} uds.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ingresa un número entero válido.");
                    }
                }

                Console.WriteLine("\n--- SELECCIÓN DE VEHÍCULO PARA ENTREGA ---");
                Console.WriteLine("1) Moto Express (Entrega rápida/ligera)");
                Console.WriteLine("2) Camión Carga Pesada (Electrodomésticos grandes)");

                string vehicleName = "";
                string selectedVehicleArt = "";

                while (string.IsNullOrEmpty(vehicleName))
                {
                    Console.Write("Selecciona el tipo de transporte (1-2): ");
                    string vInput = Console.ReadLine()?.Trim();
                    switch (vInput)
                    {
                        case "1":
                            vehicleName = "Moto Express";
                            selectedVehicleArt = motoArt;
                            break;
                        case "2":
                            vehicleName = "Camión Carga Pesada";
                            selectedVehicleArt = camionArt;
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Selecciona 1 o 2.");
                            break;
                    }
                }

                Console.Write("\nIngresa la dirección de entrega: ");
                string destination = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(destination))
                {
                    destination = "Sede Principal VeraXpress";
                }

                DeliveryOrder currentOrder = new DeliveryOrderBuilder()
                    .SetAppliance(selectedProduct.Name, selectedProduct.PriceCOP, quantity)
                    .SetVehicle(vehicleName, selectedVehicleArt)
                    .SetDestination(destination)
                    .Build();

                Console.Write("\n¿Deseas adicionar Empaque de Protección Burbuja (+$20.000 COP c/u)? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() == "s")
                {
                    currentOrder = new BubbleWrapDecorator(currentOrder);
                }

                currentOrder.DisplayReceipt();

                Console.WriteLine("Presiona ENTER para confirmar la orden y procesar el despacho...");
                Console.ReadLine();

                DispatchFacade dispatch = new DispatchFacade();
                dispatch.ProcessAndDispatch(currentOrder, selectedProduct);

                Console.Write("¿Deseas realizar otro pedido en VeraXpress? (s/n): ");
                string again = Console.ReadLine()?.Trim().ToLower();
                if (again != "s")
                {
                    keepShopping = false;
                    Console.WriteLine("\n¡Gracias por utilizar el sistema de envíos VeraXpress!");
                }
            }
        }
    }
}
