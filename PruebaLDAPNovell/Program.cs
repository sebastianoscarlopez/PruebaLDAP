using Novell.Directory.Ldap;

using System;

public class SetPassword
{
    public static void Main(String[] args)
    {


        if (args.Length < 6)
        {
            Console.Error.WriteLine("Usage:   mono SetPassword <host name> "
                + "<login dn> <password>\n"
                + "         <modify dn> <new password>");
            Console.Error.WriteLine("Example: mono SetPassword Acme.com "
                + "\"cn=Admin,o=Acme secret\"\n"
                + "         \"cn=JSmith,ou=Sales,o=Acme\""
                + " newPassword");
            Environment.Exit(1);
        }
        switch (args[0])
        {
            /*
             * Ejemplo
             * CLAVE 172.16.10.82 cn=ldap_portal,ou=DDESARROL,ou=GSISTEMAS,ou=ADM,o=garrahan G4rr4h4n19 ldaptester1 12ClaveAB
             * */
            case "CLAVE":
                CambiarClave(args[1], args[2], args[3], args[4], args[5]);
                break;
            /*
             * Ejemplo
             * ALTA 172.16.10.82 cn=ldap_portal,ou=DDESARROL,ou=GSISTEMAS,ou=ADM,o=garrahan G4rr4h4n19 ldaptester3 12ClaveAB
             * */
            case "ALTA":
                Alta(args[1], args[2], args[3], args[4], args[5], new[]{
                    ("registeredAddress", "sebastianoscarlopez@gmail.com"), // ¿Ponemos el correo personal u omitimos este atributo?

                    ("uid", $"{args[4]}"), // usuario
                    ("cn", $"{args[4]}"), // usuario
                    ("userPassword", $"{args[5]}"), // Clave
                    ("accessCardNumber", "11111111"), // DNI, nsecag
                    ("employeeNumber", "1111"), // Legajo, nuagag

                    ("employeeType", "Planta"),// agentes_activos.dclasific ?? 'Planta'
                    ("givenName", "Prueba"), // GivenName. :prenag. Ñ por N y palabras en capital
                    ("fullName", "Prueba Con ÑñÁÉÍÓÚÜáéíóúü"), // Fullname. :nusuag + ' ' + :prenag. Ñ por N y palabras en capital
                    ("sn", "Prueba"), // Surname, :nusuag. Ñ por N y palabras en capital

                    ("employeeStatus", "Activo"), // Fijo
                    ("messageServer", "cn=hpg-oes01,ou=Servers,o=GARRAHAN"), // Fijo
                    ("title", "Portal-Web"), // Fijo
                    ("securityEquals", "cn=Everyone,o=GARRAHAN"), // Fijo
                    ("securityEquals", "cn=INTERNET-General,o=GARRAHAN"), // Fijo
                    ("Profile", "cn=Usuario ZenfD - Winx Client-Server,o=GARRAHAN"), // Fijo
                    ("loginGraceRemaining", "0"), // Fijo
                    ("loginGraceLimit", "6"), // Fijo
                    ("passwordAllowChange", "TRUE"), // Fijo
                    ("passwordExpirationTime", "19920102000000Z"), // Fijo
                    ("passwordExpirationInterval", "7776000"), // Fijo
                    ("passwordMinimumLength", "5"), // Fijo
                    ("passwordRequired", "TRUE"), // Fijo
                    ("passwordUniqueRequired", "TRUE"), // Fijo
                    ("objectClass", "inetOrgPerson"), // Fijo
                    ("objectClass", "organizationalPerson"), // Fijo
                    ("objectClass", "Person"), // Fijo
                    ("objectClass", "ndsLoginProperties"), // Fijo
                    ("objectClass", "Top"), // Fijo
                    ("groupMembership", "cn=Everyone,o=GARRAHAN"), // Fijo
                    ("groupMembership", "cn=INTERNET-General,o=GARRAHAN"), // Fijo
                });
                /*
                attributeSet.Add(new LdapAttribute("cn", new string[] { $"{usuario} Prueba", $"Prueba {usuario}" }));
                attributeSet.Add(new LdapAttribute("givenname", "Prueba"));
                attributeSet.Add(new LdapAttribute("sn", $"SN{usuario}"));
                attributeSet.Add(new LdapAttribute("mail", $"{usuario}@correo.com"));

    */
                break;
        }
    }
    private static void Alta(string ldapHost, string conexionDN, string conexionClave, string usuario, string clave, (string nombre, string valor)[] atributos)
    {
        var groupdn = $"ou=UsuariosLDAP,o=garrahan";
        var userdn = $"cn={usuario},ou=UsuariosLDAP,o=garrahan";
        groupdn = userdn;

        int ldapPort = LdapConnection.DEFAULT_PORT;
        int ldapVersion = LdapConnection.Ldap_V3;
        LdapConnection lc = new LdapConnection();

        try
        {
            // connect to the server
            lc.Connect(ldapHost, ldapPort);
            lc.Bind(ldapVersion, conexionDN, conexionClave);

            //Creates the List attributes of the entry and add them to attribute set 
            LdapAttributeSet attributeSet = new LdapAttributeSet();
            attributeSet.Add(new LdapAttribute("objectclass", "inetOrgPerson"));
            foreach(var atr in atributos)
            {
                attributeSet.Add(new LdapAttribute(atr.nombre, atr.valor));
            }
            // DN of the entry to be added
            LdapEntry newEntry = new LdapEntry(userdn, attributeSet);

            //Add the entry to the directory
            lc.Add(newEntry);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.ToString());
        }
        Console.ReadKey();
        Environment.Exit(1);
    }
    public static void doCleanup(LdapConnection conn, String userdn, String groupdn)
    {
        // since we have modified the user's attributes and failed to
        // modify the group's attribute, we need to delete the modified
        // user's attribute values.

        // modifications for user
        LdapModification[] modUser = new LdapModification[2];

        // Delete the groupdn from the user's attributes
        LdapAttribute membership = new LdapAttribute("groupMembership", groupdn);
        modUser[0] = new LdapModification(LdapModification.DELETE, membership);
        LdapAttribute security = new LdapAttribute("securityEquals", groupdn);
        modUser[1] = new LdapModification(LdapModification.DELETE, security);

        try
        {
            // Modify the user's attributes
            conn.Modify(userdn, modUser);

            Console.Out.WriteLine("Deleted the modified user's attribute values.");
        }
        catch (LdapException e)
        {
            Console.Out.WriteLine("Could not delete modified user's attributes: " + e.LdapErrorMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error:" + e.Message);
            return;
        }

        return;
    }

    private static void CambiarClave(string ldapHost, string conexionDN, string conexionClave, string usuario, string clave)
    {
        int ldapPort = LdapConnection.DEFAULT_PORT;
        int ldapVersion = LdapConnection.Ldap_V3;
        LdapConnection lc = new LdapConnection();

        try
        {
            // connect to the server
            lc.Connect(ldapHost, ldapPort);
            lc.Bind(ldapVersion, conexionDN, conexionClave);
            LdapSearchResults results = lc.Search("o=garrahan", LdapConnection.SCOPE_SUB, $"(&(objectClass=user)(uid={usuario}))", null, false, (LdapSearchConstraints)null);
            var hasResult = results.hasMore();
            if (hasResult)
            {
                var next = results.next();
                var usuarioDN = next.DN;
                var attributePassword = new LdapAttribute("userPassword",
                    clave);
                lc.Modify(usuarioDN, new LdapModification(
                    LdapModification.REPLACE, attributePassword));

                Console.WriteLine("Successfully set the user's password");
            }
            else
            {

            }
            lc.Disconnect();
        }
        catch (LdapException e)
        {
            if (e.ResultCode == LdapException.NO_SUCH_OBJECT)
            {
                Console.Error.WriteLine("Error: No such entry");
            }
            else if (e.ResultCode ==
                LdapException.INSUFFICIENT_ACCESS_RIGHTS)
            {
                Console.Error.WriteLine("Error: Insufficient rights");
            }
            else
            {
                Console.Error.WriteLine("Error: " + e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.ToString());
        }
        Console.ReadKey();
        Environment.Exit(1);
    }
}