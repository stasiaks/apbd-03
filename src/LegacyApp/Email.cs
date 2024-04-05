namespace LegacyApp;

public record Email(string Local, Domain Domain);

public record Domain(string SubDomain, string TopLevelDomain);
