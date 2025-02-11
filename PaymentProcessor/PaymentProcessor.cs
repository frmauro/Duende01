namespace PaymentProcessor;

public class PaymentProcessor : IPaymentProcessor
{
    bool IPaymentProcessor.PaymentProcessor()
    {
        return true;
    }
}
