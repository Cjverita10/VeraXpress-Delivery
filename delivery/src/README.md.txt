# VeraXpress Delivery System

Sistema de consola desarrollado en C# / .NET para la gestión, decoración y despacho de pedidos de electrodomésticos.

## 📋 Resumen del Problema
VeraXpress requiere procesar órdenes de compras de electrodomésticos con opciones de empaquetado dinámico, gestión de datos en caché para productos y coordinación de múltiples servicios internos (Inventario, Pagos, Navegación).

## 🛠️ Patrones de Diseño Implementados

* **Builder Pattern:** `DeliveryOrderBuilder` para la construcción fluida de pedidos.
* **Decorator Pattern:** `BubbleWrapDecorator` para adicionar costos y servicios de empaque sin modificar la orden base.
* **Proxy Pattern:** `ApplianceCacheProxy` para almacenar y consultar eficientemente fichas técnicas.
* **Facade Pattern:** `DispatchFacade` para simplificar la ejecución del despacho involucrando múltiples subsistemas.

## 🚀 Instrucciones de Ejecución

### Requisitos Previos
* [.NET SDK 6.0](https://dotnet.microsoft.com/download) o superior.

### Pasos para Ejecutar

1. Clona o ubícate en la raíz del repositorio.
2. Navega a la carpeta de código fuente:
   ```bash
   cd src