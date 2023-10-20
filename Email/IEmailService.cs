using Domain;

namespace Email;

public interface IEmailService
{
    public Task SendNewEventsEmail(NewEventsEmailModel email);
}
