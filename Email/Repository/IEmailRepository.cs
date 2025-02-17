using Email.Messages;

namespace Email.Repository;

public interface IEmailRepository
{
    Task LogEmail(UpdatePaymentResultMessage message);
}
