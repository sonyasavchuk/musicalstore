using System.Net.Mail;

namespace MusicalStore.Services.Identity;

public interface ISmtpClientFactory
{
    string DisplayName { get; }

    SmtpClient CreateClient();
}
