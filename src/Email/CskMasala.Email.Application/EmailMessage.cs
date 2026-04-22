namespace CskMasala.Email.Application;

public record EmailMessage(string To, string Subject, string HtmlBody);
