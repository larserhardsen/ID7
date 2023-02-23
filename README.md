# ID7-DataProtection-in-SQL
This is a PoC on adding SQLServer as backend for the ASP.Net Core DataProtection.

To use, copy the output files from the project into the IdentityServer 7 folder and run the ``ID7.exe`` file. I have not tested this with the IIS integration yet.

Please add a connection string to the /Config/Sitecore.IdentityServer.Host.xml file, like this:
```<?xml version="1.0" encoding="utf-8" ?>
<Settings>
  <Sitecore>
    <IdentityServerHost>
      <DefaultCulture>en</DefaultCulture>
      <KeysConnectionString>Data Source=localhost,14330;Initial Catalog=ID;User ID=sa;Password=Password12345</KeysConnectionString>
    </IdentityServerHost>
  </Sitecore>
</Settings>
