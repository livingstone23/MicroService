using Azure.Messaging.ServiceBus;
using Manager.Services.OrderAPI.Messages;
using Manager.Services.OrderAPI.Models;
using Manager.Services.OrderAPI.Repository;
using Newtonsoft.Json;
//using Newtonsoft.Json;
using System.Text;
using Manager.MessageBus;

//using Manager.MessageBus;

namespace Manager.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly OrderRepository _orderRepository;


        private readonly string serviceBusConnectionString;
        private readonly string subscriptionCheckOut;
        private readonly string checkoutMessageTopic;

        private readonly IConfiguration _configuration;


        //Vid130.1- Variable para recibir los mensajes del service bus
        private ServiceBusProcessor checkOutProcessor;

        //Vid149.4 Consume payment Status para recibir los mensajes del service bus
        private ServiceBusProcessor orderUpdatePaymentStatusProcessor;


        //Vid143.3 Inyectamos IMessageBus para integrar PaymentStatus
        private readonly IMessageBus _messageBus;

        //Vid144.2 public payment message
        private readonly string orderPaymentProcessTopic;

        //Vid149.2 Consume payment Status
        private readonly string orderUpdatePaymentResultTopic;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            
            _orderRepository = orderRepository;
            _configuration = configuration;
            //Vid143. 5 Inyectamos IMessageBus
            _messageBus = messageBus;


            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");
            checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");

            //Vid130.2- Variable para recibir los mensajes del service bus
            var client = new ServiceBusClient(serviceBusConnectionString);
            checkOutProcessor = client.CreateProcessor(checkoutMessageTopic, subscriptionCheckOut);

            //Vid 153.2 Si utilizaramos solo Queue sin asociar a un topic
            //checkOutProcessor = client.CreateProcessor(checkoutMessageTopic);


            //Vid144.3 public payment message
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");

            //Vid149.3 Consume payment Status
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            //Vid149.5 Consume payment Status
            orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderPaymentProcessTopic, subscriptionCheckOut);


        }

        public async Task Start()
        {

            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();

            //Vid149.6 Ejecutamos el proceso para conocer el cambio de status del pago
            orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentStatusProcessor.StartProcessingAsync();

        }

        public async Task Stop()
        {
            
            await checkOutProcessor.StopProcessingAsync();
            await checkOutProcessor.DisposeAsync();


            //Vid149.7 Ejecutamos el proceso para detener el consumo de proceso. 
            await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
            await orderUpdatePaymentStatusProcessor.DisposeAsync();

        }

        private Task ErrorHandler(ProcessErrorEventArgs arg)
        {

            Console.WriteLine(arg.Exception.ToString());
            return  Task.CompletedTask;

        }


        /// <summary>
        /// Metodo revisa lo que proviene del AzureServiceBus
        /// Captura y deserealiza el mensaje.
        /// </summary>
        /// <returns></returns>
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId = checkoutHeaderDto.UserId,
                FirstName = checkoutHeaderDto.FirstName,
                LastName = checkoutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkoutHeaderDto.CardNumber,
                CouponCode = checkoutHeaderDto.CouponCode,
                CVV = checkoutHeaderDto.CVV,
                DiscountTotal = checkoutHeaderDto.DiscountTotal,
                Email = checkoutHeaderDto.Email,
                ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkoutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkoutHeaderDto.Phone,
                PickupDateTime = checkoutHeaderDto.PickupDateTime
            };

            foreach (var detailList in checkoutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = detailList.ProductId,
                    ProductName = detailList.Product.Name,
                    Price = detailList.Product.Price,
                    Count = detailList.Count
                };
                orderHeader.CartTotalItems += detailList.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            await _orderRepository.AddOrder(orderHeader);


            //Vid143. 6 Inyectamos Creamos el llamado al objeto de pago
            PaymentRequestMessage paymentRequestMessage = new()
            {

                Name = orderHeader.FirstName + " " + orderHeader.LastName,
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryMonthYear = orderHeader.ExpiryMonthYear,
                OrderId = orderHeader.OrderHeaderId,
                OrderTotal = orderHeader.OrderTotal,


                Email = orderHeader.Email

            };

            try
            {

                //Vid144.3 public payment message, variable  orderPaymentProcessTopic
                await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception e)
            {
                throw;
            }

        }


        /// <summary>
        /// Metodo para actualizar la actualizacion del status.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {

            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

            await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);

        }



        //private readonly string serviceBusConnectionString;
        //private readonly string subscriptionCheckOut;
        //private readonly string checkoutMessageTopic;
        //private readonly string orderPaymentProcessTopic;
        //private readonly string orderUpdatePaymentResultTopic;

        //

        //private ServiceBusProcessor checkOutProcessor;
        //private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

        //private readonly IConfiguration _configuration;
        ////private readonly IMessageBus _messageBus;

        //public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        //{
        //    _orderRepository = orderRepository;
        //    _configuration = configuration;
        //    _messageBus = messageBus;

        //    serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        //    subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");
        //    checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
        //    orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopics");
        //    orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");


        //    var client = new ServiceBusClient(serviceBusConnectionString);

        //    checkOutProcessor = client.CreateProcessor(checkoutMessageTopic);
        //    orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionCheckOut);
        //}

        //public async Task Start()
        //{
        //    checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
        //    checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        //    await checkOutProcessor.StartProcessingAsync();

        //    orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        //    orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        //    await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
        //}
        //public async Task Stop()
        //{
        //    await checkOutProcessor.StopProcessingAsync();
        //    await checkOutProcessor.DisposeAsync();

        //    await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        //    await orderUpdatePaymentStatusProcessor.DisposeAsync();
        //}
        //Task ErrorHandler(ProcessErrorEventArgs args)
        //{
        //    Console.WriteLine(args.Exception.ToString());
        //    return Task.CompletedTask;
        //}

        //private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        //{
        //    var message = args.Message;
        //    var body = Encoding.UTF8.GetString(message.Body);

        //    CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

        //    OrderHeader orderHeader = new()
        //    {
        //        UserId = checkoutHeaderDto.UserId,
        //        FirstName = checkoutHeaderDto.FirstName,
        //        LastName = checkoutHeaderDto.LastName,
        //        OrderDetails = new List<OrderDetails>(),
        //        CardNumber = checkoutHeaderDto.CardNumber,
        //        CouponCode = checkoutHeaderDto.CouponCode,
        //        CVV = checkoutHeaderDto.CVV,
        //        DiscountTotal = checkoutHeaderDto.DiscountTotal,
        //        Email = checkoutHeaderDto.Email,
        //        ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
        //        OrderTime = DateTime.Now,
        //        OrderTotal = checkoutHeaderDto.OrderTotal,
        //        PaymentStatus = false,
        //        Phone = checkoutHeaderDto.Phone,
        //        PickupDateTime = checkoutHeaderDto.PickupDateTime
        //    };
        //    foreach(var detailList in checkoutHeaderDto.CartDetails)
        //    {
        //        OrderDetails orderDetails = new()
        //        {
        //            ProductId = detailList.ProductId,
        //            ProductName = detailList.Product.Name,
        //            Price = detailList.Product.Price,
        //            Count = detailList.Count
        //        };
        //        orderHeader.CartTotalItems += detailList.Count;
        //        orderHeader.OrderDetails.Add(orderDetails);
        //    }

        //    await _orderRepository.AddOrder(orderHeader);


        //    PaymentRequestMessage paymentRequestMessage = new()
        //    {
        //        Name = orderHeader.FirstName + " " + orderHeader.LastName,
        //        CardNumber = orderHeader.CardNumber,
        //        CVV = orderHeader.CVV,
        //        ExpiryMonthYear = orderHeader.ExpiryMonthYear,
        //        OrderId = orderHeader.OrderHeaderId,
        //        OrderTotal = orderHeader.OrderTotal,
        //        Email=orderHeader.Email
        //    };

        //    try
        //    {
        //        await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
        //        await args.CompleteMessageAsync(args.Message);
        //    }
        //    catch(Exception e)
        //    {
        //        throw;
        //    }

        //}

        //private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        //{
        //    var message = args.Message;
        //    var body = Encoding.UTF8.GetString(message.Body);

        //    UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        //    await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
        //    await args.CompleteMessageAsync(args.Message);

        //}
    }
}
